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

		private Dictionary<long, List<PageSummary>> m_children;

		public PageBuilder(ConfluenceClient client)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			m_client = client;
		}

		public void Rebuild(string spaceCode, string pageName)
		{
			Console.Write("Reading projects page and subtree... ");
			var root = m_client.GetPage(spaceCode, pageName);
			var tree = m_client.GetSubtree(root.Id);
			Console.WriteLine("OK");

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

		private void RebuildProject(string areaName, PageSummary project)
		{
			Console.WriteLine("Processing page '{0} / {1}'...", areaName, project.Name);

			PageType pageType;
			var projectName = ResolveProjectName(project.Name, out pageType);

			var page = m_client.GetPage(project.Id);
			var document = new PageDocument(page.Content);

			IPageBuilder builder;
			try
			{
				switch (pageType)
				{
					case PageType.Library:
						builder = new ProjectPage<LibraryProjectProperties>(projectName, document);
						break;

					default:
						throw new InvalidOperationException(
							String.Format("Unknown how to process pages of type '{0}'.", pageType));
				}
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(
					String.Format("An error occurred while processing page '{0}'.", project.Name),
					e);
			}

			var updated = builder.BuildPage();
			var content = updated.Render();

			var before = NormalizeForComparison(page.Content);
			var after = NormalizeForComparison(content);

			if (after == before)
			{
				Console.WriteLine("[{0}] {1} #{2}... not changed", areaName, projectName, pageType.ToString().ToLowerInvariant());
			}
			else
			{
				page.Content = content;
				m_client.UpdatePage(page);
				Console.WriteLine("[{0}] {1} #{2}... UPDATED", areaName, projectName, pageType.ToString().ToLowerInvariant());
			}
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

		private string NormalizeForComparison(string content)
		{
			var doc = new PageDocument(content);
			var macros = doc.Root.XElements("//ac:structured-macro").ToList();

			foreach (var macro in macros)
			{
				macro.XAttribute("ac:macro-id").RemoveIfExists();
				macro.XAttribute("ac:schema-version").RemoveIfExists();
			}

			return doc.Render();
		}
	}
}
