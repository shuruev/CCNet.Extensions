using System;
using System.Net.Mail;
using CCNet.Common;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// Command line properties for current project.
	/// </summary>
	internal class Arguments
	{
		/// <summary>
		/// Gets properties instance.
		/// </summary>
		private readonly ArgumentProperties m_properties;

		/// <summary>
		/// Gets command name.
		/// </summary>
		public ConsoleCommandType ConsoleCommandType
		{
			get { return m_properties.GetEnumValue<ConsoleCommandType>("Command"); }
		}

		/// <summary>
		/// Gets email we should send master reports to.
		/// </summary>
		public string MasterEmail
		{
			get { return m_properties.GetValue("MasterEmail"); }
		}

		/// <summary>
		/// Gets URI of a TFS server.
		/// </summary>
		public Uri TfsServerUri
		{
			get { return new Uri(m_properties.GetValue("TfsServerUri")); }
		}

		/// <summary>
		/// Gets name of TFS collection.
		/// </summary>
		public string TfsCollectionName
		{
			get { return m_properties.GetValue("TfsCollectionName"); }
		}

		/// <summary>
		/// Gets how old the pending changes should be in order to consider them "old".
		/// Note that the value is positive TimeSpan.
		/// Example: CutoffTImeSpan.TotalDays = 14 means that the search will be performed for pending changes more than 2 weeks old.
		/// </summary>
		public TimeSpan CutoffTimeSpan
		{
			get { return TimeSpan.FromDays(m_properties.GetInt32Value("CutoffDays")); }
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public Arguments(string[] args)
		{
			m_properties = ArgumentProperties.Parse(args);
		}
	}
}
