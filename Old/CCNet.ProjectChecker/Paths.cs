using System.IO;
using CCNet.Common;

namespace CCNet.ProjectChecker
{
	/// <summary>
	/// Generates environment paths based on arguments.
	/// </summary>
	public static class Paths
	{
		/// <summary>
		/// Gets properties folder path.
		/// </summary>
		private static string PropertiesFolder
		{
			get { return Path.Combine(Arguments.WorkingDirectorySource, "Properties"); }
		}

		/// <summary>
		/// Gets project file extension.
		/// </summary>
		public static string ProjectFileExtension
		{
			get
			{
				if (Arguments.ProjectType == ProjectType.Azure)
					return "ccproj";

				return "csproj";
			}
		}

		/// <summary>
		/// Gets project file path.
		/// </summary>
		public static string ProjectFile
		{
			get
			{
				string file = "{0}.{1}".Display(Arguments.ProjectName, ProjectFileExtension);
				return Path.Combine(Arguments.WorkingDirectorySource, file);
			}
		}

		/// <summary>
		/// Gets manifest file path.
		/// </summary>
		public static string ManifestFile
		{
			get
			{
				string file = "App.manifest";
				return Path.Combine(PropertiesFolder, file);
			}
		}

		/// <summary>
		/// Gets assembly information file path.
		/// </summary>
		public static string AssemblyInfoFile
		{
			get
			{
				string file = "AssemblyInfo.cs";
				return Path.Combine(PropertiesFolder, file);
			}
		}

		/// <summary>
		/// Gets source control project metadata file path.
		/// </summary>
		public static string SourceControlProjectMetadataFile
		{
			get
			{
				string file = "{0}.{1}.vspscc".Display(Arguments.ProjectName, ProjectFileExtension);
				return Path.Combine(Arguments.WorkingDirectorySource, file);
			}
		}
	}
}
