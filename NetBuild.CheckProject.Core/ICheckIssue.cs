namespace NetBuild.CheckProject
{
	/// <summary>
	/// Standalone module for performing consistency check.
	/// </summary>
	public interface ICheckIssue
	{
		/// <summary>
		/// Gets issue code to check.
		/// </summary>
		string Issue { get; }

		/// <summary>
		/// Performs checking procedure.
		/// </summary>
		void Check(CheckContext context);
	}
}
