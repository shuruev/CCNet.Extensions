using CCNet.Build.Common;
using Lean.Configuration;

namespace CCNet.Build.SetupProject
{
	public static class Args
	{
		public static ArgumentProperties Current { get; set; }

		public static ProjectType ProjectType
		{
			get { return Current.Get<ProjectType>("ProjectType"); }
		}

		public static string ProjectName
		{
			get { return Current.Get<string>("ProjectName"); }
		}

		public static string ProjectPath
		{
			get { return Current.Get<string>("ProjectPath"); }
		}

		public static string CurrentVersion
		{
			get { return Current.Get<string>("CurrentVersion"); }
		}
	}
}
