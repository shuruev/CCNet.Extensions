using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CCNet.Build.Common;
using CCNet.Build.Confluence;
using CCNet.Build.Tfs;
using Arg = System.Tuple<string, object>;

namespace CCNet.Build.Reconfigure
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Execute.DisplayUsage("Generates .config files for all CCNet build servers.", typeof(Args));
				return 0;
			}

			try
			{
				Args.Current = new ArgumentProperties(args);
				Execute.DisplayCurrent(typeof(Args));

				Reconfigure();
			}
			catch (Exception e)
			{
				return Execute.RuntimeError(e);
			}

			return 0;
		}

		private static void Reconfigure()
		{
			var confluence = new CachedConfluenceClient(Config.ConfluenceUsername, Config.ConfluencePassword, Args.ConfluenceCache);
			var tfs = new TfsClient(Config.TfsUrl);
			var builder = new PageBuilder(confluence, tfs);

			using (Execute.Step("REBUILD PAGES"))
			{
				builder.Rebuild("CCSSEDRU", "Projects");
			}

			using (Execute.Step("UPDATE CONFIG"))
			{
				var configs = builder.ExportConfigurations();
				ApplyCustomizations(configs);

				BuildLibraryConfig(FilterByType<LibraryProjectConfiguration>(configs));

				BuildWebsiteConfig(
					FilterByType<WebsiteProjectConfiguration>(configs)
					.Union(FilterByType<WebserviceProjectConfiguration>(configs)));

				BuildServiceConfig(FilterByType<ServiceProjectConfiguration>(configs));

				BuildApplicationConfig(
					FilterByType<ConsoleProjectConfiguration>(configs)
					.Union(FilterByType<WindowsProjectConfiguration>(configs)));

				BuildAzureConfig(FilterByType<CloudRoleProjectConfiguration>(configs));
			}

			using (Execute.Step("SAVE GUID MAP"))
			{
				var map = builder.ExportMap();
				foreach (var item in map)
				{
					SaveProjectUid(item.Key, item.Value);
				}
			}
		}

		private static void ApplyCustomizations(List<ProjectConfiguration> configs)
		{
			LibraryProjectConfiguration library;

			library = configs.FirstOrDefault(item => item.Name == "AzureDirectory") as LibraryProjectConfiguration;
			if (library != null)
			{
				library.CustomAssemblyName = "Lucene.Net.Store.Azure";
				library.CustomPackageTitle = "AzureDirectory for Lucene.Net";
				library.CustomCompanyName = "Microsoft";
				library.Dependencies = "WindowsAzure.Storage|Lucene.Net";
			}

			library = configs.FirstOrDefault(item => item.Name == "CnetContent.FlexQueue.Client") as LibraryProjectConfiguration;
			if (library != null)
			{
				library.Dependencies = "Lean.ResourceLocators|Lean.Serialization|Lean.Rest|CnetContent.FlexQueue.Core";
			}
		}

		private static List<T> FilterByType<T>(IEnumerable<ProjectConfiguration> configs) where T : ProjectConfiguration
		{
			return configs
				.Where(c => c.GetType() == typeof(T))
				.OrderBy(c => c.UniqueName)
				.Cast<T>()
				.ToList();
		}

		private static XmlWriter WriteConfig(string filePath)
		{
			return new XmlTextWriter(filePath, Encoding.UTF8)
			{
				Formatting = Formatting.Indented,
				IndentChar = '\t',
				Indentation = 1
			};
		}

		private static void BuildLibraryConfig(IEnumerable<LibraryProjectConfiguration> configs)
		{
			Console.WriteLine("Generate library config...");
			Console.WriteLine("Output file: {0}", Paths.LibraryConfig);

			using (var writer = WriteConfig(Paths.LibraryConfig))
			{
				writer.Begin();

				writer.Comment("SERVER NAME");
				writer.CbTag("define", "serverName", "Library");

				writer.Comment("IMPORT GLOBAL");
				writer.CbTag("include", "href", "Global.config");

				foreach (var config in configs)
				{
					WriteLibraryProject(writer, config);
					Console.WriteLine("> {0}", config.UniqueName);
				}

				/*xxxWriteLibraryProject(
					writer,
					new LibraryProjectConfiguration
					{
						Name = "V3.Storage",
						Description = "Client library and value templates for V3 storage",
						Category = "ContentCast",
						TfsPath = "$/Main/ContentCast/V3/V3.Storage",
						Framework = TargetFramework.Net40,
						CustomVersions = "mongocsharpdriver",
						OwnerEmail = "oleg.shuruev@cbsinteractive.com"
					});

				WriteLibraryProject(
					writer,
					new LibraryProjectConfiguration
					{
						Branch = "Test",
						Name = "V3.Storage",
						Description = "Client library and value templates for V3 storage",
						Category = "ContentCast",
						TfsPath = "$/Main/ContentCast/V3/V3.Storage",
						Framework = TargetFramework.Net45
					});*/

				writer.End();
			}
		}

		private static void BuildWebsiteConfig(IEnumerable<WebsiteProjectConfiguration> configs)
		{
			Console.WriteLine("Generate website config...");
			Console.WriteLine("Output file: {0}", Paths.WebsiteConfig);

			using (var writer = WriteConfig(Paths.WebsiteConfig))
			{
				writer.Begin();

				writer.Comment("SERVER NAME");
				writer.CbTag("define", "serverName", "Website");

				writer.Comment("IMPORT GLOBAL");
				writer.CbTag("include", "href", "Global.config");

				foreach (var config in configs)
				{
					WriteWebsiteProject(writer, config);
					Console.WriteLine("> {0}", config.UniqueName);
				}

				writer.End();
			}
		}

		private static void BuildServiceConfig(IEnumerable<ServiceProjectConfiguration> configs)
		{
			Console.WriteLine("Generate service config...");
			Console.WriteLine("Output file: {0}", Paths.ServiceConfig);

			using (var writer = WriteConfig(Paths.ServiceConfig))
			{
				writer.Begin();

				writer.Comment("SERVER NAME");
				writer.CbTag("define", "serverName", "Service");

				writer.Comment("IMPORT GLOBAL");
				writer.CbTag("include", "href", "Global.config");

				foreach (var config in configs)
				{
					WriteServiceProject(writer, config);
					Console.WriteLine("> {0}", config.UniqueName);
				}

				writer.End();
			}
		}

		private static void BuildApplicationConfig(IEnumerable<ConsoleProjectConfiguration> configs)
		{
			Console.WriteLine("Generate application config...");
			Console.WriteLine("Output file: {0}", Paths.ApplicationConfig);

			using (var writer = WriteConfig(Paths.ApplicationConfig))
			{
				writer.Begin();

				writer.Comment("SERVER NAME");
				writer.CbTag("define", "serverName", "Application");

				writer.Comment("IMPORT GLOBAL");
				writer.CbTag("include", "href", "Global.config");

				foreach (var config in configs)
				{
					WriteApplicationProject(writer, config);
					Console.WriteLine("> {0}", config.UniqueName);
				}

				writer.End();
			}
		}

		private static void BuildAzureConfig(IEnumerable<CloudRoleProjectConfiguration> cloudRoles)
		{
			Console.WriteLine("Generate azure config...");
			Console.WriteLine("Output file: {0}", Paths.AzureConfig);

			using (var writer = WriteConfig(Paths.AzureConfig))
			{
				writer.Begin();

				writer.Comment("SERVER NAME");
				writer.CbTag("define", "serverName", "Azure");

				writer.Comment("IMPORT GLOBAL");
				writer.CbTag("include", "href", "Global.config");

				foreach (var config in cloudRoles)
				{
					WriteCloudRoleProject(writer, config);
					Console.WriteLine("> {0}", config.UniqueName);
				}

				writer.End();
			}
		}

		private static void WriteProjectHeader(XmlWriter writer, ProjectConfiguration project)
		{
			writer.WriteElementString("name", project.UniqueName);
			writer.WriteElementString("description", project.Description);
			writer.WriteElementString("queue", project.BuildQueue);
			writer.WriteElementString("category", project.Category);

			writer.WriteElementString("workingDirectory", project.WorkingDirectory);
			writer.WriteElementString("artifactDirectory", project.WorkingDirectory);
			using (writer.OpenTag("state"))
			{
				writer.WriteAttributeString("type", "state");
				writer.WriteAttributeString("directory", project.WorkingDirectory);
			}

			writer.WriteElementString("webURL", project.WebUrl);
		}

		private static void WriteSourceControl(XmlWriter writer, BasicProjectConfiguration project)
		{
			using (writer.OpenTag("sourcecontrol"))
			{
				writer.WriteAttributeString("type", "multi");
				using (writer.OpenTag("sourceControls"))
				{
					using (writer.OpenTag("vsts"))
					{
						writer.WriteElementString("executable", "$(tfsExecutable)");
						writer.WriteElementString("server", "$(tfsUrl)");
						writer.WriteElementString("project", project.TfsPath);
						writer.WriteElementString("workingDirectory", project.WorkingDirectorySource);
						writer.WriteElementString("applyLabel", "false");
						writer.WriteElementString("autoGetSource", "true");
						writer.WriteElementString("cleanCopy", "true");
						writer.WriteElementString("workspace", String.Format("CCNET_{0}_{1}", project.Type.ServerName(), project.BuildQueue));
						writer.WriteElementString("deleteWorkspace", "true");
					}

					using (writer.OpenTag("filesystem"))
					{
						writer.WriteElementString("repositoryRoot", project.WorkingDirectoryReferences);
						writer.WriteElementString("autoGetSource", "false");
						writer.WriteElementString("ignoreMissingRoot", "true");
					}

					using (writer.OpenTag("filesystem"))
					{
						writer.WriteElementString("repositoryRoot", project.AdminDirectoryRebuildAll);
						writer.WriteElementString("autoGetSource", "false");
						writer.WriteElementString("ignoreMissingRoot", "true");
					}
				}
			}

			writer.Tag("labeller", "type", "shortDateLabeller");

			using (writer.OpenTag("triggers"))
			{
				writer.Tag("intervalTrigger", "name", "source or references", "seconds", "30", "buildCondition", "IfModificationExists", "initialSeconds", "5");
			}
		}

		private static void WriteDefaultPublishers(XmlWriter writer, ProjectConfiguration project)
		{
			writer.Tag("modificationHistory", "onlyLogWhenChangesFound", "true");
			writer.Tag("xmllogger");
			writer.Tag("statistics");
			writer.Tag("artifactcleanup", "cleanUpMethod", "KeepLastXBuilds", "cleanUpValue", "100");
			writer.Tag("artifactcleanup", "cleanUpMethod", "KeepMaximumXHistoryDataEntries", "cleanUpValue", "100");

			if (!String.IsNullOrEmpty(project.OwnerEmail))
			{
				writer.CbTag("EmailPublisher", "mailto", project.OwnerEmail);
			}
		}

		private static void WriteCheckProject(XmlWriter writer, BasicProjectConfiguration project)
		{
			using (writer.OpenTag("exec"))
			{
				writer.WriteElementString("executable", "$(ccnetBuildCheckProject)");

				var args = new List<Arg>
				{
					new Arg("ProjectName", project.Name),
					new Arg(project.RootNamespace != null ? "RootNamespace" : null, project.RootNamespace),
					new Arg("ProjectPath", project.WorkingDirectorySource),
					new Arg("TfsPath", project.TfsPath)
				};

				var company = "CNET Content Solutions";

				var library = project as LibraryProjectConfiguration;
				if (library != null)
				{
					if (library.CustomAssemblyName != null)
					{
						args.Add(new Arg("AssemblyName", library.CustomAssemblyName));
					}

					if (library.CustomCompanyName != null)
					{
						company = library.CustomCompanyName;
					}
				}

				args.Add(new Arg("CompanyName", company));

				var publish = project as PublishProjectConfiguration;
				if (publish != null)
				{
					args.Add(new Arg("ProjectTitle", publish.Title));
				}

				args.Add(new Arg("CheckIssues", project.CheckIssues));

				writer.WriteBuildArgs(args.ToArray());
				writer.WriteElementString("description", "Check project");
			}
		}

		private static void WriteSetupPackages(XmlWriter writer, BasicProjectConfiguration project)
		{
			using (writer.OpenTag("exec"))
			{
				writer.WriteElementString("executable", "$(ccnetBuildSetupPackages)");
				writer.WriteElementString("buildTimeoutSeconds", "45");
				writer.WriteBuildArgs(
					new Arg("ProjectName", project.Name),
					new Arg("ProjectPath", project.WorkingDirectorySource),
					new Arg("PackagesPath", project.WorkingDirectoryPackages),
					new Arg("ReferencesPath", project.WorkingDirectoryReferences),
					new Arg("TempPath", project.WorkingDirectoryTemp),
					new Arg(project.CustomVersions != null ? "CustomVersions" : null, project.CustomVersions),
					new Arg("NuGetExecutable", "$(nugetExecutable)"),
					new Arg("NuGetUrl", project.NugetRestoreUrl));

				writer.WriteElementString("description", "Setup packages");
			}
		}

		private static void WriteBuildProject(XmlWriter writer, BasicProjectConfiguration project)
		{
			using (writer.OpenTag("msbuild"))
			{
				writer.WriteElementString("executable", project.MsbuildExecutable);
				writer.WriteElementString("targets", "Build");
				writer.WriteElementString("workingDirectory", project.WorkingDirectorySource);
				writer.WriteElementString("buildArgs", "/noconsolelogger /p:Configuration=Release");
				writer.WriteElementString("description", "Build project");
			}
		}

		private static void WriteLibraryProject(XmlWriter writer, LibraryProjectConfiguration project)
		{
			writer.Comment(String.Format("PROJECT: {0}", project.UniqueName));

			Action cleanup = () =>
			{
				WriteDefaultCleanup(writer, project);
				writer.CbTag("DeleteDirectory", "path", project.WorkingDirectoryRelease());
				writer.CbTag("DeleteDirectory", "path", project.WorkingDirectoryNuget);
			};

			using (writer.OpenTag("project"))
			{
				WriteProjectHeader(writer, project);
				WriteSourceControl(writer, project);

				using (writer.OpenTag("prebuild"))
				{
					cleanup.Invoke();
				}

				using (writer.OpenTag("tasks"))
				{
					WriteCheckProject(writer, project);

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildSetupProject)");
						writer.WriteBuildArgs(
							new Arg("ProjectType", project.Type),
							new Arg("ProjectName", project.Name),
							new Arg(project.CustomAssemblyName != null ? "PackageId" : null, project.CustomAssemblyName),
							new Arg("ProjectPath", project.WorkingDirectorySource),
							new Arg("TempPath", project.WorkingDirectoryTemp),
							new Arg("TfsPath", project.TfsPath),
							new Arg("CurrentVersion", "$[$CCNetLabel]"));

						writer.WriteElementString("description", "Setup project");
					}

					WriteSetupPackages(writer, project);
					WriteBuildProject(writer, project);

					// copy output files to the release folder, including satellite assemblies
					writer.CbTag(
						"CopyFilesWildcard",
						"from",
						project.SourceDirectoryRelease,
						"wildcard",
						String.Format("{0}.???, {0}.resources.???", project.CustomAssemblyName ?? project.Name),
						"to",
						project.WorkingDirectoryRelease());

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildGenerateNuspec)");
						writer.WriteBuildArgs(
							new Arg("ProjectName", project.Name),
							new Arg(project.CustomAssemblyName != null ? "PackageId" : null, project.CustomAssemblyName),
							new Arg(project.CustomPackageTitle != null ? "PackageTitle" : null, project.CustomPackageTitle),
							new Arg("ProjectDescription", project.Description),
							new Arg("CompanyName", project.CustomCompanyName ?? "CNET Content Solutions"),
							new Arg("CurrentVersion", "$[$CCNetLabel]"),
							new Arg("TargetFramework", project.Framework),
							new Arg(project.IncludeXmlDocumentation ? "IncludeXmlDocumentation" : null, project.IncludeXmlDocumentation),
							new Arg(project.Dependencies != null ? "Dependencies" : null, project.Dependencies),
							new Arg("ReleaseNotes", project.TempFileSource + "|" + project.TempFilePackages),
							new Arg("ReleasePath", project.WorkingDirectoryRelease()),
							new Arg("OutputDirectory", project.WorkingDirectoryNuget));

						writer.WriteElementString("description", "Generate nuspec file");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(nugetExecutable)");
						writer.WriteElementString("buildTimeoutSeconds", "45");
						writer.WriteElementString(
							"buildArgs",
							String.Format(
								@"pack ""{0}\{1}.nuspec"" -OutputDirectory ""{0}"" -MSBuildVersion 14 -NonInteractive -Verbosity Detailed",
								project.WorkingDirectoryNuget,
								project.Name));

						writer.WriteElementString("description", "Build package");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(nugetExecutable)");
						writer.WriteElementString("buildTimeoutSeconds", "45");
						writer.WriteElementString(
							"buildArgs",
							String.Format(
								@"push ""{0}\{1}.$[$CCNetLabel].nupkg"" -Source ""{2}"" -NonInteractive -Verbosity Detailed",
								project.WorkingDirectoryNuget,
								project.CustomAssemblyName ?? project.Name,
								project.NugetPushUrl));

						writer.WriteElementString("description", "Publish package");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildNotifyProjects)");
						writer.WriteBuildArgs(
							new Arg("ProjectName", project.Name),
							new Arg("BuildPath", "$(buildPath)"),
							new Arg("ServerNames", "Library|Website|Service|Application|Azure"),
							new Arg("ReferencesFolder", "references"));

						writer.WriteElementString("description", "Notify other projects");
					}

					// xxx temporarily copy release back to RUFRT-VXBUILD
					if (project.Name == "V3.Storage")
					{
						writer.CbTag(
							"CopyFiles",
							"from",
							project.WorkingDirectoryRelease() + @"\*",
							"to",
							String.Format(@"\\rufrt-vxbuild.cneu.cnwk\InternalReferences\{0}\Latest", project.Name));
					}
				}

				using (writer.OpenTag("publishers"))
				{
					WriteDefaultPublishers(writer, project);
					cleanup.Invoke();
				}
			}
		}

		private static void WriteWebsiteProject(XmlWriter writer, WebsiteProjectConfiguration project)
		{
			writer.Comment(String.Format("PROJECT: {0}", project.UniqueName));

			Action cleanup = () =>
			{
				WriteDefaultCleanup(writer, project);
				writer.CbTag("DeleteDirectory", "path", project.WorkingDirectoryPublish());
			};

			using (writer.OpenTag("project"))
			{
				WriteProjectHeader(writer, project);
				WriteSourceControl(writer, project);

				using (writer.OpenTag("prebuild"))
				{
					cleanup.Invoke();
				}

				using (writer.OpenTag("tasks"))
				{
					WriteCheckProject(writer, project);

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildSetupProject)");
						writer.WriteBuildArgs(
							new Arg("ProjectType", project.Type),
							new Arg("ProjectName", project.Name),
							new Arg("ProjectPath", project.WorkingDirectorySource),
							new Arg("TempPath", project.WorkingDirectoryTemp),
							new Arg("TfsPath", project.TfsPath),
							new Arg("CurrentVersion", "$[$CCNetLabel]"));

						writer.WriteElementString("description", "Setup project");
					}

					WriteSetupPackages(writer, project);
					WriteBuildProject(writer, project);

					using (writer.OpenTag("msbuild"))
					{
						writer.WriteElementString("executable", project.MsbuildExecutable);
						writer.WriteElementString("targets", "Build;_CopyWebApplication");
						writer.WriteElementString("workingDirectory", project.WorkingDirectorySource);
						writer.WriteElementString("buildArgs", String.Format(@"/noconsolelogger /p:Configuration=Release;OutDir={0}\", project.SourceDirectoryRelease));
						writer.WriteElementString("description", "Publish web site");
					}

					writer.CbTag("EraseXmlDocs", "path", project.SourceDirectoryPublished);
					writer.CbTag("EraseConfigFiles", "path", project.SourceDirectoryPublished);
					writer.CbTag("CompressDirectory", "path", project.SourceDirectoryPublished, "output", project.PublishFileLocal());

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "publish"),
							new Arg("LocalFile", project.PublishFileLocal()),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/{1}", project.UniqueName, project.PublishFileName())));

						writer.WriteElementString("description", "Publish release to blobs");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "publish"),
							new Arg("LocalFile", project.TempFileSource),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/source.txt", project.UniqueName)));

						writer.WriteElementString("description", "Publish source summary");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "publish"),
							new Arg("LocalFile", project.TempFilePackages),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/packages.txt", project.UniqueName)));

						writer.WriteElementString("description", "Publish packages summary");
					}
				}

				using (writer.OpenTag("publishers"))
				{
					WriteDefaultPublishers(writer, project);
					cleanup.Invoke();
				}
			}
		}

		private static void WriteServiceProject(XmlWriter writer, ServiceProjectConfiguration project)
		{
			writer.Comment(String.Format("PROJECT: {0}", project.UniqueName));

			Action cleanup = () =>
			{
				WriteDefaultCleanup(writer, project);
				writer.CbTag("DeleteDirectory", "path", project.WorkingDirectoryRelease());
			};

			using (writer.OpenTag("project"))
			{
				WriteProjectHeader(writer, project);
				WriteSourceControl(writer, project);

				using (writer.OpenTag("prebuild"))
				{
					cleanup.Invoke();
				}

				using (writer.OpenTag("tasks"))
				{
					WriteCheckProject(writer, project);

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildSetupProject)");
						writer.WriteBuildArgs(
							new Arg("ProjectType", project.Type),
							new Arg("ProjectName", project.Name),
							new Arg("ProjectPath", project.WorkingDirectorySource),
							new Arg("TempPath", project.WorkingDirectoryTemp),
							new Arg("TfsPath", project.TfsPath),
							new Arg("CurrentVersion", "$[$CCNetLabel]"));

						writer.WriteElementString("description", "Setup project");
					}

					WriteSetupPackages(writer, project);
					WriteBuildProject(writer, project);

					writer.CbTag("EraseXmlDocs", "path", project.SourceDirectoryRelease);
					writer.CbTag("EraseConfigFiles", "path", project.SourceDirectoryRelease);
					writer.CbTag("CompressDirectory", "path", project.SourceDirectoryRelease, "output", project.ReleaseFileLocal());

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "publish"),
							new Arg("LocalFile", project.ReleaseFileLocal()),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/{1}", project.UniqueName, project.ReleaseFileName())));

						writer.WriteElementString("description", "Publish release to blobs");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "publish"),
							new Arg("LocalFile", project.TempFileSource),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/source.txt", project.UniqueName)));

						writer.WriteElementString("description", "Publish source summary");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "publish"),
							new Arg("LocalFile", project.TempFilePackages),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/packages.txt", project.UniqueName)));

						writer.WriteElementString("description", "Publish packages summary");
					}
				}

				using (writer.OpenTag("publishers"))
				{
					WriteDefaultPublishers(writer, project);
					cleanup.Invoke();
				}
			}
		}

		private static void WriteApplicationProject(XmlWriter writer, ConsoleProjectConfiguration project)
		{
			writer.Comment(String.Format("PROJECT: {0}", project.UniqueName));

			Action cleanup = () =>
			{
				WriteDefaultCleanup(writer, project);
				writer.CbTag("DeleteDirectory", "path", project.WorkingDirectoryRelease());

				var windows = project as WindowsProjectConfiguration;
				if (windows != null)
				{
					writer.CbTag("DeleteDirectory", "path", windows.WorkingDirectoryPublish());
				}
			};

			using (writer.OpenTag("project"))
			{
				WriteProjectHeader(writer, project);
				WriteSourceControl(writer, project);

				using (writer.OpenTag("prebuild"))
				{
					cleanup.Invoke();
				}

				using (writer.OpenTag("tasks"))
				{
					WriteCheckProject(writer, project);

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildSetupProject)");
						writer.WriteBuildArgs(
							new Arg("ProjectType", project.Type),
							new Arg("ProjectName", project.Name),
							new Arg("ProjectPath", project.WorkingDirectorySource),
							new Arg("TempPath", project.WorkingDirectoryTemp),
							new Arg("TfsPath", project.TfsPath),
							new Arg("CurrentVersion", "$[$CCNetLabel]"));

						writer.WriteElementString("description", "Setup project");
					}

					WriteSetupPackages(writer, project);
					WriteBuildProject(writer, project);

					writer.CbTag("EraseXmlDocs", "path", project.SourceDirectoryRelease);
					writer.CbTag("EraseConfigFiles", "path", project.SourceDirectoryRelease);
					writer.CbTag("CompressDirectory", "path", project.SourceDirectoryRelease, "output", project.ReleaseFileLocal());

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "publish"),
							new Arg("LocalFile", project.ReleaseFileLocal()),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/{1}", project.UniqueName, project.ReleaseFileName())));

						writer.WriteElementString("description", "Publish release to blobs");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "publish"),
							new Arg("LocalFile", project.TempFileSource),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/source.txt", project.UniqueName)));

						writer.WriteElementString("description", "Publish source summary");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "publish"),
							new Arg("LocalFile", project.TempFilePackages),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/packages.txt", project.UniqueName)));

						writer.WriteElementString("description", "Publish packages summary");
					}
				}

				using (writer.OpenTag("publishers"))
				{
					WriteDefaultPublishers(writer, project);
					cleanup.Invoke();
				}
			}
		}

		private static void WriteCloudRoleProject(XmlWriter writer, CloudRoleProjectConfiguration project)
		{
			writer.Comment(String.Format("PROJECT: {0}", project.UniqueName));

			Action cleanup = () => WriteDefaultCleanup(writer, project);

			using (writer.OpenTag("project"))
			{
				WriteProjectHeader(writer, project);
				WriteSourceControl(writer, project);

				using (writer.OpenTag("prebuild"))
				{
					cleanup.Invoke();
				}

				using (writer.OpenTag("tasks"))
				{
					WriteCheckProject(writer, project);

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildSetupProject)");
						writer.WriteBuildArgs(
							new Arg("ProjectType", project.Type),
							new Arg("ProjectName", project.Name),
							new Arg("ProjectPath", project.WorkingDirectorySource),
							new Arg("TempPath", project.WorkingDirectoryTemp),
							new Arg("TfsPath", project.TfsPath),
							new Arg("CurrentVersion", "$[$CCNetLabel]"));

						writer.WriteElementString("description", "Setup project");
					}

					WriteSetupPackages(writer, project);
					WriteBuildProject(writer, project);

					writer.CbTag("AppendToFile", "file", project.TempFileExclude(), "text", "$tf");
					writer.CbTag("CompressDirectoryExclude", "path", project.WorkingDirectorySource, "output", project.TempFileSnapshot(), "exclude", project.TempFileExclude());

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "snapshot"),
							new Arg("LocalFile", project.TempFileSnapshot()),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/{1}", project.UniqueName, project.SnapshotFileName())));

						writer.WriteElementString("description", "Publish source snapshot");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "snapshot"),
							new Arg("LocalFile", project.TempFileSource),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/source.txt", project.UniqueName)));

						writer.WriteElementString("description", "Publish source summary");
					}

					using (writer.OpenTag("exec"))
					{
						writer.WriteElementString("executable", "$(ccnetBuildAzureUpload)");
						writer.WriteBuildArgs(
							new Arg("Storage", "Devbuild"),
							new Arg("Container", "snapshot"),
							new Arg("LocalFile", project.TempFilePackages),
							new Arg("BlobFile", String.Format("{0}/$[$CCNetLabel]/packages.txt", project.UniqueName)));

						writer.WriteElementString("description", "Publish packages summary");
					}
				}

				using (writer.OpenTag("publishers"))
				{
					WriteDefaultPublishers(writer, project);
					cleanup.Invoke();
				}
			}
		}

		private static void WriteDefaultCleanup(XmlWriter writer, ProjectConfiguration project)
		{
			writer.CbTag("DeleteDirectory", "path", project.WorkingDirectorySource);
			writer.CbTag("DeleteDirectory", "path", project.WorkingDirectoryPackages);
			writer.CbTag("DeleteDirectory", "path", project.WorkingDirectoryTemp);
		}

		private static void SaveProjectUid(string projectName, Guid projectUid)
		{
			var mapPath = Args.ProjectMap;
			mapPath.CreateDirectoryIfNotExists();

			var fileName = String.Format("{0}.txt", projectName);
			var filePath = Path.Combine(mapPath, fileName);

			File.WriteAllText(filePath, projectUid.ToString().ToUpperInvariant());
		}
	}
}
