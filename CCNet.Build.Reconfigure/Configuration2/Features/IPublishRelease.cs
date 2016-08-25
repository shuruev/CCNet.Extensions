namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project compilation result should be published as a release.
	/// </summary>
	public interface IPublishRelease :
		IPublishCompressed,
		IBuildAssembly
	{
	}
}
