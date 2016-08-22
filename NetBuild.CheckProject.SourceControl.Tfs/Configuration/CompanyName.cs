namespace NetBuild.CheckProject.SourceControl.Tfs
{
	/// <summary>
	/// Gets TFS collection URL.
	/// E.g. "http://my-server.domain.net:8080/tfs/acme".
	/// </summary>
	public class TfsUrl : ConfigurationValue<string>
	{
		public TfsUrl()
			: base("TfsUrl")
		{
		}
	}
}
