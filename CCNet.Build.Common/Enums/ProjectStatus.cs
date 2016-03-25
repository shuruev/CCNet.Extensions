namespace CCNet.Build.Common
{
	/// <summary>
	/// Describes continuous integration status.
	/// </summary>
	public enum ProjectStatus
	{
		/// <summary>
		/// The project is deprecated or obsolete and is not being built.
		/// </summary>
		Disabled,

		/// <summary>
		/// The project has no plans to be developed any further, build process has the lowest priority.
		/// </summary>
		Legacy,

		/// <summary>
		/// Project is supported without active development, continuous integration is performed with a normal priority.
		/// </summary>
		Normal,

		/// <summary>
		/// The project is actively developed, builds are issued with the highest priority based on source code changes and updated dependencies.
		/// </summary>
		Active
	}
}
