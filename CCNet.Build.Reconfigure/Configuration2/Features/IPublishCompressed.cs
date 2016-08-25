namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project output is compressed and published as release.
	/// </summary>
	public interface IPublishCompressed :
		IPublishDirectory,
		ITempDirectory
	{
		/// <summary>
		/// Gets file wildcards which should be excluded while publishing a release.
		/// </summary>
		string ExcludeFromPublish { get; }
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets directory for storing temporary files while publishing the release.
		/// </summary>
		public static string TempDirectoryPublish(this IPublishCompressed config)
		{
			return $@"{config.TempDirectory()}\publish";
		}

		/// <summary>
		/// Gets temporary file which stores wildcards to be excluded from the published release.
		/// </summary>
		public static string TempFileExcludeFromPublish(this IPublishCompressed config)
		{
			return $@"{config.TempDirectory()}\excludeFromPublish.txt";
		}
	}
}
