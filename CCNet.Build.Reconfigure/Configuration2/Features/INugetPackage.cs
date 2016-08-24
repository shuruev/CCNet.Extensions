namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project output should be published to NuGet repository.
	/// </summary>
	public interface INugetPackage : IProjectConfiguration
	{
		/// <summary>
		/// Gets other assemblies which should be marked as dependencies for the current assembly.
		/// </summary>
		string Dependencies { get; }
	}
}
