using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public abstract class ReleaseProjectPage : LibraryProjectPage
	{
		public string Title { get; set; }

		protected ReleaseProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument)
			: base(areaName, projectName, pageName, pageDocument)
		{
			Title = ParseTitle(m_properties);
		}

		private string ParseTitle(Dictionary<string, string> properties)
		{
			var title = FindByKey(properties, key => key.Contains("titl"));

			if (title == null)
				throw new InvalidOperationException("Cannot find project title.");

			var norm = title.AsciiOnly(' ').CleanWhitespaces();
			if (norm.Length < 10)
				throw new InvalidOperationException("Something is wrong with project title.");

			return norm;
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

			var xmlDoc = page.Root.XElement("table/tbody/tr/th[text()='XML documentation']");
			if (xmlDoc != null && Documentation == DocumentationType.None)
			{
				xmlDoc.Parent.Remove();
			}

			return page;
		}

		public override XElement RenderSummaryRow(bool forArea)
		{
			var row = base.RenderSummaryRow(forArea);

			var project = row.Elements().Skip(forArea ? 0 : 1).First();
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

			return row;
		}
	}
}
