using CCNet.Common;

namespace CCNet.ServiceChecker
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
		/// Gets service name.
		/// </summary>
		public static string ServiceName
		{
			get { return Default.GetValue("ServiceName"); }
		}

		/// <summary>
		/// Gets display name.
		/// </summary>
		public static string DisplayName
		{
			get { return Default.GetValue("DisplayName"); }
		}

		/// <summary>
		/// Gets target framework.
		/// </summary>
		public static TargetFramework TargetFramework
		{
			get { return Default.GetEnumValue<TargetFramework>("TargetFramework"); }
		}

		/// <summary>
		/// Gets path to *.exe file.
		/// </summary>
		public static string BinaryPathName
		{
			get { return Default.GetValue("BinaryPathName"); }
		}
	}
}
