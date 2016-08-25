namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses "related" directory.
	/// </summary>
	public interface IRelatedDirectory : IProjectConfiguration
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets directory for preparing related projects.
		/// </summary>
		public static string RelatedDirectory(this IRelatedDirectory config)
		{
			return $@"{config.WorkingDirectory()}\related";
		}
	}
}
