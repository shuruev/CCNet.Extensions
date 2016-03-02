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
		/// The project has no plans to be developed any further, build process can be forced manually.
		/// </summary>
		Legacy,

		/// <summary>
		/// This project will not be supported forever, but we still perform continuous integration for it.
		/// </summary>
		Temporary,

		/// <summary>
		/// The project is actively supported, builds are issued automatically based on source code changes and updated dependencies.
		/// </summary>
		Active
	}
}
