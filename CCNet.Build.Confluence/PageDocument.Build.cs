using System;
using System.Xml.Linq;

namespace CCNet.Build.Confluence
{
	/// <summary>
	/// Builds specified blocks withing confluence XML document.
	/// </summary>
	public partial class PageDocument
	{
		/// <summary>
		/// Possible colors for "status" macro block.
		/// </summary>
		public enum StatusColor
		{
			Grey,
			Red,
			Yellow,
			Green,
			Blue
		}

		/// <summary>
		/// Builds "Status" macro block.
		/// </summary>
		public static XElement BuildStatus(string title, StatusColor color, bool outline)
		{
			return new XElement(
				Name("ac:structured-macro"),
				new XAttribute(Name("ac:name"), "status"),
				new XElement(
					Name("ac:parameter"),
					new XAttribute(Name("ac:name"), "subtle"),
					outline.ToString().ToLowerInvariant()),
				new XElement(
					Name("ac:parameter"),
					new XAttribute(Name("ac:name"), "colour"),
					color.ToString()),
				new XElement(
					Name("ac:parameter"),
					new XAttribute(Name("ac:name"), "title"),
					title));
		}

		/// <summary>
		/// Builds "Table of Contents" macro block.
		/// </summary>
		public static XElement BuildToc()
		{
			return new XElement(
				Name("ac:structured-macro"),
				new XAttribute(Name("ac:name"), "toc"));
		}

		/// <summary>
		/// Builds "Info" macro block.
		/// </summary>
		public static XElement BuildInfo(XElement body)
		{
			CheckBody(body);

			return new XElement(
				Name("ac:structured-macro"),
				new XAttribute(Name("ac:name"), "info"),
				body);
		}

		/// <summary>
		/// Builds "Section" macro block.
		/// </summary>
		public static XElement BuildSection(XElement body)
		{
			CheckBody(body);

			return new XElement(
				Name("ac:structured-macro"),
				new XAttribute(Name("ac:name"), "section"),
				body);
		}

		/// <summary>
		/// Builds "Column" macro block.
		/// </summary>
		public static XElement BuildColumn(string width, XElement body)
		{
			CheckBody(body);

			return new XElement(
				Name("ac:structured-macro"),
				new XAttribute(Name("ac:name"), "column"),
				width == null
					? null
					: new XElement(
						Name("ac:parameter"),
						new XAttribute(Name("ac:name"), "width"),
						width),
				body);
		}

		/// <summary>
		/// Builds "Image" block.
		/// </summary>
		public static XElement BuildImage(string imageUrl)
		{
			return new XElement(
				Name("ac:image"),
				new XElement(
					Name("ri:url"),
					new XAttribute(Name("ri:value"), imageUrl)));
		}

		/// <summary>
		/// Builds "User link" block.
		/// </summary>
		public static XElement BuildUserLink(string userKey)
		{
			return new XElement(
				Name("ac:link"),
				new XElement(
					Name("ri:user"),
					new XAttribute(Name("ri:userkey"), userKey)));
		}

		/// <summary>
		/// Builds "Page link" block.
		/// </summary>
		public static XElement BuildPageLink(string pageTitle, string anchor, string linkText)
		{
			return new XElement(
				Name("ac:link"),
				new XAttribute(Name("ac:anchor"), anchor),
				new XElement(
					Name("ri:page"),
					new XAttribute(Name("ri:content-title"), pageTitle)),
				new XElement(Name("ac:plain-text-link-body"), new XCData(linkText)));
		}

		/// <summary>
		/// Makes sure specified element is a rich text body section.
		/// </summary>
		private static void CheckBody(XElement body)
		{
			if (body.Name != Name("ac:rich-text-body"))
				throw new InvalidOperationException("Rich text body expected.");
		}

		/// <summary>
		/// Builds rich text body section.
		/// </summary>
		public static XElement BuildBody(params object[] content)
		{
			return new XElement(
				Name("ac:rich-text-body"),
				content);
		}
	}
}
