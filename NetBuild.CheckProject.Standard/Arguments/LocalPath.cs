namespace NetBuild.CheckProject.Standard
{
	/// <summary>
	/// Gets local path for a project.
	/// E.g. "D:\BuildServer\Projects\Acme.Product.Core\source".
	/// </summary>
	public class LocalPath : ArgumentValue<string>
	{
		public LocalPath()
			: base("LocalPath")
		{
		}
	}
}
