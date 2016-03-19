using System;
using System.Collections.Generic;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class ServiceProjectPage : LibraryProjectPage
	{
		public string Title { get; set; }

		public ServiceProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument)
			: base(areaName, projectName, pageName, pageDocument)
		{
			Title = ParseTitle(m_properties);
		}

		public override ProjectType Type
		{
			get { return ProjectType.Service; }
		}

		private string ParseTitle(Dictionary<string, string> properties)
		{
			var title = FindByKey(properties, key => key.Contains("titl"));

			if (title == null)
				throw new InvalidOperationException("Cannot find project title.");

			return title.AsciiOnly(' ').CleanWhitespaces();
		}

		private XElement RenderTitle()
		{
			return new XElement("td", Title);
		}

		public override PageDocument RenderPage()
		{
			var page = base.RenderPage();

			var addBefore = page.Root.XElement("table/tbody/tr/th[text()='Description']").Parent;
			addBefore.AddBeforeSelf(new XElement("tr", new XElement("th", "Title"), RenderTitle()));

			return page;
		}

		public override List<ProjectConfiguration> ExportConfigurations()
		{
			return new List<ProjectConfiguration>
			{
				new ServiceProjectConfiguration
				{
					Name = ProjectName,
					Title = Title,
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
