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

		/// <summary>
		/// Gets temporary file for storing source codes information.
		/// </summary>
		public static string TempFileSource(this ITempDirectory config)
		{
			return $@"{config.TempDirectory()}\source.txt";
		}

		/// <summary>
		/// Gets temporary file for storing information about used packages.
		/// </summary>
		public static string TempFilePackages(this ITempDirectory config)
		{
			return $@"{config.TempDirectory()}\packages.txt";
		}

		/// <summary>
		/// Gets temporary file for storing Version information.
		/// </summary>
		public static string TempFileVersion(this ITempDirectory config)
		{
			return $@"{config.TempDirectory()}\version.txt";
		}
	}
}
