using CCNet.Common;

namespace CCNet.ProjectChecker
{
	/// <summary>
	/// Command line properties for current project.
	/// </summary>
	public static class Arguments
	{
		/// <summary>
		/// Gets or sets a default instance.
		/// </summary>
		public static ArgumentProperties Default { get; set; }

		/// <summary>
		/// Gets project name.
		/// </summary>
		public static string ProjectName
		{
			get { return Default.GetValue("ProjectName"); }
		}

		/// <summary>
		/// Gets path for project references folder.
		/// </summary>
		public static string ReferencesDirectory
		{
			get { return Default.GetValue("ReferencesDirectory"); }
		}

		/// <summary>
		/// Gets path for project source.
		/// </summary>
		public static string WorkingDirectorySource
		{
			get { return Default.GetValue("WorkingDirectorySource"); }
		}

		/// <summary>
		/// Gets path for external references.
		/// </summary>
		public static string ExternalReferencesPath
		{
			get { return Default.GetValue("ExternalReferencesPath"); }
		}

		/// <summary>
		/// Gets path for internal references.
		/// </summary>
		public static string InternalReferencesPath
		{
			get { return Default.GetValue("InternalReferencesPath"); }
		}

		/// <summary>
		/// Gets project type.
		/// </summary>
		public static ProjectType ProjectType
		{
			get { return Default.GetEnumValue<ProjectType>("ProjectType"); }
		}

		/// <summary>
		/// Gets assembly name.
		/// </summary>
		public static string AssemblyName
		{
			get { return Default.GetValue("AssemblyName"); }
		}

		/// <summary>
		/// Gets project friendly name.
		/// </summary>
		public static string FriendlyName
		{
			get { return Default.GetValue("FriendlyName"); }
		}

		/// <summary>
		/// Gets project download zone.
		/// </summary>
		public static string DownloadZone
		{
			get { return Default.GetValue("DownloadZone"); }
		}

		/// <summary>
		/// Gets Visual Studio version.
		/// </summary>
		public static string VisualStudioVersion
		{
			get { return Default.GetValue("VisualStudioVersion"); }
		}

		/// <summary>
		/// Gets target framework.
		/// </summary>
		public static TargetFramework TargetFramework
		{
			get { return Default.GetEnumValue<TargetFramework>("TargetFramework"); }
		}

		/// <summary>
		/// Gets target platform.
		/// </summary>
		public static string TargetPlatform
		{
			get { return Default.GetValue("TargetPlatform"); }
		}

		/// <summary>
		/// Gets root namespace.
		/// </summary>
		public static string RootNamespace
		{
			get { return Default.GetValue("RootNamespace"); }
		}

		/// <summary>
		/// Gets suppressed warnings.
		/// </summary>
		public static string SuppressWarnings
		{
			get { return Default.GetValue("SuppressWarnings"); }
		}

		/// <summary>
		/// Gets a value indicating whether unsafe blocks are supported.
		/// </summary>
		public static bool AllowUnsafeBlocks
		{
			get { return Default.GetBooleanValue("AllowUnsafeBlocks"); }
		}

		/// <summary>
		/// Gets expected version.
		/// </summary>
		public static string ExpectedVersion
		{
			get { return Default.GetValue("ExpectedVersion"); }
		}
	}
}
