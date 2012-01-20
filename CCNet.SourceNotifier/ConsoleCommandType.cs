namespace CCNet.SourceNotifier
{
	internal enum ConsoleCommandType
	{
		/// <summary>
		/// Displays report in a plain-text format.
		/// </summary>
		DisplayText,

		/// <summary>
		/// Displays report in an html format.
		/// </summary>
		DisplayHtml,

		/// <summary>
		/// Sends emails to users.
		/// </summary>
		ReportToUsers,

		/// <summary>
		/// Sends a report to master.
		/// </summary>
		ReportToMaster,
	}
}
