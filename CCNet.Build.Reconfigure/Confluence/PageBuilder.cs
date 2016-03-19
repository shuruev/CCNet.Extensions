using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;
using CCNet.Build.Tfs;

namespace CCNet.Build.Reconfigure
{
	public class PageBuilder
	{
		private readonly ConfluenceClient m_confluence;
		private readonly TfsClient m_tfs;

		private Dictionary<long, List<PageSummary>> m_children;
		private List<IProjectPage> m_pages;

		public PageBuilder(ConfluenceClient confluence, TfsClient tfs)
		{
			if (confluence == null)
				throw new ArgumentNullException("confluence");

			if (tfs == null)
				throw new ArgumentNullException("tfs");

			m_confluence = confluence;
			m_tfs = tfs;
		}

		public void Rebuild(string spaceCode, string pageName)
		{
			Console.Write("Reading projects page and subtree... ");
			var root = m_confluence.GetPage(spaceCode, pageName);
			var tree = m_confluence.GetSubtree(root.Id);
			Console.WriteLine("OK");

			m_children = tree.GroupBy(p => p.ParentId).ToDictionary(g => g.Key, g => g.ToList());

			var result = new ConcurrentBag<IProjectPage>();

			var areas = m_children[root.Id];
			Parallel.ForEach(
				areas,
				new ParallelOptions { MaxDegreeOfParallelism = 2 },
				area =>
				{
					var pages = RebuildArea(area);
					foreach (var page in pages)
					{
						result.Add(page);
					}
				});

			var updated = UpdateSummaryPage(result, root);
			if (updated)
			{
				Console.WriteLine("Rebuild projects summary ... UPDATED");
			}
			else
			{
				Console.WriteLine("Rebuild projects summary ... not changed");
			}

			m_pages = result.ToList();
		}

		public List<ProjectConfiguration> ExportConfigurations()
		{
			return m_pages.SelectMany(p => p.ExportConfigurations()).ToList();
		}

		private bool UpdateSummaryPage(IEnumerable<IProjectPage> pages, Page existing)
		{
			var updated = new PageDocument();

			var tbody = new XElement(
				"tbody",
				new XElement(
					"tr",
					new XElement("th", "Area"),
					new XElement("th", "Project"),
					new XElement("th", ".NET"),
					new XElement("th", "Owner"),
					new XElement("th", "Status")));

			foreach (var page in pages.OrderBy(p => p.OrderKey))
			{
				tbody.Add(page.RenderSummaryRow(false));
			}

			updated.Root.Add(
				new XElement(
					"table",
					tbody));

			var content = updated.Render();

			var before = NormalizeForComparison(existing.Content);
			var after = NormalizeForComparison(content);

			if (after == before)
				return false;

			existing.Content = content;
			m_confluence.UpdatePage(existing);
			return true;
		}

		private List<IProjectPage> RebuildArea(PageSummary area)
		{
			var areaName = ResolveAreaName(area.Name);

			var knownAreas = new HashSet<string>
			{
				"Archive",
				"ContentCast",
				"DataSource",
				"Internal",
				"PartnerAccess",
				"Production",
				"Sandbox",
				"Vortex"
			};

			if (!knownAreas.Contains(areaName))
			{
				throw new InvalidOperationException(
					String.Format("Unknown area name '{0}'.", area.Name));
			}

			var result = new ConcurrentBag<IProjectPage>();

			var projects = m_children[area.Id];
			Parallel.ForEach(
				projects,
				new ParallelOptions { MaxDegreeOfParallelism = 5 },
				project =>
				{
					var page = RebuildProject(areaName, project);
					result.Add(page);
				});

			var updated = UpdateAreaPage(result, area.Id);
			if (updated)
			{
				Console.WriteLine("Rebuild [{0}] area summary ... UPDATED", areaName);
			}
			else
			{
				Console.WriteLine("Rebuild [{0}] area summary ... not changed", areaName);
			}

			return result.ToList();
		}

		private bool UpdateAreaPage(IEnumerable<IProjectPage> pages, long pageId)
		{
			var updated = new PageDocument();

			var tbody = new XElement(
				"tbody",
				new XElement(
					"tr",
					new XElement("th", "Project"),
					new XElement("th", ".NET"),
					new XElement("th", "Owner"),
					new XElement("th", "Status")));

			foreach (var page in pages.OrderBy(p => p.OrderKey))
			{
				tbody.Add(page.RenderSummaryRow(true));
			}

			updated.Root.Add(
				new XElement(
					"table",
					tbody));

			var content = updated.Render();
			var existing = m_confluence.GetPage(pageId);

			var before = NormalizeForComparison(existing.Content);
			var after = NormalizeForComparison(content);

			if (after == before)
				return false;

			existing.Content = content;
			m_confluence.UpdatePage(existing);
			return true;
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

		private IProjectPage RebuildProject(string areaName, PageSummary project)
		{
			Console.WriteLine("Processing page '{0} / {1}'...", areaName, project.Name);

			ProjectType projectType;
			var projectName = ResolveProjectName(project.Name, out projectType);

			var existing = m_confluence.GetPage(project.Id);
			var document = new PageDocument(existing.Content);

			IProjectPage page;
			try
			{
				switch (projectType)
				{
					case ProjectType.Library:
						page = new LibraryProjectPage(areaName, projectName, project.Name, document);
						break;

					case ProjectType.Website:
						page = new WebsiteProjectPage(areaName, projectName, project.Name, document);
						break;

					default:
						throw new InvalidOperationException(
							String.Format("Unknown how to process project of type '{0}'.", projectType.ToString().ToLowerInvariant()));
				}

				page.CheckPage(m_tfs);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(
					String.Format("An error occurred while processing page '{0}'.", project.Name),
					e);
			}

			var updated = UpdateProjectPage(page, existing);
			if (updated)
			{
				Console.WriteLine("[{0}] {1} #{2}... UPDATED", areaName, projectName, projectType.ToString().ToLowerInvariant());
			}
			else
			{
				Console.WriteLine("[{0}] {1} #{2}... not changed", areaName, projectName, projectType.ToString().ToLowerInvariant());
			}

			return page;
		}

		private bool UpdateProjectPage(IProjectPage page, Page existing)
		{
			var updated = page.RenderPage();
			var content = updated.Render();

			var before = NormalizeForComparison(existing.Content);
			var after = NormalizeForComparison(content);

			if (after == before)
				return false;

			existing.Content = content;
			m_confluence.UpdatePage(existing);
			return true;
		}

		private static string ResolveProjectName(string pageName, out ProjectType projectType)
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
					projectType = ProjectType.Library;
					break;

				case "web site":
					projectType = ProjectType.Website;
					break;

				default:
					throw new InvalidOperationException(
						String.Format("Unknown project type '{0}'.", type));
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
