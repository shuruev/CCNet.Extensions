using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using CCNet.Build.Common;

namespace CCNet.Build.Confluence
{
	/// <summary>
	/// Represents confluence page as XML document.
	/// </summary>
	public class PageDocument
	{
		private readonly XmlNamespaceManager m_namespaces;
		private readonly HashSet<string> m_entities;

		private readonly string m_beforePage;
		private readonly string m_afterPage;
		private readonly XDocument m_document;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public PageDocument(string content = null)
		{
			if (content == null)
				content = String.Empty;

			var prefixes = new Dictionary<string, string>
			{
				{ "ac", "http://tempuri.org/ac" },
				{ "at", "http://tempuri.org/at" },
				{ "ri", "http://tempuri.org/ri" }
			};

			m_namespaces = new XmlNamespaceManager(new NameTable());
			foreach (var prefix in prefixes)
			{
				m_namespaces.AddNamespace(prefix.Key, prefix.Value);
			}

			m_entities = new HashSet<string>
			{
				"nbsp",
				"middot"
			};

			m_beforePage = "<page "
				+ String.Join(" ", prefixes.Select(p => String.Format(@"xmlns:{0}=""{1}""", p.Key, p.Value)))
				+ ">";

			m_afterPage = "</page>";

			m_document = ParseDocument(content);
		}

		/// <summary>
		/// Creates XML document for specified page content.
		/// </summary>
		private XDocument ParseDocument(string content)
		{
			if (content == null)
				throw new ArgumentNullException("content");

			content = content.CleanWhitespaces();
			content = EncodeEntities(content);

			return XDocument.Parse(m_beforePage + content + m_afterPage);
		}

		/// <summary>
		/// Encodes all HTML entities to avoid confusions for XML parser.
		/// </summary>
		private string EncodeEntities(string content)
		{
			foreach (var entity in m_entities)
			{
				content = content.Replace('&' + entity + ';', '$' + entity + '$');
			}

			return content;
		}

		/// <summary>
		/// Encodes all HTML  entities back to their original representation.
		/// </summary>
		private string DecodeEntities(string content)
		{
			foreach (var entity in m_entities)
			{
				content = content.Replace('$' + entity + '$', '&' + entity + ';');
			}

			return content;
		}

		/// <summary>
		/// Renders page content back to its XML representation.
		/// </summary>
		public string Render()
		{
			var sb = new StringBuilder();
			using (var sw = new StringWriter(sb))
			{
				using (var xtw = new XmlTextWriter(sw))
				{
					m_document.WriteTo(xtw);
				}
			}

			var xml = sb.ToString();

			xml = xml
				.RemoveFromStart("<?xml version=\"1.0\" encoding=\"utf-16\"?>")
				.RemoveFromStart(m_beforePage)
				.RemoveFromEnd(m_afterPage);

			xml = DecodeEntities(xml);
			return xml.CleanWhitespaces();
		}

		/// <summary>
		/// Executes XPath query over existing project document and namespace manager.
		/// </summary>
		public XElement SelectElement(string xpath)
		{
			return m_document.XPathSelectElement(xpath, m_namespaces);
		}

		/// <summary>
		/// Executes XPath query over existing project document and namespace manager.
		/// </summary>
		public IEnumerable<XElement> SelectElements(string xpath)
		{
			return m_document.XPathSelectElements(xpath, m_namespaces);
		}
	}
}
