using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class CloudServiceProjectPage : BasicProjectPage
	{
		public string Title { get; set; }

		public CloudServiceProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument, BuildOwners buildOwners)
			: base(areaName, projectName, pageName, pageDocument, buildOwners)
		{
			Title = ParseTitle(m_properties);
		}

		public override ProjectType Type => ProjectType.CloudService;

		public override string ProjectFile
		{
			get
			{
				// we use TFS folder name here instead of ProjectName due to CnetContent.* exception
				var folderName = Path.GetFileName(TfsPath);
				var fileName = $"{folderName}.ccproj";
				return $"{TfsPath}/{fileName}";
			}
		}

		private string ParseTitle(Dictionary<string, string> properties)
		{
			var title = FindByKey(properties, key => key.Contains("titl"));

			if (title == null)
				throw new InvalidOperationException("Cannot find project title.");

			var norm = title.AsciiOnly(' ', '-').CleanWhitespaces();
			if (norm.Length < 10)
				throw new InvalidOperationException("Something is wrong with project title.");

			if (title != norm)
				throw new ArgumentException($"Project title '{title}' does not look well-formed.");

			return title;
		}

		private XElement RenderTitle()
		{
			return new XElement("td", PageDocument.BuildPageLink(Title));
		}

		public override PageDocument RenderPage()
		{
			var page = base.RenderPage();

			var addBefore = page.Root.XElement("table/tbody/tr/th[text()='Description']").Parent;
			addBefore.AddBeforeSelf(new XElement("tr", new XElement("th", "Title"), RenderTitle()));

			return page;
		}

		public override XElement RenderSummaryRow(bool forArea)
		{
			var row = base.RenderSummaryRow(forArea);

			if (forArea)
			{
				var project = row.Elements().First();
				project.Add(
					new XElement("br"),
					new XElement(
						"sup",
						PageDocument.BuildEmoticon(PageDocument.Emoticon.YellowStar),
						"$nbsp$released as$nbsp$",
						PageDocument.BuildPageLink(
							Title,
							new XElement(
								"span",
								new XAttribute("style", PageDocument.Style.Color.DarkGreen),
								new XElement("strong", Title)))));
			}

			return row;
		}

		public override List<ProjectConfiguration> ExportConfigurations()
		{
			var config = new CloudServiceProjectConfiguration();
			ApplyTo(config);

			return new List<ProjectConfiguration> { config };
		}
	}
}
