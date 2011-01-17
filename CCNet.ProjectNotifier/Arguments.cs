using CCNet.Common;

namespace CCNet.ProjectNotifier
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
		/// Gets root path for project folders.
		/// </summary>
		public static string RootPath
		{
			get { return Default.GetValue("RootPath"); }
		}

		/// <summary>
		/// Gets references folder name.
		/// </summary>
		public static string ReferencesFolderName
		{
			get { return Default.GetValue("ReferencesFolderName"); }
		}
	}
}
