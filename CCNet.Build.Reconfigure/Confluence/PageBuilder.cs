using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;
using CCNet.Build.Tfs;
using System.Text;

namespace CCNet.Build.Reconfigure
{
	public class PageBuilder
	{
		private readonly CachedConfluenceClient m_confluence;
		private readonly TfsClient m_tfs;
		private readonly BuildOwners m_owners;

		private Dictionary<long, List<PageSummary>> m_children;
		private List<IProjectPage> m_pages;

		public PageBuilder(CachedConfluenceClient confluence, TfsClient tfs, BuildOwners owners)
		{
			if (confluence == null)
				throw new ArgumentNullException(nameof(confluence));

			if (tfs == null)
				throw new ArgumentNullException(nameof(tfs));

			if (owners == null)
				throw new ArgumentNullException(nameof(owners));

			m_confluence = confluence;
			m_tfs = tfs;
			m_owners = owners;
		}

		public void Rebuild(string spaceCode, string pageName)
		{
			Console.Write("Reading projects page and subtree... ");
			var root = m_confluence.GetPageSummary(spaceCode, pageName);
			var tree = m_confluence.GetSubtree(root.Id);
			Console.WriteLine("OK");

			m_children = tree.GroupBy(p => p.ParentId).ToDictionary(g => g.Key, g => g.ToList());

			var result = new ConcurrentBag<IProjectPage>();

			var areas = m_children[root.Id];
			Parallel.ForEach(
				areas,
				new ParallelOptions { MaxDegreeOfParallelism = 5 },
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

		public List<IProjectConfigurationTemp> ExportConfigurations()
		{
			return m_pages
				.SelectMany(p => p.ExportConfigurations())
				.ToList();
		}

		public Dictionary<string, Guid> ExportMap()
		{
			var map = m_pages
				.Select(p => p.ExportMap())
				.Where(i => i != null)
				.ToDictionary(i => i.Item1, i => i.Item2);

			var dups = map.GroupBy(i => i.Value).Where(g => g.Count() > 1).ToList();
			if (dups.Count > 0)
			{
				StringBuilder sb = new StringBuilder();

				foreach (var dup in dups)
				{
					var projectNames = dup.Select(i => i.Key);
					var shortName = projectNames.OrderBy(i => i.Length).First();

					if (projectNames.Where(p => !p.StartsWith(shortName + "-")).All(p => p == shortName))
					{
						continue;
					}

					var list = string.Join(", ", dup.Select(i => "'" + i.Key + "'"));
					sb.AppendLine($"Project UID = {dup.Key.ToString("B").ToUpper()} seems not unique and belongs to projects {list}.");
				}

				if (sb.Length > 0)
				{
					throw new InvalidOperationException(sb.ToString());
				}
			}

			return map;
		}

		private bool UpdateSummaryPage(IEnumerable<IProjectPage> pages, PageSummary summary)
		{
			var updated = new PageDocument();

			var tbody = new XElement(
				"tbody",
				new XElement(
					"tr",
					new XElement("th", "Area"),
					new XElement("th", "Project"),
					new XElement("th", "Type"),
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
			var existing = m_confluence.GetCachedPage(summary);

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
				"GuidedSelling",
				"Internal",
				"PartnerAccess",
				"Platform",
				"Production",
				"Sandbox",
				"Vortex"
			};

			if (!knownAreas.Contains(areaName))
				throw new InvalidOperationException($"Unknown area name '{area.Name}'.");

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

			var updated = UpdateAreaPage(result, area);
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

		private bool UpdateAreaPage(IEnumerable<IProjectPage> pages, PageSummary area)
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
			var existing = m_confluence.GetCachedPage(area);

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
				throw new ArgumentException($"Area name '{pageName}' does not look well-formed.");

			var parts = pageName.Split(new[] { ' ' }, 2);
			if (parts.Length != 2)
				throw new ArgumentException($"Page name '{pageName}' does not look well-formed.");

			var name = parts[0];
			var type = parts[1];

			if (type != "area")
				throw new InvalidOperationException($"Invalid area name '{pageName}'.");

			return name;
		}

		private IProjectPage RebuildProject(string areaName, PageSummary project)
		{
			Console.WriteLine("Processing page '{0} / {1}'...", areaName, project.Name);

			IProjectPage page;
			try
			{
				ProjectType projectType;
				var projectName = ResolveProjectName(project.Name, out projectType);

				var existing = m_confluence.GetCachedPage(project);
				var document = new PageDocument(existing.Content);

				switch (projectType)
				{
					case ProjectType.Library:
						page = new LibraryProjectPage(areaName, projectName, project.Name, document, m_owners);
						break;

					case ProjectType.Website:
						page = new WebsiteProjectPage(areaName, projectName, project.Name, document, m_owners);
						break;

					case ProjectType.Webservice:
						page = new WebserviceProjectPage(areaName, projectName, project.Name, document, m_owners);
						break;

					case ProjectType.Service:
						page = new ServiceProjectPage(areaName, projectName, project.Name, document, m_owners);
						break;

					case ProjectType.Console:
						page = new ConsoleProjectPage(areaName, projectName, project.Name, document, m_owners);
						break;

					case ProjectType.Windows:
						page = new WindowsProjectPage(areaName, projectName, project.Name, document, m_owners);
						break;

					case ProjectType.CloudRole:
						page = new CloudRoleProjectPage(areaName, projectName, project.Name, document, m_owners);
						break;

					case ProjectType.CloudService:
						page = new CloudServiceProjectPage(areaName, projectName, project.Name, document, m_owners);
						break;

					case ProjectType.FabricService:
						page = new FabricServiceProjectPage(areaName, projectName, project.Name, document, m_owners);
						break;

					case ProjectType.FabricApplication:
						page = new FabricApplicationProjectPage(areaName, projectName, project.Name, document, m_owners);
						break;

					default:
						throw new InvalidOperationException(
							$"Unknown how to process project of type '{projectType.ToString().ToLowerInvariant()}'.");
				}

				page.CheckPage(m_tfs);

				var updated = UpdateProjectPage(page, existing);
				if (updated)
				{
					Console.WriteLine("[{0}] {1} #{2}... UPDATED", areaName, projectName, projectType.ToString().ToLowerInvariant());
				}
				else
				{
					Console.WriteLine("[{0}] {1} #{2}... not changed", areaName, projectName, projectType.ToString().ToLowerInvariant());
				}
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(
					$"An error occurred while processing page '{project.Name}' version {project.Version}.",
					e);
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
			if (pageName.StartsWith("~"))
			{
				var pair = pageName.Split(new[] { "~" }, StringSplitOptions.None);
				if (pair.Length != 3
					|| !pair[1].StartsWith(" ")
					|| !pair[1].EndsWith(" ")
					|| !pair[2].StartsWith(" "))
				{
					throw new ArgumentException($"Page name '{pageName}' does not look well-formed.");
				}

				pageName = pair[2].TrimStart();
			}

			if (pageName != pageName.AsciiOnly('.', ' ').CleanWhitespaces())
				throw new ArgumentException($"Page name '{pageName}' does not look well-formed.");

			var parts = pageName.Split(new[] { ' ' }, 2);
			if (parts.Length != 2)
				throw new ArgumentException($"Page name '{pageName}' does not look well-formed.");

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

				case "web service":
					projectType = ProjectType.Webservice;
					break;

				case "service":
					projectType = ProjectType.Service;
					break;

				case "console":
					projectType = ProjectType.Console;
					break;

				case "application":
					projectType = ProjectType.Windows;
					break;

				case "cloud role":
					projectType = ProjectType.CloudRole;
					break;

				case "cloud service":
					projectType = ProjectType.CloudService;
					break;

				case "fabric service":
					projectType = ProjectType.FabricService;
					break;

				case "fabric application":
					projectType = ProjectType.FabricApplication;
					break;

				default:
					throw new InvalidOperationException($"Unknown project type '{type}'.");
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

			return doc.Render()
				.CleanWhitespaces()
				.Replace("<table class=\"wrapped\"><colgroup><col /><col /></colgroup>", "<table>");
		}
	}
}
