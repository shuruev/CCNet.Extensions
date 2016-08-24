namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses "snapshot" directory.
	/// </summary>
	public interface ISnapshotDirectory : IProjectConfiguration
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets directory for preparing snapshot files to save.
		/// </summary>
		public static string SnapshotDirectory(this ISnapshotDirectory config)
		{
			return $@"{config.WorkingDirectory()}\snapshot";
		}

		/// <summary>
		/// Gets file name for source snapshot.
		/// </summary>
		public static string SnapshotSourceFile(this ISnapshotDirectory config)
		{
			return $@"{config.SnapshotDirectory()}\source.zip";
		}

		/// <summary>
		/// Gets file name for packages snapshot.
		/// </summary>
		public static string SnapshotPackagesFile(this ISnapshotDirectory config)
		{
			return $@"{config.SnapshotDirectory()}\packages.zip";
		}
	}
}
