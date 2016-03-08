using System;
using System.Xml.Linq;

namespace CCNet.Build.Confluence
{
	/// <summary>
	/// Builds specified blocks within confluence XML document.
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
		/// Builds new element, using internal namespace prefixes.
		/// </summary>
		private static XElement Element(string name, params object[] content)
		{
			return new XElement(Nm(name), content);
		}

		/// <summary>
		/// Builds new attribute, using internal namespace prefixes.
		/// </summary>
		private static XAttribute Attribute(string name, object value)
		{
			return new XAttribute(Nm(name), value);
		}

		/// <summary>
		/// Builds "Status" macro block.
		/// </summary>
		public static XElement BuildStatus(string title, StatusColor color, bool outline)
		{
			return Element(
				"ac:structured-macro",
				Attribute("ac:name", "status"),
				Element(
					"ac:parameter",
					Attribute("ac:name", "subtle"),
					outline.ToString().ToLowerInvariant()),
				Element(
					"ac:parameter",
					Attribute("ac:name", "colour"),
					color.ToString()),
				Element(
					"ac:parameter",
					Attribute("ac:name", "title"),
					title));
		}

		/// <summary>
		/// Builds "Table of Contents" macro block.
		/// </summary>
		public static XElement BuildToc()
		{
			return Element(
				"ac:structured-macro",
				Attribute("ac:name", "toc"));
		}

		/// <summary>
		/// Builds "Info" macro block.
		/// </summary>
		public static XElement BuildInfo(XElement body)
		{
			CheckBody(body);

			return Element(
				"ac:structured-macro",
				Attribute("ac:name", "info"),
				body);
		}

		/// <summary>
		/// Builds "Section" macro block.
		/// </summary>
		public static XElement BuildSection(XElement body)
		{
			CheckBody(body);

			return Element(
				"ac:structured-macro",
				Attribute("ac:name", "section"),
				body);
		}

		/// <summary>
		/// Builds "Column" macro block.
		/// </summary>
		public static XElement BuildColumn(string width, XElement body)
		{
			CheckBody(body);

			return Element(
				"ac:structured-macro",
				Attribute("ac:name", "column"),
				width == null
					? null
					: Element(
						"ac:parameter",
						Attribute("ac:name", "width"),
						width),
				body);
		}

		/// <summary>
		/// Builds "Image" block.
		/// </summary>
		public static XElement BuildImage(string imageUrl)
		{
			return Element(
				"ac:image",
				Element(
					"ri:url",
					Attribute("ri:value", imageUrl)));
		}

		/// <summary>
		/// Builds "User link" block.
		/// </summary>
		public static XElement BuildUserLink(string userKey)
		{
			return Element(
				"ac:link",
				Element(
					"ri:user",
					Attribute("ri:userkey", userKey)));
		}

		/// <summary>
		/// Builds "Page link" block.
		/// </summary>
		public static XElement BuildPageLink(string pageTitle, string linkText = null, string anchor = null)
		{
			return Element(
				"ac:link",
				String.IsNullOrEmpty(anchor) ? null : Attribute("ac:anchor", anchor),
				Element(
					"ri:page",
					Attribute("ri:content-title", pageTitle)),
					String.IsNullOrEmpty(linkText) ? null : Element("ac:plain-text-link-body", new XCData(linkText)));
		}

		/// <summary>
		/// Makes sure specified element is a rich text body section.
		/// </summary>
		private static void CheckBody(XElement body)
		{
			if (body.Name != Nm("ac:rich-text-body"))
				throw new InvalidOperationException("Rich text body expected.");
		}

		/// <summary>
		/// Builds rich text body section.
		/// </summary>
		public static XElement BuildBody(params object[] content)
		{
			return Element(
				"ac:rich-text-body",
				content);
		}
	}
}
