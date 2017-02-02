using System;
using CCNet.Build.Common;
using Lean.Configuration;

namespace CCNet.Build.SetupPackages
{
	public static class Args
	{
		public static ArgumentProperties Current { get; set; }

		public static string ProjectFile
		{
			get { return Current.Get<string>("ProjectFile"); }
		}
		public static string BranchName
		{
			get { return Current.IsEmpty("BranchName") ? null : Current.Get<string>("BranchName"); }
		}

		public static string ReferencesPath
		{
			get { return Current.Get<string>("ReferencesPath"); }
		}

		public static string TempPath
		{
			get { return Current.Get<string>("TempPath"); }
		}

		public static string PackagesPath
		{
			get { return Current.Get("PackagesPath", String.Empty); }
		}

		public static string RelatedPath
		{
			get { return Current.Get("RelatedPath", String.Empty); }
		}

		public static string CustomVersions
		{
			get { return Current.Get("CustomVersions", String.Empty); }
		}

		public static string Dependencies
		{
			get { return Current.Get("Dependencies", String.Empty); }
		}

		public static string Bundles
		{
			get { return Current.Get("Bundles", String.Empty); }
		}

		public static string NuGetExecutable
		{
			get { return Current.Get<string>("NuGetExecutable"); }
		}

		public static string NuGetUrl
		{
			get { return Current.Get<string>("NuGetUrl"); }
		}
	}
}
