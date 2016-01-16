using CCNet.Build.Common;
using Lean.Configuration;

namespace CCNet.Build.GenerateNuspec
{
	public static class Args
	{
		public static ArgumentProperties Current { get; set; }

		public static PackageType PackageType
		{
			get { return Current.Get<PackageType>("PackageType"); }
		}

		public static string ProjectName
		{
			get { return Current.Get<string>("ProjectName"); }
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

		public static string OutputDirectory
		{
			get { return Current.Get<string>("OutputDirectory"); }
		}

		public static bool IncludeXmlDocumentation
		{
			get { return Current.Get<bool>("IncludeXmlDocumentation"); }
		}
	}
}
