using CCNet.Common;

namespace CCNet.ProjectAdapter
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
		/// Gets current version.
		/// </summary>
		public static string CurrentVersion
		{
			get { return Default.GetValue("CurrentVersion"); }
		}

		/// <summary>
		/// Gets path for project source.
		/// </summary>
		public static string WorkingDirectorySource
		{
			get { return Default.GetValue("WorkingDirectorySource"); }
		}

		/// <summary>
		/// Gets path for related projects.
		/// </summary>
		public static string WorkingDirectoryRelated
		{
			get { return Default.GetValue("WorkingDirectoryRelated"); }
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
		/// Gets path for pinned references.
		/// </summary>
		public static string PinnedReferencesPath
		{
			get { return Default.GetValue("PinnedReferencesPath"); }
		}

		/// <summary>
		/// Gets project type.
		/// </summary>
		public static ProjectType ProjectType
		{
			get { return Default.GetEnumValue<ProjectType>("ProjectType"); }
		}

		/// <summary>
		/// Gets pinned references pack name to be used.
		/// </summary>
		public static string UsePinned
		{
			get { return Default.GetValue("UsePinned"); }
		}

		/// <summary>
		/// Gets size of virtual machine for Azure role.
		/// </summary>
		public static VmSize VmSize
		{
			get { return Default.GetEnumValue<VmSize>("VmSize"); }
		}

	}
}
