using System;
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
		public string Namespace { get; set; }

		public LibraryProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument)
			: base(areaName, projectName, pageName, pageDocument)
		{
			Framework = ParseFramework(m_properties);
			Documentation = ParseDocumentation(m_properties);
			Namespace = ParseNamespace(m_properties);
		}

		public override ProjectType Type
		{
			get { return ProjectType.Library; }
		}

		private TargetFramework ParseFramework(Dictionary<string, string> properties)
		{
			return ParseEnum(
				properties,
				key => key.Contains("net"),
				value => "Net" + value.AsciiOnly(),
				TargetFramework.Net45);
		}

		private DocumentationType ParseDocumentation(Dictionary<string, string> properties)
		{
			return ParseEnum(properties, DocumentationType.None, "xml", "doc");
		}

		private string ParseNamespace(Dictionary<string, string> properties)
		{
			var ns = FindByKey(properties, key => key.Contains("namespace"));

			if (ns == null)
				return null;

			ns = ns.AsciiOnly('.');
			if (ns == ProjectName)
				return null;

			return ns;
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
					"Документацияпроекта(XMLdocumentation)",
					BuildDocumentation(Documentation)));
		}

		private XElement RenderNamespace()
		{
			return new XElement(
				"td",
				BuildExplain(
					"Свойствадлябиблиотек(Library)",
					new XElement("code", Namespace)));
		}

		public override PageDocument RenderPage()
		{
			var page = base.RenderPage();

			var addBefore = page.Root.XElement("table/tbody/tr/th[text()='Owner']").Parent;

			if (!String.IsNullOrEmpty(Namespace))
			{
				addBefore.AddBeforeSelf(new XElement("tr", new XElement("th", "Namespace"), RenderNamespace()));
			}

			addBefore.AddBeforeSelf(new XElement("tr", new XElement("th", ".NET framework"), RenderFramework()));
			addBefore.AddBeforeSelf(new XElement("tr", new XElement("th", "XML documentation"), RenderDocumentation()));

			return page;
		}

		public override XElement RenderSummaryRow(bool forArea)
		{
			var row = base.RenderSummaryRow(forArea);

			var framework = row.Elements().Skip(forArea ? 1 : 3).First();
			framework.Value = DisplayFramework(Framework);

			return row;
		}

		public override List<ProjectConfiguration> ExportConfigurations()
		{
			return new List<ProjectConfiguration>
			{
				new LibraryProjectConfiguration
				{
					Name = ProjectName,
					Description = Description,
					Category = AreaName,
					TfsPath = TfsPath,
					//xxx
					OwnerEmail = "oleg.shuruev@cbsinteractive.com",
					Framework = Framework,
					Documentation = Documentation,
					RootNamespace = Namespace
				}
			};
		}
	}
}
