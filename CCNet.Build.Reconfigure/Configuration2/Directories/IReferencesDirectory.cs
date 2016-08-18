namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses "references" directory.
	/// </summary>
	public interface IReferencesDirectory : IProjectConfiguration
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets directory for storing all the referenced projects.
		/// Files in this directory are used to trigger the build for current project after dependent projects were built.
		/// </summary>
		public static string ReferencesDirectory(this IReferencesDirectory config)
		{
			return $@"{config.ProjectDirectory()}\references";
		}
	}
}
