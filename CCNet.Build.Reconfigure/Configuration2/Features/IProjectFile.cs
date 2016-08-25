namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project has a primary project file.
	/// </summary>
	public interface IProjectFile : ISourceDirectory
	{
		/// <summary>
		/// Gets project file extension.
		/// </summary>
		string ProjectExtension { get; }
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets project file name.
		/// </summary>
		public static string ProjectFileName(this IProjectFile config)
		{
			return $"{config.LocalName()}.{config.ProjectExtension}";
		}

		/// <summary>
		/// Gets project file path.
		/// </summary>
		public static string ProjectFilePath(this IProjectFile config)
		{
			return $@"{config.SourceDirectory()}\{config.ProjectFileName()}";
		}
	}
}
