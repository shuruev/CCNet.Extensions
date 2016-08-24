namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project will be prepared during the scenario.
	/// </summary>
	public interface IPrepareProject :
		ITfsControl,
		ITempDirectory
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets temporary file for storing source codes information.
		/// </summary>
		public static string TempFileSource(this IPrepareProject config)
		{
			return $@"{config.TempDirectory()}\source.txt";
		}

		/// <summary>
		/// Gets temporary file for storing Version information.
		/// </summary>
		public static string TempFileVersion(this IPrepareProject config)
		{
			return $@"{config.TempDirectory()}\version.txt";
		}
	}
}
