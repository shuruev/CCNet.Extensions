namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses "packages" directory.
	/// </summary>
	public interface IPackagesDirectory : IProjectConfiguration
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets directory for storing downloaded NuGet packages.
		/// </summary>
		public static string PackagesDirectory(this IPackagesDirectory config)
		{
			return $@"{config.WorkingDirectory()}\packages";
		}
	}
}
