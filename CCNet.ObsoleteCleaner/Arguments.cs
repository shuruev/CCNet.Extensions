using CCNet.Common;

namespace CCNet.ObsoleteCleaner
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
		/// Gets path for internal references.
		/// </summary>
		public static string InternalReferencesPath
		{
			get { return Default.GetValue("InternalReferencesPath"); }
		}

		/// <summary>
		/// Gets period in days when build is not obsolete yet.
		/// </summary>
		public static int DaysToLive
		{
			get { return Default.GetInt32Value("DaysToLive"); }
		}
	}
}
