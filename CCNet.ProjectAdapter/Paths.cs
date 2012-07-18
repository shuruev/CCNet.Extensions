using System.IO;
using CCNet.Common;

namespace CCNet.ProjectAdapter
{
	/// <summary>
	/// Generates environment paths based on arguments.
	/// </summary>
	public static class Paths
	{
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
		/// Gets assembly information file path.
		/// </summary>
		public static string AssemblyInfoFile
		{
			get
			{
				string propertiesPath = Path.Combine(Arguments.WorkingDirectorySource, "Properties");
				return Path.Combine(propertiesPath, "AssemblyInfo.cs");
			}
		}

		/// <summary>
		/// Gets service definition file path.
		/// </summary>
		public static string ServiceDefinitionFile
		{
			get
			{
				return Path.Combine(Arguments.WorkingDirectorySource, "ServiceDefinition.csdef");
			}
		}

		/// <summary>
		/// Gets folder path from which references should be taken at first.
		/// </summary>
		public static string PinnedReferencesFolder
		{
			get
			{
				return Path.Combine(Arguments.PinnedReferencesPath, Arguments.UsePinned);
			}
		}
	}
}
