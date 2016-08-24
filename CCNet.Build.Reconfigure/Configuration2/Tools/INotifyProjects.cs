namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project should notify other projects after successfull build.
	/// </summary>
	public interface INotifyProjects
	{
		/// <summary>
		/// Gets custom issues to check (force or ignore).
		/// </summary>
		string CustomIssues { get; }
	}
}
