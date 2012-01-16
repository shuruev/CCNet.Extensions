using System;
using System.Configuration;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// Holds the debug configuration settings.
	/// </summary>
	internal class Debug
	{
		/// <summary>
		/// Singleton instance.
		/// </summary>
		public static readonly Debug Instance = new Debug();

		/// <summary>
		/// Whether the debug or production behaviour should be used.
		/// </summary>
		public readonly bool IsDebugModeEnabled;

		/// <summary>
		/// If debugMode is enabled, the value is used instead of the recipients email addresses.
		/// I.e. all the emails will be sent to this address.
		/// </summary>
		public readonly string OverrideEmail;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		private Debug()
		{
			string email = ConfigurationManager.AppSettings["Debug.OverrideEmail"];
			IsDebugModeEnabled = !String.IsNullOrEmpty(email);
			OverrideEmail = email;
		}

		/// <summary>
		/// Logs a line.
		/// </summary>
		public void Log(string format, params object[] arg)
		{
			Console.WriteLine(format, arg);
		}
	}
}
