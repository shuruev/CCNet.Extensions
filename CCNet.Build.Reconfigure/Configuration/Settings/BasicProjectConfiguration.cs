using System;
using System.Collections.Generic;
using System.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public abstract class BasicProjectConfiguration : ProjectConfiguration
	{
		public abstract ProjectType Type { get; }
		public TargetFramework Framework { get; set; }
		public DocumentationType Documentation { get; set; }
		public string RootNamespace { get; set; }
		public string CustomVersions { get; set; }
		public List<string> IgnoreChecks { get; set; }
		public List<string> ForceChecks { get; set; }

		protected BasicProjectConfiguration()
		{
			IgnoreChecks = new List<string>();
			ForceChecks = new List<string>();
		}

		public virtual string SourceDirectoryRelease => $@"{WorkingDirectorySource}\bin\Release";
		public string SourceDirectoryAppPublish => $@"{SourceDirectoryRelease}\app.publish";

		public string MsbuildExecutable => @"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe";

		public string NugetRestoreUrl
		{
			get
			{
				if (String.IsNullOrEmpty(Branch))
					return "$(nugetUrl)/api/v2";

				return $"$(nugetUrl)/private/{Branch}/api/v2";
			}
		}

		public bool IncludeXmlDocumentation => Documentation != DocumentationType.None;

		public string CheckIssues
		{
			get
			{
				return String.Join(
					"|",
					GetIssuesToCheck()
						.Except(IgnoreChecks)
						.Union(ForceChecks)
						.Where(code => code != null));
			}
		}

		private string ProjectTargetFrameworkIssue
		{
			get
			{
				switch (Framework)
				{
					case TargetFramework.Net20:
						return "P10"; // ProjectTargetFramework20

					case TargetFramework.Net35:
						return "P11"; // ProjectTargetFramework35

					case TargetFramework.Net40:
						return "P12"; // ProjectTargetFramework40

					case TargetFramework.Net45:
						return "P13"; // ProjectTargetFramework45

					case TargetFramework.Net452:
						return "P19"; // ProjectTargetFramework452

					case TargetFramework.Net461:
						return "P18"; // ProjectTargetFramework461

					case TargetFramework.Net462:
						return "P22"; // ProjectTargetFramework462

					default:
						throw new InvalidOperationException(
							$"Unknown target framework '{Framework}'.");
				}
			}
		}

		private string DocumentationIssue
		{
			get
			{
				switch (Documentation)
				{
					case DocumentationType.None:
						return "P15"; // ProjectDocumentationNone

					case DocumentationType.Partial:
						return "P16"; // ProjectDocumentationPartial

					case DocumentationType.Full:
						return "P17"; // ProjectDocumentationFull

					default:
						throw new InvalidOperationException(
							$"Unknown documentation type '{Documentation}'.");
				}
			}
		}

		protected virtual List<string> GetIssuesToCheck()
		{
			var checks = new List<string>();

			// file structure
			checks.AddRange(new[]
			{
				"F01", // ProjectFolderShouldHaveProjectName
				"F02", // ProjectFileShouldExist
				"F03", // AssemblyInfoShouldExist
				"F04", // PrimarySolutionShouldExist
				"F05", // NugetConfigShouldExist
				"F06", // PackagesFolderShouldNotHavePackages
				"F07", // LocalFilesShouldMatchProjectFiles
				null
			});

			// file contents
			checks.AddRange(new[]
			{
				"C01", // AllFilesShouldUseUtf8
				"C02", // CheckAssemblyInfo
				"C03", // CheckPrimarySolution
				"C04", // CheckNugetConfig
				null
			});

			// project properties
			checks.AddRange(new[]
			{
				"P01", // CheckProjectConfigurations
				"P02", // CheckProjectPlatforms
				"P03", // CheckProjectSourceControl
				"P07", // CheckProjectAssemblyName
				"P08", // CheckProjectRootNamespace
				"P09", // CheckProjectStartupObject
				"P14", // CheckProjectCompilation
				"P20", // OutputPathDefault
				null
			});

			checks.Add("P04"); // ProjectOutputTypeLibrary

			checks.Add(ProjectTargetFrameworkIssue);
			checks.Add(DocumentationIssue);

			return checks;
		}
	}
}
