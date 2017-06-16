using System;
using System.Collections.Generic;
using System.Linq;

namespace CCNet.Build.Reconfigure
{
	public partial class ConfigurationBuilder
	{
		private void WriteCheckProject(IProjectConfiguration config)
		{
			var check = config as ICheckProject;
			if (check == null)
				return;

			var args = new List<Arg>
			{
				new Arg("name", check.Name),
				new Arg("local", check.SourceDirectory()),
				new Arg("remote", check.TfsPath),
				new Arg("extension", check.ProjectExtension)
			};

			/*var company = "CNET Content Solutions";

			var assembly = config as IAssembly;
			if (assembly != null)
			{
				if (assembly.RootNamespace != null)
					args.Add(new Arg("RootNamespace", assembly.RootNamespace));

				if (assembly.CustomAssemblyName != null)
					args.Add(new Arg("AssemblyName", assembly.CustomAssemblyName));

				if (assembly.CustomCompanyName != null)
					company = assembly.CustomCompanyName;
			}

			args.Add(new Arg("CompanyName", company));*/

			var issues = new List<string>();

			issues.Add(F01_ProjectFileShouldExist);
			issues.Add(F02_AssemblyInfoShouldExist);

			issues.Add(S01_ProjectFolderShouldHaveProjectName);
			issues.Add(S02_PrimarySolutionShouldExist);
			//xxx issues.Add(S03_NugetFolderShouldNotExist);
			issues.Add(S04_PackagesFolderShouldNotExist);

			if (config is FabricApplicationProjectConfiguration)
			{
				issues.Remove(F02_AssemblyInfoShouldExist);
			}

			if (!String.IsNullOrWhiteSpace(check.CustomIssues))
			{
				var all = check.CustomIssues.Split('|');
				var force = all.Where(code => code.StartsWith("+")).Select(code => code.Substring(1)).ToList();
				var ignore = all.Where(code => code.StartsWith("-")).Select(code => code.Substring(1)).ToList();
				issues = issues.Union(force).Except(ignore).ToList();
			}

			args.Add(new Arg("issues", String.Join("|", issues)));

			ExecTask(
				"$(netBuildCheckProject)",
				"Check project",
				args.ToArray());
		}

		private void WritePrepareProject(IProjectConfiguration config)
		{
			var prepare = config as IPrepareProject;
			if (prepare == null)
				return;

			var args = new List<Arg>
			{
				new Arg("path", prepare.SourceDirectory()),
				new Arg("version", "$[$CCNetLabel]"),
				new Arg("tfs", prepare.TfsPath),
				new Arg("output", prepare.TempDirectory())
			};

			if (prepare.ProjectExtension == "csproj")
			{
				args.Add(new Arg("updateAssemblyInfo", "true"));
			}

			ExecTask(
				"$(ccnetBuildPrepareProject)",
				"Prepare project",
				args.ToArray());
		}

		private void WriteCustomReport(IProjectConfiguration config)
		{
			var report = config as ICustomReport;
			if (report == null)
				return;

			var args = new List<Arg>
			{
				new Arg("confluence", report.ConfluencePage)
			};

			var nuget = config as INugetPackage;
			if (nuget != null)
			{
				args.Add(new Arg("nuget", nuget.UniqueName()));
			}

			ExecTask(
				"$(ccnetBuildCustomReport)",
				"Report custom output",
				args.ToArray());
		}

		private void WriteXxx(IProjectConfiguration config)
		{
			var xxx = config as FabricServiceProjectConfiguration;
			if (xxx == null)
				return;

			if (xxx.Name == "Metro.Portal.Web")
			{
				using (Tag("exec"))
				{
					Tag("executable", @"C:\Program Files\nodejs\npm.cmd");
					Tag("buildTimeoutSeconds", "180");
					Tag("buildArgs", "install");
					Tag("baseDirectory", xxx.SourceDirectory());
					Tag("description", "XXX npm");
				}

				using (Tag("msbuild"))
				{
					Tag("executable", "$(msbuildExecutable)");
					Tag("targets", "Build");
					Tag("workingDirectory", xxx.SourceDirectory());
					Tag("buildArgs", "/noconsolelogger /p:Configuration=Release");
					Tag("description", "Build XXX project");
				}
			}
		}

		private void WriteSetupPackages(IProjectConfiguration config)
		{
			var setup = config as ISetupPackages;
			if (setup == null)
				return;

			//xxx working on xproj here
			if (setup.ProjectExtension == "xproj")
				return;

			var args = new List<Arg>
			{
				new Arg("ProjectFile", setup.ProjectFilePath()),
				new Arg("PackagesPath", setup.PackagesDirectory()),
				new Arg("ReferencesPath", setup.ReferencesDirectory()),
				new Arg("TempPath", setup.TempDirectory()),
				new Arg("NuGetExecutable", "$(nugetExecutable)"),
				new Arg("NuGetUrl", setup.NugetRestoreUrl())
			};

			var related = config as IResolveRelated;
			if (related != null)
				args.Add(new Arg("RelatedPath", related.RelatedDirectory()));

			if (setup.CustomVersions != null)
				args.Add(new Arg("CustomVersions", setup.CustomVersions));

			var bundle = config as IMakeBundle;
			if (bundle != null)
			{
				args.Add(new Arg("Bundles", bundle.Bundles));
			}

			var nuget = config as INugetPackage;
			if (nuget != null)
			{
				args.Add(new Arg("Dependencies", nuget.Dependencies));
			}

			ExecTaskLegacy(
				"$(ccnetBuildSetupPackages)",
				"Setup packages",
				TimeSpan.FromSeconds(120),
				args.ToArray());
		}

