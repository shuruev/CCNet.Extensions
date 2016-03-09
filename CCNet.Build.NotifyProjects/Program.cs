using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCNet.Build.Common;
using QuickGraph;

namespace CCNet.Build.NotifyProjects
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Execute.DisplayUsage("Notifies other projects that current project was successfully built.", typeof(Args));
				return 0;
			}

			try
			{
				Args.Current = new ArgumentProperties(args);
				Execute.DisplayCurrent(typeof(Args));

				NotifyProjects();
			}
			catch (Exception e)
			{
				return Execute.RuntimeError(e);
			}

			return 0;
		}

		private static void NotifyProjects()
		{
			Console.Write("Building references graph... ");
			var graph = BuildReferencesGraph();
			Console.WriteLine("OK");

			Console.Write("Removing unnecessary edges... ");
			var cleared = graph.Clone();
			GraphHelper.RemoveExplicitEdges(cleared);
			Console.WriteLine("OK");

			IEnumerable<Edge<string>> outEdges;

			var projectsToReport = new List<string>();
			if (graph.TryGetOutEdges(Args.ProjectName, out outEdges))
			{
				projectsToReport.AddRange(outEdges.Select(edge => edge.Target));
			}

			foreach (var project in projectsToReport.OrderBy(i => i))
			{
				Console.WriteLine(
					"[USAGE] {0} | {1}",
					project,
					Args.ServerName);
			}

			var projectsToNotify = new List<string>();
			if (cleared.TryGetOutEdges(Args.ProjectName, out outEdges))
			{
				projectsToNotify.AddRange(outEdges.Select(edge => edge.Target));
			}

			Console.Write("Notified dependant projects... ");
			foreach (var project in projectsToNotify)
			{
				NotifyProject(project);
			}

			Console.WriteLine("OK");
		}

		private static AdjacencyGraph<string, Edge<string>> BuildReferencesGraph()
		{
			var graph = new AdjacencyGraph<string, Edge<string>>();

			foreach (string projectFolder in Directory.GetDirectories(Args.ProjectsPath))
			{
				var projectName = Path.GetFileName(projectFolder);
				var referencesFolder = Path.Combine(projectFolder, Args.ReferencesFolder);

				if (!Directory.Exists(referencesFolder))
					continue;

				var referenceFiles = Directory.GetFiles(referencesFolder).Select(Path.GetFileNameWithoutExtension);

				graph.AddVerticesAndEdgeRange(referenceFiles.Select(referenceName => new Edge<string>(referenceName, projectName)));
			}

			return graph;
		}

		private static void NotifyProject(string projectName)
		{
			var projectFolder = Path.Combine(Args.ProjectsPath, projectName);
			var referencesFolder = Path.Combine(projectFolder, Args.ReferencesFolder);

			var fileName = String.Format("{0}.txt", Args.ProjectName);
			var filePath = Path.Combine(referencesFolder, fileName);

			File.WriteAllText(filePath, String.Format("Updated on {0}", DateTime.Now.ToDetailedString()));
		}
	}
}
