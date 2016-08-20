using CCNet.Build.Common;
using Lean.Configuration;

namespace CCNet.Build.CheckProject
{
	public static class Args
	{
		public static ArgumentProperties Current { get; set; }

		public static string ProjectName
		{
			get { return Current.Get<string>("ProjectName"); }
		}

		public static string CompanyName
		{
			get { return Current.Get<string>("CompanyName"); }
		}

		public static string AssemblyName
		{
			get { return Current.Get("AssemblyName", ProjectName); }
		}

		public static string RootNamespace
		{
			get { return Current.Get("RootNamespace", ProjectName); }
		}

		public static string ProjectPath
		{
			get { return Current.Get<string>("ProjectPath"); }
		}

		public static string TfsPath
		{
			get { return Current.Get<string>("TfsPath"); }
		}

		public static string CheckIssues
		{
			get { return Current.Get<string>("CheckIssues"); }
		}
	}
}
