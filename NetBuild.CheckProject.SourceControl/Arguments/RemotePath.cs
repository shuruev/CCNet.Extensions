namespace NetBuild.CheckProject.SourceControl
{
	/// <summary>
	/// Gets source control path for a project.
	/// E.g. "$/Main/Product/Product.Core".
	/// </summary>
	public class RemotePath : ArgumentValue<string>
	{
		public RemotePath()
			: base("remote")
		{
		}
	}
}
