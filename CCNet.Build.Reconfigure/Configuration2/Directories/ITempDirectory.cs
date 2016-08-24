namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses "temp" directory.
	/// </summary>
	public interface ITempDirectory : IProjectConfiguration
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets directory for storing temporary files during the build process.
		/// </summary>
		public static string TempDirectory(this ITempDirectory config)
		{
			return $@"{config.WorkingDirectory()}\temp";
		}
	}
}
