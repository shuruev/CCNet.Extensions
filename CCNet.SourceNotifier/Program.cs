using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Xml.Linq;
using System.Xml.Xsl;
using CCNet.Common;
using CCNet.SourceNotifier.Gateways.MailGateway;
using CCNet.SourceNotifier.UserInfo;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// The entry point class.
	/// </summary>
	internal class Program
	{
		/// <summary>
		/// TeamFoundationServerGateway instance.
		/// </summary>
		private readonly TeamFoundationServerGateway m_tfsGateway;

		/// <summary>
		/// ActiveDirectoryGateway instance.
		/// </summary>
		private readonly ActiveDirectoryGateway m_adGateway;

		/// <summary>
		/// MailGateway instance.
		/// </summary>
		private readonly IMailGateway m_mailGateway;

		/// <summary>
		/// How old the pending changes should be in order to consider them "old".
		/// </summary>
		private readonly TimeSpan m_cutoffTimeSpan;

		/// <summary>
		/// Time when the Program instance has been created.
		/// Used in places where variable time is unacceptable and leads to the inconsistent behaviour.
		/// </summary>
		private readonly DateTime m_runTime;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		private Program(TeamFoundationServerGateway tfsGateway, IMailGateway mailGateway, TimeSpan cutoffTimeSpan)
		{
			m_tfsGateway = tfsGateway;
			m_adGateway = new ActiveDirectoryGateway();
			m_mailGateway = mailGateway;
			m_runTime = DateTime.Now;
			m_cutoffTimeSpan = cutoffTimeSpan;
		}

		/// <summary>
		/// Gets the time point used in pending changes filter to distinguish between "old" and not-so-old changes.
		/// </summary>
		private DateTime CutoffTime
		{
			get
			{
				return m_runTime - m_cutoffTimeSpan;
			}
		}

		/// <summary>
		/// Gets the list of old pending changes, grouped by user.
		/// </summary>
		private IEnumerable<IGrouping<IUserInfo, PendingChange>> OldCheckouts
		{
			get
			{
				return
					from tuple in m_tfsGateway.GetOldPendingChanges(CutoffTime)
					let userName = tuple.Item1.OwnerName
					let userInfo = m_adGateway.GetUserInfo(userName)
					let change = tuple.Item2
					orderby change.CreationDate
					group change by userInfo into g
					select g;
			}
		}

		/// <summary>
		/// Gets the XDocument containing all "old" pending changes.
		/// </summary>
		private XDocument OldCheckoutsAsXml
		{
			get
			{
				return XmlExporter.CreateDocument(
					from changesGroup in OldCheckouts
					select XmlExporter.ExportPendingChangesGroup(changesGroup));
			}
		}

		/// <summary>
		/// Gets the arguments for the XSLT transformation.
		/// </summary>
		private XsltArgumentList XsltArguments
		{
			get
			{
				XsltArgumentList result = new XsltArgumentList();
				result.AddParam("cutoffDays", string.Empty, XmlExporter.ExportTimeSpanDays(m_cutoffTimeSpan));
				result.AddParam("currentDate", string.Empty, XmlExporter.ExportDateTime(m_runTime));
				return result;
			}
		}

		/// <summary>
		/// Assembly entry point.
		/// </summary>
		private static int Main(string[] args)
		{
			/*xxxargs = new[]
			{
				@"Command=ReportToMaster",
				@"TfsServerUri=http://rufrt-vxbuild:8080/tfs",
				@"TfsCollectionName=SED",
				@"CutoffDays=3",
				@"SenderEmail=Igor.Prohorov@cbsinteractive.com",
				@"SenderName=Robot",
				@"MasterEmail=Igor.Prohorov@cbsinteractive.com",
			};*/

			try
			{
				Arguments arguments = new Arguments(args);
				using (var tfsGateway = new TeamFoundationServerGateway(arguments.TfsServerUri, arguments.TfsCollectionName))
				{
					using (var mailGateway = MailGatewayFactory.CreateGateway(arguments.Sender))
					{
						return (new Program(tfsGateway, mailGateway, arguments.CutoffTimeSpan)).Run(arguments);
					}
				}
			}
			catch (Exception e)
			{
				return ErrorHandler.Runtime(e);
			}
		}

		/// <summary>
		/// Instance entry point.
		/// </summary>
		private int Run(Arguments arguments)
		{
			switch (arguments.ConsoleCommandType)
			{
				case ConsoleCommandType.DisplayText:
					Display();
					break;
				case ConsoleCommandType.DisplayHtml:
					DisplayHtml();
					break;
				case ConsoleCommandType.ReportToMaster:
					ReportToMaster(arguments.MasterEmail);
					break;
				case ConsoleCommandType.ReportToUsers:
					ReportToUsers();
					break;
				default:
					throw new ApplicationException("Wrong command");
			}

			return 0;
		}

		/// <summary>
		/// Outputs the information on the old pending changes in the text format.
		/// </summary>
		private void Display()
		{
			TemplateEngine.WriteProcessed("MasterTextReport.xslt", XsltArguments, OldCheckoutsAsXml, Console.Out);
		}

		/// <summary>
		/// Outputs the information on the old pending changes in the html format.
		/// </summary>
		private void DisplayHtml()
		{
			TemplateEngine.WriteProcessed("MasterReport.xslt", XsltArguments, OldCheckoutsAsXml, Console.Out);
		}

		/// <summary>
		/// Sends the report on all old pending changes to the master.
		/// </summary>
		private void ReportToMaster(string to)
		{
			m_mailGateway.SendMessage(
				new MailAddress(to),
				"Team Foundation pending changes summary",
				TemplateEngine.GetProcessedString("MasterReport.xslt", XsltArguments, OldCheckoutsAsXml));
		}

		/// <summary>
		/// Sends the personal report to each user having some old pending changes.
		/// </summary>
		private void ReportToUsers()
		{
			foreach (var group in OldCheckouts.Where(group => group.Key.HasEmail))
			{
				XDocument data = XmlExporter.CreateDocument(XmlExporter.ExportPendingChangesGroup(group));
				m_mailGateway.SendMessage(
					new MailAddress(((RegisteredUserInfo)group.Key).EmailAddress, group.Key.DisplayName),
					"Team Foundation pending changes",
					TemplateEngine.GetProcessedString("UserReport.xslt", XsltArguments, data));
			}
		}
	}
}
