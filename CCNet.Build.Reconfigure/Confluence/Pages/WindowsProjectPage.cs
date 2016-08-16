using System.Collections.Generic;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class WindowsProjectPage : ReleaseProjectPage
	{
		public bool ClickOnce { get; set; }

		public WindowsProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument, BuildOwners buildOwners)
			: base(areaName, projectName, pageName, pageDocument, buildOwners)
		{
			ClickOnce = ParseClickOnce(m_properties);
		}

		public override ProjectType Type => ProjectType.Windows;

		private bool ParseClickOnce(Dictionary<string, string> properties)
		{
			return ParseBoolean(properties, false, "click");
		}

		private XElement RenderClickOnce()
		{
			return new XElement("td", BuildBoolean(ClickOnce));
		}

		public override PageDocument RenderPage()
		{
			var page = base.RenderPage();

			var addBefore = page.Root.XElement("table/tbody/tr/th[text()='TFS path']").Parent;

			if (ClickOnce)
			{
				addBefore.AddAfterSelf(new XElement("tr", new XElement("th", "ClickOnce"), RenderClickOnce()));
			}

			return page;
		}

		public override List<ProjectConfiguration> ExportConfigurations()
		{
			var config = new WindowsProjectConfiguration();
			ApplyTo(config);

			return new List<ProjectConfiguration> { config };
		}
	}
}
