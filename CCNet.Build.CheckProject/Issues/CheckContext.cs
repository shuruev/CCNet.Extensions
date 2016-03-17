using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCNet.Build.Common;
using CCNet.Build.Tfs;

namespace CCNet.Build.CheckProject
{
	public class CheckContext
	{
		private readonly TfsClient m_tfs;

		public CheckContextService<List<string>> LocalFiles { get; private set; }
		public CheckContextService<ProjectDocument> ProjectDocument { get; private set; }
		public CheckContextService<Dictionary<string, string>> ProjectCommonProperties { get; private set; }
		public CheckContextService<Dictionary<string, string>> ProjectDebugProperties { get; private set; }
		public CheckContextService<Dictionary<string, string>> ProjectReleaseProperties { get; private set; }
		public CheckContextService<List<ProjectFile>> ProjectFiles { get; private set; }

		public CheckContextService<List<string>> TfsSolutionItems { get; private set; }
		public CheckContextService<List<string>> TfsNugetItems { get; private set; }
		public CheckContextService<List<string>> TfsPackagesItems { get; private set; }
		public CheckContextService<string> TfsSolutionFile { get; private set; }
		public CheckContextService<string> TfsNugetConfig { get; private set; }

		public CheckContext(TfsClient tfs)
		{
			m_tfs = tfs;

			LocalFiles = new CheckContextService<List<string>>(GetLocalFiles);
			ProjectDocument = new CheckContextService<ProjectDocument>(GetProjectDocument);
			ProjectCommonProperties = new CheckContextService<Dictionary<string, string>>(GetProjectCommonProperties);
			ProjectDebugProperties = new CheckContextService<Dictionary<string, string>>(GetProjectDebugProperties);
			ProjectReleaseProperties = new CheckContextService<Dictionary<string, string>>(GetProjectReleaseProperties);
			ProjectFiles = new CheckContextService<List<ProjectFile>>(GetProjectFiles);

			TfsSolutionItems = new CheckContextService<List<string>>(GetTfsSolutionItems);
			TfsNugetItems = new CheckContextService<List<string>>(GetTfsNugetItems);
			TfsPackagesItems = new CheckContextService<List<string>>(GetTfsPackagesItems);
			TfsSolutionFile = new CheckContextService<string>(GetTfsSolutionFile);
			TfsNugetConfig = new CheckContextService<string>(GetTfsNugetConfig);
		}

		private List<string> GetLocalFiles()
		{
			Console.WriteLine("Getting local files from project folder...");
			var files = Directory.GetFiles(Args.ProjectPath, "*", SearchOption.AllDirectories)
				.Select(item => item.Replace(Args.ProjectPath + '\\', String.Empty))
				.ToList();

			Console.WriteLine("Found {0} files in project folder.", files.Count);
			return files;
		}

		private ProjectDocument GetProjectDocument()
		{
			Console.WriteLine("Loading project...");
			var project = new ProjectDocument(Paths.ProjectFile);
			project.Load();

			return project;
		}

		private Dictionary<string, string> GetProjectCommonProperties()
		{
			Console.WriteLine("Getting project common properties...");
			return ProjectDocument.Result.GetCommonProperties();
		}

		private Dictionary<string, string> GetProjectDebugProperties()
		{
			Console.WriteLine("Getting project debug properties...");
			return ProjectDocument.Result.GetDebugProperties();
		}

		private Dictionary<string, string> GetProjectReleaseProperties()
		{
			Console.WriteLine("Getting project release properties...");
			return ProjectDocument.Result.GetReleaseProperties();
		}

		private List<ProjectFile> GetProjectFiles()
		{
			Console.WriteLine("Getting project files...");
			return ProjectDocument.Result.GetProjectFiles();
		}

		private List<string> GetTfsSolutionItems()
		{
			Console.WriteLine("Getting TFS items from parent folder...");
			var items = m_tfs.GetChildItems(Paths.TfsSolutionPath);

			Console.WriteLine("Found {0} solution items.", items.Count);
			return items;
		}

		private List<string> GetTfsNugetItems()
		{
			Console.WriteLine("Getting TFS items from '.nuget' folder...");
			var items = m_tfs.GetChildItems(Paths.TfsNugetPath);

			Console.WriteLine("Found {0} files.", items.Count);
			return items;
		}

		private List<string> GetTfsPackagesItems()
		{
			Console.WriteLine("Getting TFS items from 'packages' folder...");
			var items = m_tfs.GetChildItems(Paths.TfsPackagesPath);

			Console.WriteLine("Found {0} files.", items.Count);
			return items;
		}

		private string GetTfsSolutionFile()
		{
			Console.WriteLine("Reading solution file from TFS...");
			return m_tfs.ReadFile(Paths.TfsSolutionFile);
		}

		private string GetTfsNugetConfig()
		{
			Console.WriteLine("Reading nuget.config file from TFS...");
			return m_tfs.ReadFile(Paths.TfsNugetConfig);
		}
	}
}
