using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace ConsoleApplication1
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
		private readonly MailGateway m_mailGateway;

		/// <summary>
		/// Time when the Program instance has been created.
		/// Used in places where variable time is unacceptable and leads to the inconsistent behaviour.
		/// </summary>
		private readonly DateTime m_runTime;

		/// <summary>
		/// Constructor.
		/// </summary>
		private Program(TeamFoundationServerGateway tfsGateway, MailGateway mailGateway)
		{
			m_tfsGateway = tfsGateway;
			m_adGateway = new ActiveDirectoryGateway();
			m_mailGateway = mailGateway;
			m_runTime = DateTime.Now;
		}

		/// <summary>
		/// Gets the time point used in pending changes filter to distinguish between "old" and not-so-old changes.
		/// </summary>
		private DateTime CutoffTime
		{
			get
			{
				return m_runTime - Config.Instance.CutoffTimeSpan;
			}
		}

		/// <summary>
		/// Gets the list of old pending changes, grouped by user.
		/// </summary>
		private IEnumerable<IGrouping<UserInfo, PendingChange>> OldCheckouts
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
				return XmlExporter.CreateDocument(from changesGroup in OldCheckouts select XmlExporter.ExportPendingChangesGroup(changesGroup));
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
				result.AddParam("cutoffDays", string.Empty, XmlExporter.ExportTimeSpanDays(Config.Instance.CutoffTimeSpan));
				result.AddParam("currentDate", string.Empty, XmlExporter.ExportDateTime(m_runTime));
				return result;
			}
		}

		/// <summary>
		/// Assembly entry point.
		/// </summary>
		private static void Main(string[] args)
		{
			using (TeamFoundationServerGateway tfsGateway = new TeamFoundationServerGateway(Config.Instance.TfsServerUri, Config.Instance.TfsCollectionName))
			{
				using (MailGateway mailGateway = MailGateway.CreateGateway(Config.Instance.Sender))
				{
					(new Program(tfsGateway, mailGateway)).Run(args);
				}
			}
		}

		/// <summary>
		/// Instance entry point.
		/// </summary>
		private void Run(string[] args)
		{
			switch (args[0])
			{
				case "display":
					Display();
					break;
				case "displayhtml":
					DisplayHtml();
					break;
				case "reporttomaster":
					ReportMaster(args[1]);
					break;
				case "reporttousers":
					ReportToUsers();
					break;
				default:
					throw new ApplicationException("Wrong command");
			}
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
		private void ReportMaster(string to)
		{
			m_mailGateway.SendMessage(
				new MailAddress(to),
				"Old checkouts stats",
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
					new MailAddress(((UserInfo.RegisteredUserInfo)group.Key).EmailAddress, group.Key.DisplayName),
					"You have old checkouts",
					TemplateEngine.GetProcessedString("UserReport.xslt", XsltArguments, data));
			}
		}
	}
}
