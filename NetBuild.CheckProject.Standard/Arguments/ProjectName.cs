namespace NetBuild.CheckProject.Standard
{
	/// <summary>
	/// Gets project name.
	/// E.g. "Acme.Product.Core".
	/// </summary>
	public class ProjectName : ArgumentValue<string>
	{
		public ProjectName()
			: base("ProjectName")
		{
		}
	}
}
