using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class ProjectProperties
	{
		public string Description { get; set; }
		public string TfsPath { get; set; }
		public string Owner { get; set; }
		public ProjectStatus Status { get; set; }

		public virtual void ParseTable(Dictionary<string, string> properties)
		{
			Description = ExtractDescription(properties);
			TfsPath = ExtractTfsPath(properties);
			Owner = ExtractOwner(properties);
			Status = ExtractStatus(properties);
		}

		protected static string FindByKey(Dictionary<string, string> properties, Func<string, bool> keyFilter)
		{
			return properties
				.Select(i => new Tuple<string, string>(i.Key.AsciiOnly().ToLowerInvariant(), i.Value))
				.Where(i => keyFilter(i.Item1))
				.Select(i => i.Item2)
				.FirstOrDefault();
		}

		protected static T ExtractEnum<T>(
			Dictionary<string, string> properties,
			Func<string, bool> keyFilter,
			Func<string, string> convertValue,
			T byDefault)
			where T : struct
		{
			var value = FindByKey(properties, keyFilter);

			if (value == null)
				return byDefault;

			T result;
			if (Enum.TryParse(convertValue(value), true, out result))
				return result;

			return byDefault;
		}

		protected static T ExtractEnum<T>(Dictionary<string, string> properties, string keyContains, T byDefault)
			where T : struct
		{
			return ExtractEnum(
				properties,
				key => key.Contains(keyContains),
				value => value.AsciiOnly(),
				byDefault);
		}

		private static string ExtractDescription(Dictionary<string, string> properties)
		{
			var desc = FindByKey(properties, key => key.Contains("desc"));

			if (desc == null)
				throw new InvalidOperationException("Cannot find project description.");

			return desc.CleanWhitespaces();
		}

		private static string ExtractTfsPath(Dictionary<string, string> properties)
		{
			var path = FindByKey(properties, key => key.Contains("tfs") || key.Contains("path"));

			if (path == null)
				throw new InvalidOperationException("Cannot find TFS path.");

			if (path != path.AsciiOnly('$', '/', '.').CleanWhitespaces())
				throw new ArgumentException(
					String.Format("TFS path '{0}' does not look well-formed.", path));

			return path;
		}

		private static string ExtractOwner(Dictionary<string, string> properties)
		{
			var owner = FindByKey(properties, key => key.Contains("owner"));

			if (owner == null)
				throw new InvalidOperationException("Cannot find project owner.");

			return owner.AsciiOnly();
		}

		private static ProjectStatus ExtractStatus(Dictionary<string, string> properties)
		{
			return ExtractEnum(properties, "status", ProjectStatus.Temporary);
		}

		private XElement BuildStatus()
		{
			PageDocument.StatusColor color;

			switch (Status)
			{
				case ProjectStatus.Active:
					color = PageDocument.StatusColor.Green;
					break;

				case ProjectStatus.Temporary:
					color = PageDocument.StatusColor.Yellow;
					break;

				case ProjectStatus.Legacy:
					color = PageDocument.StatusColor.Red;
					break;

				default:
					color = PageDocument.StatusColor.Grey;
					break;
			}

			return PageDocument.BuildStatus(Status.ToString(), color, false);
		}

		private XElement BuildOwner()
		{
			if (Owner.ToLowerInvariant() == "na"
				|| Owner.ToLowerInvariant() == "none")
			{
				return PageDocument.BuildStatus("none", PageDocument.StatusColor.Grey, false);
			}

			return PageDocument.BuildUser("8a99855552936a300152936cadf74b66");
		}

		public virtual XElement Build()
		{
			var desc = new XElement("td").XValue(Description);
			var path = new XElement("td", new XElement("code").XValue(TfsPath));
			var owner = new XElement("td", BuildOwner());
			var status = new XElement("td", BuildStatus());

			return new XElement(
				"table",
				new XElement(
					"tbody",
					new XElement("tr", new XElement("th", "Description"), desc),
					new XElement("tr", new XElement("th", "TFS path"), path),
					new XElement("tr", new XElement("th", "Owner"), owner),
					new XElement("tr", new XElement("th", "Status"), status)));
		}
	}
}
