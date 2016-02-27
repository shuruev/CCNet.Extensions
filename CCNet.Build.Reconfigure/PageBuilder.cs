using System;
using System.Collections.Generic;
using System.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class PageBuilder
	{
		private readonly ConfluenceClient m_client;

		private Dictionary<long, PageSummary> m_summaries;
		private Dictionary<long, List<PageSummary>> m_children;

		public PageBuilder(ConfluenceClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			m_client = client;
		}

		public void Rebuild(string spaceCode, string pageName)
		{
			var root = m_client.GetPage(spaceCode, pageName);
			var tree = m_client.GetSubtree(root.Id);

			m_summaries = tree.ToDictionary(p => p.Id);
			m_children = tree.GroupBy(p => p.ParentId).ToDictionary(g => g.Key, g => g.ToList());

			var areas = m_children[root.Id];
			foreach (var area in areas.AsParallel())
			{
				RebuildArea(area);
			}

			Console.WriteLine("Done.");
			Console.ReadKey();
		}

		private void RebuildArea(PageSummary area)
		{
			var areaName = ResolveAreaName(area.Name);

			var knownAreas = new HashSet<string>
			{
				"Archive",
				"ContentCast",
				"Internal",
				"Sandbox",
				"Vortex"
			};

			if (!knownAreas.Contains(areaName))
			{
				throw new InvalidOperationException(
					String.Format("Unknown area name '{0}'.", area.Name));
			}

			var projects = m_children[area.Id];
			foreach (var project in projects.AsParallel())
			{
				RebuildProject(areaName, project);
			}
		}

		private void RebuildProject(string areaName, PageSummary project)
		{
			Console.WriteLine("BEGIN processing {0}...", project.Name);

			PageType pageType;
			var projectName = ResolveProjectName(project.Name, out pageType);

			var full = m_client.GetPage(project.Id);

			var doc = new PageDocument(full.Content);

			var table = doc.SelectElement("/page/table");
			var section = doc.SelectElements("/page/ac:structured-macro[@ac:name='section']").ToList();

			var b = doc.Render().Replace("2.1", "2.2");

			m_client.UpdatePage(project.Id, b);
			Console.WriteLine("END processing {0}", project.Name);
		}

		private static string ResolveAreaName(string pageName)
		{
			if (pageName != pageName.AsciiOnly(' ').CleanWhitespaces())
				throw new ArgumentException(
					String.Format("Area name '{0}' does not look well-formed.", pageName));

			var parts = pageName.Split(new[] { ' ' }, 2);
			if (parts.Length != 2)
				throw new ArgumentException(
					String.Format("Page name '{0}' does not look well-formed.", pageName));

			var name = parts[0];
			var type = parts[1];

			if (type != "area")
				throw new InvalidOperationException(
					String.Format("Invalid area name '{0}'.", pageName));

			return name;
		}

		private static string ResolveProjectName(string pageName, out PageType pageType)
		{
			if (pageName != pageName.AsciiOnly('.', ' ').CleanWhitespaces())
				throw new ArgumentException(
					String.Format("Page name '{0}' does not look well-formed.", pageName));

			var parts = pageName.Split(new[] { ' ' }, 2);
			if (parts.Length != 2)
				throw new ArgumentException(
					String.Format("Page name '{0}' does not look well-formed.", pageName));

			var name = parts[0];
			var type = parts[1];

			switch (type)
			{
				case "library":
					pageType = PageType.Library;
					break;

				default:
					throw new InvalidOperationException(
						String.Format("Unknown page type '{0}'.", type));
			}

			return name;
		}
	}
}