		private void WriteBuildAssembly(IProjectConfiguration config)
		{
			var build = config as IBuildAssembly;
			if (build == null)
				return;

			//xxx working on xproj here
			if (config is IProjectFile)
			{
				if (((IProjectFile)config).ProjectExtension == "xproj")
				return;
			}

			using (Tag("msbuild"))
			{
				Tag("executable", "$(msbuildExecutable)");
				Tag("targets", "Build");
				Tag("workingDirectory", build.SourceDirectory());
				Tag("buildArgs", "/noconsolelogger /p:Configuration=Release");
				Tag("description", "Build project");
			}
		}

		private void WritePackageFabric(IProjectConfiguration config)
		{
			var fabric = config as FabricApplicationProjectConfiguration;
			if (fabric == null)
				return;

			using (Tag("msbuild"))
			{
				Tag("executable", "$(msbuildExecutable)");
				Tag("targets", "Package");
				Tag("workingDirectory", fabric.SourceDirectory());
				Tag("buildArgs", "/noconsolelogger /p:Configuration=Release");
				Tag("description", "Package fabric application");
			}
		}

		private void WritePublishRelease(IProjectConfiguration config)
		{
			var release = config as IPublishRelease;
			if (release == null)
				return;

			using (CbTag("CopyFiles"))
			{
				Attr("from", $@"{release.SourceDirectoryRelease()}\*");
				Attr("to", release.TempDirectoryPublish());
			}

			using (CbTag("EraseXmlDocs"))
			{
				Attr("path", release.TempDirectoryPublish());
			}

			using (CbTag("EraseConfigFiles"))
			{
				Attr("path", release.TempDirectoryPublish());
			}
		}

		private void WritePublishFabric(IProjectConfiguration config)
		{
			var fabric = config as FabricApplicationProjectConfiguration;
			if (fabric == null)
				return;

			using (CbTag("CopyFiles"))
			{
				Attr("from", $@"{fabric.SourceDirectoryPackage()}\*");
				Attr("to", fabric.TempDirectoryPublish());
			}
		}

		private void WritePublishCompressed(IProjectConfiguration config)
		{
			var compressed = config as IPublishCompressed;
			if (compressed == null)
				return;

			if (compressed.ExcludeFromPublish != null)
			{
				foreach (var exclude in compressed.ExcludeFromPublish.Split('|'))
				{
					using (CbTag("AppendToFile"))
					{
						Attr("file", compressed.TempFileExcludeFromPublish());
						Attr("text", exclude);
					}
				}

				using (CbTag("CompressDirectoryExclude"))
				{
					Attr("path", compressed.TempDirectoryPublish());
					Attr("output", compressed.PublishReleaseFile());
					Attr("exclude", compressed.TempFileExcludeFromPublish());
				}
			}
			else
			{
				using (CbTag("CompressDirectory"))
				{
					Attr("path", compressed.TempDirectoryPublish());
					Attr("output", compressed.PublishReleaseFile());
				}
			}

			AzureUpload(config, "publish", compressed.PublishReleaseFile());
		}

		private void WriteSaveSnapshot(IProjectConfiguration config)
		{
			var snapshot = config as ISaveSnapshot;
			if (snapshot == null)
				return;

			using (CbTag("AppendToFile"))
			{
				Attr("file", snapshot.TempFileExcludeFromSnapshot());
				Attr("text", "$tf");
			}

			using (CbTag("AppendToFile"))
			{
				Attr("file", snapshot.TempFileExcludeFromSnapshot());
				Attr("text", "*.nupkg");
			}

			var source = config as ISourceDirectory;
			if (source != null)
			{
				using (CbTag("CompressDirectoryExclude"))
				{
					Attr("path", source.SourceDirectory());
					Attr("output", snapshot.SnapshotSourceFile());
					Attr("exclude", snapshot.TempFileExcludeFromSnapshot());
				}

				AzureUpload(config, "snapshot", snapshot.SnapshotSourceFile());
			}

			var packages = config as IPackagesDirectory;
			if (packages != null)
			{
				using (CbTag("CompressDirectoryExclude"))
				{
					Attr("path", packages.PackagesDirectory());
					Attr("output", snapshot.SnapshotPackagesFile());
					Attr("exclude", snapshot.TempFileExcludeFromSnapshot());
				}

				AzureUpload(config, "snapshot", snapshot.SnapshotPackagesFile());
			}
		}

		private void WriteCompleteBuild(IProjectConfiguration config)
		{
			var setup = config as ISetupPackages;
			if (setup != null)
			{
				AzureUpload(config, "build", setup.TempFilePackages());
			}

			var prepare = config as IPrepareProject;
			if (prepare != null)
			{
				AzureUpload(config, "build", prepare.TempFileSource());
				AzureUpload(config, "build", prepare.TempFileVersion(), false);
			}
		}

		private void WriteNotifyProjects(IProjectConfiguration config)
		{
			var notify = config as INotifyProjects;
			if (notify == null)
				return;

			ExecTaskLegacy(
				"$(ccnetBuildNotifyProjects)",
				"Notify other projects",
				new Arg("ProjectName", config.Name),
				new Arg("BuildPath", "$(buildPath)"),
				new Arg("ServerNames", "Library|Website|Service|Application|Azure"),
				new Arg("ReferencesFolder", "references"));
		}
	}
}
