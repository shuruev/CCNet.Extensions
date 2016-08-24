namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses *.sfproj as a project file.
	/// </summary>
	public interface ISfProj : IProjectConfiguration
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets project file name.
		/// </summary>
		public static string ProjectFileName(this ISfProj config)
		{
			return $"{config.LocalName()}.sfproj";
		}
	}
}
