namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// All build project use "admin" directory.
	/// </summary>
	public interface IAdminDirectory
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets special server directory for administrative features.
		/// </summary>
		public static string AdminDirectory(this IAdminDirectory config)
		{
			return "$(adminPath)";
		}

		/// <summary>
		/// Gets service directory which is used to trigger rebuild for all the projects.
		/// </summary>
		public static string AdminDirectoryRebuildAll(this IAdminDirectory config)
		{
			return $@"{config.AdminDirectory()}\RebuildAll";
		}
	}
}
