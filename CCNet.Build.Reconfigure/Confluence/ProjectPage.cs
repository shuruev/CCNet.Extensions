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
		protected readonly Dictionary<string, string> m_properties;

		public string AreaName { get; private set; }
		public string ProjectName { get; private set; }
		public string Description { get; private set; }
		public string Owner { get; private set; }
		public ProjectStatus Status { get; private set; }

		protected ProjectPage(string areaName, string projectName, string pageName, PageDocument pageDocument)
		{
			if (String.IsNullOrEmpty(areaName))
				throw new ArgumentNullException("areaName");

			if (String.IsNullOrEmpty(projectName))
				throw new ArgumentNullException("projectName");

			if (String.IsNullOrEmpty(pageName))
				throw new ArgumentNullException("pageName");

			if (pageDocument == null)
				throw new ArgumentNullException("pageDocument");

			AreaName = areaName;
			ProjectName = projectName;

			m_page = pageName;
			m_root = pageDocument.Root;
			m_properties = ParseProperties(m_root);

			Description = ParseDescription(m_properties);
			Owner = ParseOwner(m_properties);
			Status = ParseStatus(m_properties);
		}

		public string OrderKey
		{
			get { return AreaName + ":" + ProjectName; }
		}

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

				var status = td.XElement("ac:structured-macro[@ac:name='status']/ac:parameter[@ac:name='title']");
				if (status != null)
					value = status.XValue();

				var user = td.XElements("ac:link/ri:user").Select(e => e.XAttribute("ri:userkey").Value).FirstOrDefault();
				if (user != null)
					value = user;

				map[key] = value;
			}

			if (map.Count == 0)
				throw new InvalidOperationException("Cannot locate any rows in properties.");

			return map;
		}

		private string ParseDescription(Dictionary<string, string> properties)
		{
			var desc = FindByKey(properties, key => key.Contains("desc"));

			if (desc == null)
				throw new InvalidOperationException("Cannot find project description.");

			return desc.CleanWhitespaces();
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
			switch (owner.AsciiOnly().ToLowerInvariant())
			{
				case "na":
				case "none":
					return String.Empty;

				case "kluzin":
				case "8a99855552936a300152936cc08259d7":
					return "8a99855552936a300152936cc08259d7";

				case "oshuruev":
				case "olshuruev":
				case "8a99855552936a300152936cadf74b66":
					return "8a99855552936a300152936cadf74b66";

				case "pbelousov":
				case "8a99855552936a300152936cbe1246f1":
					return "8a99855552936a300152936cbe1246f1";

				case "psvintsov":
				case "8a99855552936a300152936cb15d6557":
					return "8a99855552936a300152936cb15d6557";

				case "rpusenkov":
				case "8a99855552936a300152936ca5f61316":
					return "8a99855552936a300152936ca5f61316";

				case "skolemasov":
				case "kolemasovs":
				case "8a99855552936a300152936cb4a77e8a":
					return "8a99855552936a300152936cb4a77e8a";

				default:
					throw new InvalidOperationException(
						String.Format("Unknown user '{0}'.", owner));
			}
		}

		private ProjectStatus ParseStatus(Dictionary<string, string> properties)
		{
			return ParseEnum(properties, "status", ProjectStatus.Temporary);
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
			if (forArea)
			{
				return new XElement(
					"tr",
					RenderNameAndDescription(),
					new XElement("td"),
					new XElement("td", BuildOwner(Owner)),
					new XElement("td", BuildStatus(Status)));
			}

			return new XElement(
				"tr",
				RenderAreaColumn(),
				RenderNameAndDescription(),
				new XElement("td"),
				new XElement("td", BuildOwner(Owner)),
				new XElement("td", BuildStatus(Status)));
		}

		public abstract List<ProjectConfiguration> ExportConfigurations();

		public abstract Tuple<string, Guid> ExportMap();
	}
}
