namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses "source" directory.
	/// </summary>
	public interface ISourceDirectory : IProjectConfiguration
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets directory for storing source codes for the current project.
		/// </summary>
		public static string SourceDirectory(this ISourceDirectory config)
		{
			return $@"{config.WorkingDirectory()}\source";
		}
	}
}
