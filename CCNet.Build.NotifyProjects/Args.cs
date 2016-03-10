using CCNet.Build.Common;
using Lean.Configuration;

namespace CCNet.Build.NotifyProjects
{
	public static class Args
	{
		public static ArgumentProperties Current { get; set; }

		public static string ProjectName
		{
			get { return Current.Get<string>("ProjectName"); }
		}

		public static string BuildPath
		{
			get { return Current.Get<string>("BuildPath"); }
		}

		public static string ServerNames
		{
			get { return Current.Get<string>("ServerNames"); }
		}

		public static string ReferencesFolder
		{
			get { return Current.Get<string>("ReferencesFolder"); }
		}
	}
}
