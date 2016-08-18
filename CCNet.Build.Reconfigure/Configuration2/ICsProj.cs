namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses *.csproj as a project file.
	/// </summary>
	public interface ICsProj : IProjectConfiguration
	{
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets project file name.
		/// </summary>
		public static string ProjectFileName(this ICsProj config)
		{
			return $"{config.LocalName()}.csproj";
		}
	}
}
