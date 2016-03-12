using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCNet.Build.Tfs;

namespace CCNet.Build.CheckProject
{
	public class CheckContext
	{
		private readonly TfsClient m_tfs;

		public CheckContextService<List<string>> LocalFiles { get; private set; }
		public CheckContextService<List<string>> TfsSolutionItems { get; private set; }
		public CheckContextService<List<string>> TfsNugetItems { get; private set; }
		public CheckContextService<string> TfsSolutionFile { get; private set; }
		public CheckContextService<string> TfsNugetConfig { get; private set; }

		public CheckContext(TfsClient tfs)
		{
			m_tfs = tfs;

			LocalFiles = new CheckContextService<List<string>>(GetLocalFiles);
			TfsSolutionItems = new CheckContextService<List<string>>(GetTfsSolutionItems);
			TfsNugetItems = new CheckContextService<List<string>>(GetTfsNugetItems);
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
