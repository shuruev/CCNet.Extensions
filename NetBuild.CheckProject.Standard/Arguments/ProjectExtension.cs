namespace NetBuild.CheckProject.Standard
{
	/// <summary>
	/// Gets project file extension.
	/// E.g. "csproj", "ccproj", "sfproj", etc.
	/// </summary>
	public class ProjectExtension : ArgumentValue<string>
	{
		public ProjectExtension()
			: base("ProjectExtension", "csproj")
		{
		}
	}
}
