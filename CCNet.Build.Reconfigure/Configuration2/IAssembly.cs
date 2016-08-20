using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project can be built as .NET assembly.
	/// </summary>
	public interface IAssembly
	{
		/// <summary>
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

		/// <summary>
		/// Gets custom versions definition, which should be used when building a project.
		/// </summary>
		string CustomVersions { get; }
	}
}
