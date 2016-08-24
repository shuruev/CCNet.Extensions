namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project can be built as .NET assembly.
	/// </summary>
	public interface IBuildAssembly : ISourceDirectory
	{
		/*xxx/// <summary>
		/// Gets target .NET Framework version.
		/// </summary>
		TargetFramework Framework { get; }

		/// <summary>
		/// Gets XML documentation mode.
		/// </summary>
		DocumentationType Documentation { get; }

		/// <summary>
		/// Gets root namespace for an assembly.
		/// </summary>
		string RootNamespace { get; }

		/// <summary>
		/// Gets custom assembly name.
		/// </summary>
		string CustomAssemblyName { get; }

		/// <summary>
		/// Gets custom company name.
		/// </summary>
		string CustomCompanyName { get; }
		*/
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets directory which stores primary release output.
		/// </summary>
		public static string SourceDirectoryRelease(this IBuildAssembly config)
		{
			return $@"{config.SourceDirectory()}\bin\Release";
		}
	}
}
