using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;
using CCNet.Build.Tfs;

namespace CCNet.Build.Reconfigure
{
	public abstract class BasicProjectPage : ProjectPage
	{
		public abstract ProjectType Type { get; }

		public string TfsPath { get; set; }
		public Guid ProjectUid { get; set; }

		public XElement Stats { get; set; }
		public XElement History { get; set; }
		public XElement About { get; set; }

		protected BasicProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument, BuildOwners buildOwners)
			: base(areaName, projectName, pageName, pageDocument, buildOwners)
		{
			TfsPath = ParseTfsPath(m_properties);

			Stats = ParseStats(m_root);
			History = ParseHistory(m_root);
			About = ParseAbout(m_root);
		}

		public virtual string ProjectFile
		{
			get
			{
				// we use TFS folder name here instead of ProjectName due to CnetContent.* exception
				var folderName = Path.GetFileName(TfsPath);
				var fileName = $"{folderName}.csproj";
				return $"{TfsPath}/{fileName}";
			}
		}

		private string ParseTfsPath(Dictionary<string, string> properties)
		{
			var path = FindByKey(properties, key => key.Contains("tfs") || key.Contains("path"));

			if (path == null)
				throw new InvalidOperationException("Cannot find TFS path.");

			if (path != path.AsciiOnly('$', '/', '.').CleanWhitespaces())
				throw new ArgumentException($"TFS path '{path}' does not look well-formed.");

			return path.TrimEnd('/');
		}

		private List<XElement> ParseSections(XElement page)
		{
			return page.XElements("ac:structured-macro[@ac:name='section']").ToList();
		}

		private List<XElement> ParseColumns(XElement section)
		{
			return section.XElements("ac:rich-text-body/ac:structured-macro[@ac:name='column']").ToList();
		}

		private XElement ParseFromDetails(XElement page, int index)
		{
			var sections = ParseSections(page);
			if (sections.Count != 2)
				return null;

			var details = sections[0];
			var columns = ParseColumns(details);
			if (columns.Count != 3)
				return null;

			var stats = columns[index].XElements("ac:rich-text-body").FirstOrDefault();
			if (stats == null)
				return null;

			return stats;
		}

		private XElement ParseStats(XElement page)
		{
			return ParseFromDetails(page, 0);
		}

		private XElement ParseHistory(XElement page)
		{
			return ParseFromDetails(page, 2);
		}

		private XElement ParseAbout(XElement page)
		{
			var sections = ParseSections(page);
			if (sections.Count != 2)
				return null;

			var about = sections[1].XElements("ac:rich-text-body").FirstOrDefault();
			if (about == null)
				return null;

			return about;
		}

		private XElement RenderTfsPath()
		{
			if (ProjectUid == Guid.Empty)
				return new XElement("td", new XElement("code").XValue(TfsPath));

			return new XElement(
				"td",
				new XElement("code").XValue(TfsPath),
				new XElement("br"),
				new XElement(
					"sup",
					new XElement(
						"sub",
						new XElement(
							"code",
							new XElement(
								"span",
								new XAttribute("style", PageDocument.Style.Color.Gray),
								ProjectUid.ToString().ToUpperInvariant())))));
		}

		private XElement RenderLinks()
		{
			var content = new List<object>();

			var title = new XElement("p", new XElement("code", new XElement("u", "ʟɪɴᴋs")));
			content.Add(title);

			if (GetType() == typeof(LibraryProjectPage))
			{
				var nuget = new XElement(
					"p",
					new XElement(
						"a",
						ProjectBranch == null
							? new XAttribute("href", $"{Config.NuGetUrl}/packages/{ProjectName}/")
							: new XAttribute("href", $"{Config.NuGetUrl}/private/{ProjectBranch}/packages/{ProjectName}/"),
						PageDocument.BuildImage($"{Config.NuGetUrl}/favicon.ico"),
						"$nbsp$NuGet package"));

				content.Add(nuget);
			}

			var build = new XElement(
				"p",
				new XElement(
					"a",
					new XAttribute(
						"href",
						ProjectBranch == null
							? $"{Config.CCNetUrl}/server/{Type.ServerName()}/project/{ProjectName}/ViewProjectReport.aspx"
							: $"{Config.CCNetUrl}/server/{Type.ServerName()}/project/{ProjectName}-{ProjectBranch}/ViewProjectReport.aspx"),
					PageDocument.BuildImage($"{Config.CCNetUrl}/favicon.ico"),
					"$nbsp$Build project"));

			content.Add(build);

			return PageDocument.BuildBody(content);
		}

