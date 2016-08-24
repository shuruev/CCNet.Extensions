namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project source files and packages should be saved as a snapshot.
	/// </summary>
	public interface ISaveSnapshot :
		ISnapshotDirectory,
		ITempDirectory
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets temporary file which stores wildcards to be excluded from the snapshot.
		/// </summary>
		public static string TempFileExcludeFromSnapshot(this ISaveSnapshot config)
		{
			return $@"{config.TempDirectory()}\excludeFromSnapshot.txt";
		}
	}
}
