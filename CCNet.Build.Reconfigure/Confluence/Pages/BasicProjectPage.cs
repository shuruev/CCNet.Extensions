using System;
using System.Collections.Generic;
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

		protected BasicProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument)
			: base(areaName, projectName, pageName, pageDocument)
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
				var fileName = String.Format("{0}.csproj", ProjectName);
				return String.Format("{0}/{1}", TfsPath, fileName);
			}
		}

		private string ParseTfsPath(Dictionary<string, string> properties)
		{
			var path = FindByKey(properties, key => key.Contains("tfs") || key.Contains("path"));

			if (path == null)
				throw new InvalidOperationException("Cannot find TFS path.");

			if (path != path.AsciiOnly('$', '/', '.').CleanWhitespaces())
				throw new ArgumentException(
					String.Format("TFS path '{0}' does not look well-formed.", path));

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
								new XAttribute("style", "color: rgb(153,153,153);"),
								ProjectUid.ToString().ToUpperInvariant())))));
		}

		private XElement RenderLinks()
		{
			var title = new XElement("p", new XElement("code", new XElement("u", "ʟɪɴᴋs")));

			return PageDocument.BuildBody(
				title,
				new XElement(
					"p",
					new XElement(
						"a",
						new XAttribute("href", String.Format("{0}/packages/{1}/", Config.NuGetUrl, ProjectName)),
						PageDocument.BuildImage(String.Format("{0}/favicon.ico", Config.NuGetUrl)),
						"$nbsp$NuGet package")),
				new XElement(
					"p",
					new XElement(
						"a",
						new XAttribute("href", String.Format("{0}/server/{1}/project/{2}/ViewProjectReport.aspx", Config.CCNetUrl, Type, ProjectName)),
						PageDocument.BuildImage(String.Format("{0}/favicon.ico", Config.CCNetUrl)),
						"$nbsp$Build project")));
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

			if (!path.Contains(String.Format("/{0}/", AreaName)))
				throw new InvalidOperationException(
					String.Format("TFS path '{0}' seems not conforming with area name '{1}'.", path, AreaName));

			if (!path.EndsWith(String.Format("/{0}", ProjectName)))
				throw new InvalidOperationException(
					String.Format("TFS path '{0}' seems not conforming with project name '{1}'.", path, ProjectName));

			var xml = client.ReadFile(ProjectFile);
			var project = new ProjectDocument();
			project.Load(xml);

			ProjectUid = project.GetProjectGuid();
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
	}
}
