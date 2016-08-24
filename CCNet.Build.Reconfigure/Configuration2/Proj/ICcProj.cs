namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses *.ccproj as a project file.
	/// </summary>
	public interface ICcProj : IProjectConfiguration
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets project file name.
		/// </summary>
		public static string ProjectFileName(this ICcProj config)
		{
			return $"{config.LocalName()}.ccproj";
		}
	}
}
