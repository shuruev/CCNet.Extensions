using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CCNet.Build.Confluence
{
	/// <summary>
	/// Helper methods to work with Confluence XML document.
	/// </summary>
	public static class PageDocumentExtensions
	{
		/// <summary>
		/// Executes XPath query over existing project document and namespace manager.
		/// </summary>
		public static XElement XElement(this XElement element, string xpath)
		{
			return element.XPathSelectElement(xpath, PageDocument.Ns);
		}

		/// <summary>
		/// Executes XPath query over existing project document and namespace manager.
		/// </summary>
		public static IEnumerable<XElement> XElements(this XElement element, string xpath)
		{
			return element.XPathSelectElements(xpath, PageDocument.Ns);
		}

		/// <summary>
		/// Finds attribute by name, using internal namespace prefixes.
		/// </summary>
		public static XAttribute XAttribute(this XElement element, string name)
		{
			return element.Attribute(PageDocument.Nm(name));
		}

		/// <summary>
		/// Decodes specified value from internal HTML entities.
		/// </summary>
		public static string XValue(this XElement element)
		{
			return WebUtility.HtmlDecode(PageDocument.DecodeEntities(element.Value));
		}

		/// <summary>
		/// Encodes internal HTML entities in specified value.
		/// </summary>
		public static XElement XValue(this XElement element, string value)
		{
			element.Value = PageDocument.EncodeEntities(WebUtility.HtmlEncode(value));
			return element;
		}

		/// <summary>
		/// Removes specified attribute, allows null to be passed.
		/// </summary>
		public static void RemoveIfExists(this XAttribute attribute)
		{
			if (attribute == null)
				return;

			attribute.Remove();
		}
	}
}
