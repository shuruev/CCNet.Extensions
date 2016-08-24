namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project will be rendering custom report output.
	/// </summary>
	public interface ICustomReport
	{
		/// <summary>
		/// Gets page name in Confluence (includes suffix which depends on a project type).
		/// </summary>
		string ConfluencePage { get; }
	}
}
