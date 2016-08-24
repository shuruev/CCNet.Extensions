namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project uses TFS source control location.
	/// </summary>
	public interface ITfsControl : ISourceDirectory
	{
		/// <summary>
		/// Gets source control path.
		/// </summary>
		string TfsPath { get; set; }
	}
}
