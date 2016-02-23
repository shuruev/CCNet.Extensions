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

		public static string ServerName
		{
			get { return Current.Get<string>("ServerName"); }
		}

		public static string ProjectsPath
		{
			get { return Current.Get<string>("ProjectsPath"); }
		}

		public static string ReferencesFolder
		{
			get { return Current.Get<string>("ReferencesFolder"); }
		}
	}
}
