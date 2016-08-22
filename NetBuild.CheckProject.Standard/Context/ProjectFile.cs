namespace NetBuild.CheckProject.Standard
{
	/// <summary>
	/// Gets project file name (uses local name instead of a project name).
	/// E.g. "Product.Core.csproj" for the "Acme.Product.Core" project.
	/// </summary>
	public class ProjectFile : ValueProvider<string>
	{
		public override string Get(CheckContext context)
		{
			var name = context.Of<LocalName>().Value;
			var ext = context.Of<ProjectExtension>().Value;

			return $"{name}.{ext}";
		}
	}
}