		private XElement RenderStats()
		{
			var title = new XElement("p", new XElement("code", new XElement("u", "sᴛᴀᴛs")));

			if (Stats == null || !Stats.Elements().Any())
			{
				return PageDocument.BuildBody(
					title,
					BuildNotAvailable());
			}

			Stats.Elements().First().Remove();
			Stats.AddFirst(title);

			return Stats;
		}

		private XElement RenderHistory()
		{
			var title = new XElement("p", new XElement("code", new XElement("u", "ʜɪsᴛᴏʀʏ")));

			if (History == null || !History.Elements().Any())
			{
				return PageDocument.BuildBody(
					title,
					BuildNotAvailable());
			}

			History.Elements().First().Remove();
			History.AddFirst(title);

			return History;
		}

		private XElement RenderDetails()
		{
			return PageDocument.BuildSection(
				PageDocument.BuildBody(
					PageDocument.BuildColumn("300px", RenderStats()),
					PageDocument.BuildColumn("200px", RenderLinks()),
					PageDocument.BuildColumn(null, RenderHistory())));
		}

		private XElement RenderAbout()
		{
			if (About == null)
			{
				About = PageDocument.BuildBody(new XElement("p", "..."));
			}

			return PageDocument.BuildSection(About);
		}

		private XElement RenderMore()
		{
			return PageDocument.BuildInfo(
				PageDocument.BuildBody(
					new XElement(
						"p",
						"Знаешь об этом компоненте что-то еще? Пожалуйста напиши! (в произвольной форме, после заголовка About)",
						new XElement("br"),
						"Например цели создания, какие функции выполняет, может быть какие-то неочевидные особенности или решения по дизайну или структуре классов, и$nbsp$т.$nbsp$п.")));
		}

		public override void CheckPage(TfsClient client)
		{
			base.CheckPage(client);

			var path = TfsPath;

			if (ProjectBranch == null && !CheckTfsPathArea(path, AreaName))
			{
				throw new InvalidOperationException($"TFS path '{path}' seems not conforming with area name '{AreaName}'.");
			}

			if (!CheckTfsPathProject(path, ProjectName))
				throw new InvalidOperationException($"TFS path '{path}' seems not conforming with project name '{ProjectName}'.");

			var project = new ProjectDocument(() => client.ReadFile(ProjectFile));

			ProjectUid = project.GetProjectGuid();
		}

		private static bool CheckTfsPathArea(string tfsPath, string areaName)
		{
			return tfsPath.Contains($"/{areaName}/");
		}

		private static bool CheckTfsPathProject(string tfsPath, string projectName)
		{
			if (tfsPath.EndsWith($"/{projectName}"))
				return true;

			const string prefix = "CnetContent.";
			if (projectName.StartsWith(prefix))
			{
				var custom = projectName.Substring(prefix.Length);
				if (tfsPath.EndsWith($"/{custom}"))
					return true;
			}

			return false;
		}

		public override PageDocument RenderPage()
		{
			var page = base.RenderPage();

			var addBefore = page.Root.XElement("table/tbody/tr/th[text()='Owner']").Parent;
			addBefore.AddBeforeSelf(new XElement("tr", new XElement("th", "TFS path"), RenderTfsPath()));

			var details = RenderDetails();
			var h2 = new XElement("h2", "About");
			var about = RenderAbout();
			var h4 = new XElement("h4", "Please contribute...");
			var more = RenderMore();

			page.Root.Add(details);
			page.Root.Add(h2);
			page.Root.Add(about);
			page.Root.Add(h4);
			page.Root.Add(more);

			return page;
		}

		public override Tuple<string, Guid> ExportMap()
		{
			if (ProjectUid == Guid.Empty)
				return null;

			if (ProjectBranch == null)
			{
				return new Tuple<string, Guid>(ProjectName, ProjectUid);
			}

			return new Tuple<string, Guid>(
					string.Format("{0}-{1}", ProjectName, ProjectBranch),
					ProjectUid);
		}

		protected new void ApplyTo(ProjectConfiguration config)
		{
			base.ApplyTo(config);

			config.TfsPath = TfsPath;
		}
	}
}
