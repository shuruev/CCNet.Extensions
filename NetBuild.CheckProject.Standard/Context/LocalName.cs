namespace NetBuild.CheckProject.Standard
{
	/// <summary>
	/// Gets local name for a project, which can be different from the project name.
	/// E.g. "Product.Core" instead of "Acme.Product.Core".
	/// </summary>
	public class LocalName : ValueProvider<string>
	{
		public override string Get(CheckContext context)
		{
			return context.Of<ProjectName>().Value;
		}
	}
}
