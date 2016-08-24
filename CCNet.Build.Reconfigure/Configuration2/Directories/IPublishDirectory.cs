namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses "publish" directory.
	/// </summary>
	public interface IPublishDirectory : IProjectConfiguration
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets directory for preparing release files to publish.
		/// </summary>
		public static string PublishDirectory(this IPublishDirectory config)
		{
			return $@"{config.WorkingDirectory()}\publish";
		}

		/// <summary>
		/// Gets local file with a compressed release to publish.
		/// </summary>
		public static string PublishReleaseFile(this IPublishDirectory config)
		{
			return $@"{config.PublishDirectory()}\{config.Name}.zip";
		}
	}
}
