namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses TFS source control location.
	/// </summary>
	public interface ITfsPath : ISourceDirectory
	{
		/// <summary>
		/// Gets source control path.
		/// </summary>
		string TfsPath { get; set; }
	}
}
