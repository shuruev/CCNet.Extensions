using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCNet.Build.Common;
using NetBuild.Tfs;

namespace CCNet.Build.CheckProject
{
	public class CheckContext
	{
		private static readonly Guid s_webType = new Guid("349C5851-65DF-11DA-9384-00065B846F21");

		private readonly TfsClient m_tfs;

		public CheckContextService<List<string>> LocalFiles { get; }
		public CheckContextService<ProjectDocument> ProjectDocument { get; }
		public CheckContextService<Dictionary<string, string>> ProjectCommonProperties { get; }
		public CheckContextService<Dictionary<string, string>> ProjectDebugProperties { get; }
		public CheckContextService<Dictionary<string, string>> ProjectReleaseProperties { get; }
		public CheckContextService<bool> ProjectIsWeb { get; }
		public CheckContextService<List<ProjectFile>> ProjectFiles { get; }

		public CheckContextService<List<string>> TfsSolutionItems { get; }
		public CheckContextService<List<string>> TfsNugetItems { get; }
		public CheckContextService<List<string>> TfsPackagesItems { get; }
		public CheckContextService<string> TfsSolutionFile { get; }
		public CheckContextService<string> TfsNugetConfig { get; }

		public CheckContext(TfsClient tfs)
		{
			m_tfs = tfs;

			LocalFiles = new CheckContextService<List<string>>(GetLocalFiles);
			ProjectDocument = new CheckContextService<ProjectDocument>(GetProjectDocument);
			ProjectCommonProperties = new CheckContextService<Dictionary<string, string>>(GetProjectCommonProperties);
			ProjectDebugProperties = new CheckContextService<Dictionary<string, string>>(GetProjectDebugProperties);
			ProjectReleaseProperties = new CheckContextService<Dictionary<string, string>>(GetProjectReleaseProperties);
			ProjectIsWeb = new CheckContextService<bool>(GetProjectIsWeb);
			ProjectFiles = new CheckContextService<List<ProjectFile>>(GetProjectFiles);

			TfsSolutionItems = new CheckContextService<List<string>>(GetTfsSolutionItems);
			TfsNugetItems = new CheckContextService<List<string>>(GetTfsNugetItems);
			TfsPackagesItems = new CheckContextService<List<string>>(GetTfsPackagesItems);
			TfsSolutionFile = new CheckContextService<string>(GetTfsSolutionFile);
			TfsNugetConfig = new CheckContextService<string>(GetTfsNugetConfig);
		}

		public TfsClient Tfs => m_tfs;

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

		private bool GetProjectIsWeb()
		{
			return ProjectDocument.Result.GetProjectTypeGuids().Contains(s_webType);
		}

		private List<ProjectFile> GetProjectFiles()
		{
			Console.WriteLine("Getting project files...");
			return ProjectDocument.Result.GetProjectFiles();
		}

		private List<string> GetTfsSolutionItems()
		{
			Console.WriteLine("Getting TFS items from parent folder...");
			var items = m_tfs.GetChildItems(Paths.TfsSolutionPath)
				.Select(item => item.ServerPath)
				.Where(path => String.Compare(Paths.TfsSolutionPath, path, StringComparison.OrdinalIgnoreCase) != 0)
				.ToList();

			Console.WriteLine("Found {0} solution items.", items.Count);
			return items;
		}

		private List<string> GetTfsNugetItems()
		{
			Console.WriteLine("Getting TFS items from '.nuget' folder...");
			var items = m_tfs.GetChildItems(Paths.TfsNugetPath)
				.Select(item => item.ServerPath)
				.Where(path => String.Compare(Paths.TfsNugetPath, path, StringComparison.OrdinalIgnoreCase) != 0)
				.ToList();

			Console.WriteLine("Found {0} files.", items.Count);
			return items;
		}

		private List<string> GetTfsPackagesItems()
		{
			Console.WriteLine("Getting TFS items from 'packages' folder...");
			var items = m_tfs.GetChildItems(Paths.TfsPackagesPath)
				.Select(item => item.ServerPath)
				.Where(path => String.Compare(Paths.TfsPackagesPath, path, StringComparison.OrdinalIgnoreCase) != 0)
				.ToList();

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
