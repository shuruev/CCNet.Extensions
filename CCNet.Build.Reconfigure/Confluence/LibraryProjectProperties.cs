using System;
using System.Collections.Generic;
using System.Xml.Linq;
using CCNet.Build.Common;
using CCNet.Build.Confluence;

namespace CCNet.Build.Reconfigure
{
	public class LibraryProjectProperties : ProjectProperties
	{
		public TargetFramework Framework { get; set; }
		public DocumentationType Documentation { get; set; }

		public override void ParseTable(Dictionary<string, string> properties)
		{
			base.ParseTable(properties);

			Documentation = ExtractDocumentation(properties);
			Framework = ExtractFramework(properties);
		}

		private static TargetFramework ExtractFramework(Dictionary<string, string> properties)
		{
			return ExtractEnum(
				properties,
				key => key.Contains("net"),
				value => "Net" + value.AsciiOnly(),
				TargetFramework.Net45);
		}

		private static DocumentationType ExtractDocumentation(Dictionary<string, string> properties)
		{
			return ExtractEnum(properties, "doc", DocumentationType.None);
		}

		private XElement BuildFramework()
		{
			string text;

			switch (Framework)
			{
				case TargetFramework.Net20:
					text = "2.0";
					break;

				case TargetFramework.Net35:
					text = "3.5";
					break;

				case TargetFramework.Net40:
					text = "4.0";
					break;

				case TargetFramework.Net45:
					text = "4.5";
					break;

				default:
					throw new InvalidOperationException(
						String.Format("Unknown target framework '{0}'.", Framework));
			}

			return PageDocument.BuildStatus(text, PageDocument.StatusColor.Blue, true);
		}

		private XElement BuildDocumentation()
		{
			PageDocument.StatusColor color;

			switch (Documentation)
			{
				case DocumentationType.Full:
					color = PageDocument.StatusColor.Green;
					break;

				case DocumentationType.Partial:
					color = PageDocument.StatusColor.Yellow;
					break;

				default:
					color = PageDocument.StatusColor.Grey;
					break;
			}

			return PageDocument.BuildStatus(Documentation.ToString(), color, true);
		}

		public override XElement Build()
		{
			var table = base.Build();

			var framework = new XElement("td", BuildFramework());
			var documentation = new XElement("td", BuildDocumentation());

			var addBefore = table.XElement("tbody/tr/th[text()='Owner']").Parent;
			addBefore.AddBeforeSelf(new XElement("tr", new XElement("th", ".NET framework"), framework));
			addBefore.AddBeforeSelf(new XElement("tr", new XElement("th", "Documentation"), documentation));

			return table;
		}
	}
}
