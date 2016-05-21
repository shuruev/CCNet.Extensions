using System;
using CCNet.Build.Common;
using Lean.Configuration;

namespace CCNet.Build.GenerateNuspec
{
	public static class Args
	{
		public static ArgumentProperties Current { get; set; }

		public static string ProjectName
		{
			get { return Current.Get<string>("ProjectName"); }
		}

		public static string PackageId
		{
			get { return Current.Get("PackageId", ProjectName); }
		}

		public static string ProjectDescription
		{
			get { return Current.Get<string>("ProjectDescription"); }
		}

		public static string CompanyName
		{
			get { return Current.Get<string>("CompanyName"); }
		}

		public static string CurrentVersion
		{
			get { return Current.Get<string>("CurrentVersion"); }
		}

		public static TargetFramework TargetFramework
		{
			get { return Current.Get<TargetFramework>("TargetFramework"); }
		}

		public static bool IncludeXmlDocumentation
		{
			get { return Current.Get("IncludeXmlDocumentation", false); }
		}

		public static bool MarkAsCustom
		{
			get { return Current.Get("MarkAsCustom", false); }
		}

		public static bool MarkAsStatic
		{
			get { return Current.Get("MarkAsStatic", false); }
		}

		public static string Dependencies
		{
			get { return Current.Get("Dependencies", String.Empty); }
		}

		public static string ReleaseNotes
		{
			get { return Current.Get("ReleaseNotes", String.Empty); }
		}

		public static string ReleasePath
		{
			get { return Current.Get<string>("ReleasePath"); }
		}

		public static string OutputDirectory
		{
			get { return Current.Get<string>("OutputDirectory"); }
		}
	}
}
