namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project use TFS location.
	/// </summary>
	public interface ITfsPath : ISourceDirectory
	{
		/// <summary>
		/// Gets or sets source control path.
		/// </summary>
		string TfsPath { get; set; }
	}
}
