using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;
using CCNet.Build.Tfs;

namespace CCNet.Build.Reconfigure
{
	public abstract partial class ProjectPage : IProjectPage
	{
		protected readonly string m_page;
		protected readonly XElement m_root;
		protected readonly BuildOwners m_owners;
		protected readonly Dictionary<string, string> m_properties;

		public string AreaName { get; }
		public string ProjectName { get; }
		public string ProjectBranch { get; }
		public string Description { get; }
		public string Owner { get; }
		public ProjectStatus Status { get; }

		protected ProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument, BuildOwners buildOwners)
		{
			if (String.IsNullOrEmpty(areaName))
				throw new ArgumentNullException(nameof(areaName));

			if (String.IsNullOrEmpty(projectName))
				throw new ArgumentNullException(nameof(projectName));

			if (String.IsNullOrEmpty(pageName))
				throw new ArgumentNullException(nameof(pageName));

			if (pageDocument == null)
				throw new ArgumentNullException(nameof(pageDocument));

			if (buildOwners == null)
				throw new ArgumentNullException(nameof(buildOwners));

			AreaName = areaName;
			ProjectName = projectName;
			ProjectBranch = ParseProjectBranch(pageName, projectName);

			m_page = pageName;
			m_root = pageDocument.Root;
			m_owners = buildOwners;
			m_properties = ParseProperties(m_root);

			Description = ParseDescription(m_properties);
			Owner = ParseOwner(m_properties);
			Status = ParseStatus(m_properties);
		}

		public string OrderKey => AreaName + ":" + ProjectName;

		private Dictionary<string, string> ParseProperties(XElement page)
		{
			var table = page.XElement("table");
			if (table == null)
				throw new InvalidOperationException("Cannot locate properties table.");

			var map = new Dictionary<string, string>();
			foreach (var tr in table.XElements("tbody/tr"))
			{
				var columns = tr.Elements().ToList();
				if (columns.Count < 2)
					throw new InvalidOperationException("Properties table should contain at least 2 columns.");

				var th = columns[0];
				var td = columns[1];

				var key = th.XValue();
				var value = td.XValue();

				var code = td.XElement("code");
				if (code != null)
					value = code.XValue();

				var status = td.XElement("div/ac:structured-macro[@ac:name='status']/ac:parameter[@ac:name='title']|ac:structured-macro[@ac:name='status']/ac:parameter[@ac:name='title']");
				if (status != null)
					value = status.XValue();

				var user = td.XElements("div/ac:link/ri:user|ac:link/ri:user").Select(e => e.XAttribute("ri:userkey").Value).FirstOrDefault();
				if (user != null)
					value = user;

				var link = td.XElements("ac:link/ri:page").Select(e => e.XAttribute("ri:content-title").Value).FirstOrDefault();
				if (link != null)
					value = link;

				map[key] = value.Trim();
			}

			if (map.Count == 0)
				throw new InvalidOperationException("Cannot locate any rows in properties.");

			return map;
		}

		private string ParseProjectBranch(string pageName, string projectName)
		{
			if (pageName.IndexOf("//") < 0)
				return null;

			return pageName.Substring(0, pageName.IndexOf("//")).TrimEnd();
		}

		private string ParseDescription(Dictionary<string, string> properties)
		{
			var desc = FindByKey(properties, key => key.Contains("desc"));

			if (desc == null)
				throw new InvalidOperationException("Cannot find project description.");

			var norm = desc.CleanWhitespaces();
			if (norm.Length < 10)
				throw new InvalidOperationException("Something is wrong with project description.");

			return norm;
		}

		private string ParseOwner(Dictionary<string, string> properties)
		{
			var owner = FindByKey(properties, key => key.Contains("owner"));

			if (owner == null)
				throw new InvalidOperationException("Cannot find project owner.");

			return NormalizeOwner(owner);
		}

		private string NormalizeOwner(string owner)
		{
			var user = owner.AsciiOnly().ToLowerInvariant();

			switch (user)
			{
				case "na":
				case "none":
					return String.Empty;

				default:
					return m_owners.GetUid(user);
			}
		}

		private ProjectStatus ParseStatus(Dictionary<string, string> properties)
		{
			return ParseEnum(properties, ProjectStatus.Normal, "status");
		}

		private XElement RenderDescription()
		{
			return new XElement("td").XValue(Description);
		}

		private XElement RenderOwner()
		{
			return new XElement(
				"td",
				BuildExplain(
					"Владелецпроекта(Owner)",
					BuildOwner(Owner)));
		}

		private XElement RenderStatus()
		{
			return new XElement(
				"td",
				BuildExplain(
					"Статуспроекта(Status)",
					BuildStatus(Status)));
		}

		private XElement RenderProperties()
		{
			return new XElement(
				"table",
				new XElement(
					"tbody",
					new XElement("tr", new XElement("th", "Description"), RenderDescription()),
					new XElement("tr", new XElement("th", "Owner"), RenderOwner()),
					new XElement("tr", new XElement("th", "Status"), RenderStatus())));
		}

		private XElement RenderNameAndDescription()
		{
			return new XElement(
				"td",
				PageDocument.BuildPageLink(m_page),
				new XElement("br"),
				new XElement("sup").XValue(Description));
		}

		private XElement RenderAreaColumn()
		{
			return new XElement(
				"td",
				PageDocument.BuildPageLink(AreaName + " area", AreaName));
		}

		private XElement RenderNameColumn()
		{
			return new XElement(
				"td",
				PageDocument.BuildPageLink(m_page, ProjectName));
		}

		private XElement RenderTypeColumn()
		{
			return new XElement(
				"td",
				m_page.Replace(ProjectName, String.Empty).Trim());
		}

		public virtual void CheckPage(TfsClient client)
		{
		}

		public virtual PageDocument RenderPage()
		{
			var page = new PageDocument();

			page.Root.Add(RenderProperties());

			return page;
		}

		public virtual XElement RenderSummaryRow(bool forArea)
		{
			const string na = "n/a";

			if (forArea)
			{
				return new XElement(
					"tr",
					RenderNameAndDescription(),
					new XElement("td", na),
					new XElement("td", BuildOwner(Owner)),
					new XElement("td", BuildStatus(Status)));
			}

			return new XElement(
				"tr",
				RenderAreaColumn(),
				RenderNameColumn(),
				RenderTypeColumn(),
				new XElement("td", na),
				new XElement("td", BuildOwner(Owner)),
				new XElement("td", BuildStatus(Status)));
		}

		protected void ApplyTo(ProjectConfiguration config)
		{
			config.Name = ProjectName;
			config.Branch = ProjectBranch;
			config.Description = Description;
			config.Category = AreaName;

			if (!String.IsNullOrEmpty(Owner))
				config.OwnerEmail = m_owners.GetEmail(Owner);

			switch (Status)
			{
				case ProjectStatus.Active:
					config.BuildEvery = TimeSpan.FromSeconds(45);
					break;

				case ProjectStatus.Normal:
					config.BuildEvery = TimeSpan.FromMinutes(5);
					break;

				case ProjectStatus.Legacy:
					config.BuildEvery = TimeSpan.FromHours(3);
					break;
			}
		}

		public abstract List<IProjectConfigurationTemp> ExportConfigurations();

		public abstract Tuple<string, Guid> ExportMap();
	}
}
