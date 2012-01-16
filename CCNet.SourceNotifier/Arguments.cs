using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCNet.Common;
using System.Net.Mail;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// Command line properties for current project.
	/// </summary>
	internal class Arguments
	{
		/// <summary>
		/// Properties instance.
		/// </summary>
		private readonly ArgumentProperties m_properties;

		/// <summary>
		/// Command name.
		/// </summary>
		public ConsoleCommandType ConsoleCommandType
		{
			get { return m_properties.GetEnumValue<ConsoleCommandType>("Command"); }
		}

		/// <summary>
		/// Email we should send master reports to.
		/// </summary>
		public string MasterEmail
		{
			get { return m_properties.GetValue("MasterEmail"); }
		}

		/// <summary>
		/// URI of a TFS server.
		/// </summary>
		public Uri TfsServerUri
		{
			get { return new Uri(m_properties.GetValue("TfsServerUri")); }
		}

		/// <summary>
		/// Name of TFS collection.
		/// </summary>
		public string TfsCollectionName
		{
			get { return m_properties.GetValue("TfsCollectionName"); }
		}

		/// <summary>
		/// Mail address of a robot used to send emails.
		/// </summary>
		public MailAddress Sender
		{
			get { return new MailAddress(m_properties.GetValue("SenderEmail")); }
		}

		/// <summary>
		/// How old the pending changes should be in order to consider them "old"
		/// Note that the value is positive TimeSpan.
		/// Example: CutoffTImeSpan.TotalDays = 14 means that the search will be performed for pending changes more than 2 weeks old.
		/// </summary>
		public TimeSpan CutoffTimeSpan
		{
			get { return TimeSpan.FromDays(m_properties.GetInt32Value("CutoffDays")); }
		}

		public Arguments(string[] args)
		{
			m_properties = ArgumentProperties.Parse(args);
		}
	}
}
