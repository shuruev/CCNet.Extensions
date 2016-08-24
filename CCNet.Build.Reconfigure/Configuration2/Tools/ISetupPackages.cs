using System;

namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project should setup NuGet packages prepared during the scenario.
	/// </summary>
	public interface ISetupPackages :
		ISourceDirectory,
		IPackagesDirectory,
		IReferencesDirectory,
		ITempDirectory
	{
		/// <summary>
		/// Gets custom versions definition, which should be used when building a project.
		/// </summary>
		string CustomVersions { get; }
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets temporary file for storing information about used packages.
		/// </summary>
		public static string TempFilePackages(this ISetupPackages config)
		{
			return $@"{config.TempDirectory()}\packages.txt";
		}

		/// <summary>
		/// Gets NuGet URL used to restore packages.
		/// </summary>
		public static string NugetRestoreUrl(this ISetupPackages config)
		{
			if (String.IsNullOrEmpty(config.Branch))
				return "$(nugetUrl)/api/v2";

			return $"$(nugetUrl)/private/{config.Branch}/api/v2";
		}
	}
}
