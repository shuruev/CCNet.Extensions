using System;
using System.Configuration;
using System.Net.Mail;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// Holds all of the application configuration parameters.
	/// </summary>
	internal class Config
	{
		/// <summary>
		/// Singleton instance.
		/// </summary>
		public static readonly Config Instance = new Config();

		/// <summary>
		/// URI of a TFS server.
		/// </summary>
		public readonly Uri TfsServerUri;

		/// <summary>
		/// Name of TFS collection.
		/// </summary>
		public readonly string TfsCollectionName;

		/// <summary>
		/// Mail address of a robot used to send emails.
		/// </summary>
		public readonly MailAddress Sender;

		/// <summary>
		/// How old the pending changes should be in order to consider them "old"
		/// Note that the value is positive TimeSpan.
		/// Example: CutoffTImeSpan.TotalDays = 14 means that the search will be performed for pending changes more than 2 weeks old.
		/// </summary>
		public readonly TimeSpan CutoffTimeSpan;

		/// <summary>
		/// Constructor.
		/// </summary>
		private Config()
		{
			var appSettings = ConfigurationManager.AppSettings;
			TfsServerUri = new Uri(appSettings["tfsServerUri"]);
			TfsCollectionName = appSettings["tfsCollectionName"];
			CutoffTimeSpan = TimeSpan.FromDays(int.Parse(appSettings["cutoffDays"]));
			Sender = new MailAddress(appSettings["senderEmail"], appSettings["senderName"]);
		}
	}
}
