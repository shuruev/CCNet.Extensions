namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project will be checked during the scenario.
	/// </summary>
	public interface ICheckProject : ITfsControl
	{
		/// <summary>
		/// Gets custom issues to check (force or ignore).
		/// </summary>
		string CustomIssues { get; }
	}
}
