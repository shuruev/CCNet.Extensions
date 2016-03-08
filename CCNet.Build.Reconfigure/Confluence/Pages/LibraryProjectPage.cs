using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class LibraryProjectPage : BasicProjectPage
	{
		public TargetFramework Framework { get; set; }
		public DocumentationType Documentation { get; set; }

		public LibraryProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument)
			: base(areaName, projectName, pageName, pageDocument)
		{
			Framework = ParseFramework(m_properties);
			Documentation = ParseDocumentation(m_properties);
		}

		public override ProjectType Type
		{
			get { return ProjectType.Library; }
		}

		private static TargetFramework ParseFramework(Dictionary<string, string> properties)
		{
			return ParseEnum(
				properties,
				key => key.Contains("net"),
				value => "Net" + value.AsciiOnly(),
				TargetFramework.Net45);
		}

		private static DocumentationType ParseDocumentation(Dictionary<string, string> properties)
		{
			return ParseEnum(properties, "doc", DocumentationType.None);
		}

		private XElement RenderFramework()
		{
			return new XElement("td", BuildFramework(Framework));
		}

		private XElement RenderDocumentation()
		{
			return new XElement(
				"td",
				BuildExplain(
					"Документацияпроекта(Documentation)",
					BuildDocumentation(Documentation)));
		}

		public override PageDocument RenderPage()
		{
			var page = base.RenderPage();

			var addBefore = page.Root.XElement("table/tbody/tr/th[text()='Owner']").Parent;
			addBefore.AddBeforeSelf(new XElement("tr", new XElement("th", ".NET framework"), RenderFramework()));
			addBefore.AddBeforeSelf(new XElement("tr", new XElement("th", "Documentation"), RenderDocumentation()));

			return page;
		}

		public override XElement RenderSummaryRow(bool forArea)
		{
			var row = base.RenderSummaryRow(forArea);

			var framework = row.Elements().Skip(forArea ? 1 : 2).First();
			framework.Value = DisplayFramework(Framework);

			return row;
		}
	}
}
