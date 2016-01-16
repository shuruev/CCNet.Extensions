using System;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class LibraryProjectConfiguration : ProjectConfiguration
	{
		public TargetFramework Framework { get; set; }

		public DocumentationType Documentation { get; set; }

		public string MsbuildExecutable
		{
			get
			{
				return @"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe";
				switch (Framework)
				{
					case TargetFramework.Net40:
					case TargetFramework.Net45:
						return @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe";

					default:
						throw new InvalidOperationException(String.Format("Unknown target framework '{0}'.", Framework));
				}
			}
		}

		public string NugetRestoreUrl
		{
			get
			{
				if (String.IsNullOrEmpty(Branch))
					return "$(nugetUrl)/api/v2";

				return String.Format("$(nugetUrl)/private/{0}/api/v2", Branch);
			}
		}

		public string NugetPushUrl
		{
			get
			{
				if (String.IsNullOrEmpty(Branch))
					return "$(nugetUrl)/api/v2/package";

				return String.Format("$(nugetUrl)/private/{0}/api/v2/package", Branch);
			}
		}

		public bool IncludeXmlDocumentation
		{
			get { return Documentation != DocumentationType.None; }
		}
	}
}
