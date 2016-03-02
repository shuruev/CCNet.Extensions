using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.Confluence
{
	/// <summary>
	/// Represents confluence page as XML document.
	/// </summary>
	public partial class PageDocument
	{
		private static readonly Dictionary<string, string> s_prefixes;
		private static readonly XmlNamespaceManager s_namespaces;
		private static readonly HashSet<string> s_entities;

		private static readonly string s_beforePage;
		private static readonly string s_afterPage;

		private readonly XDocument m_document;

		static PageDocument()
		{
			s_prefixes = new Dictionary<string, string>
			{
				{ "ac", "http://tempuri.org/ac" },
				{ "at", "http://tempuri.org/at" },
				{ "ri", "http://tempuri.org/ri" }
			};

			s_namespaces = new XmlNamespaceManager(new NameTable());
			foreach (var prefix in s_prefixes)
			{
				s_namespaces.AddNamespace(prefix.Key, prefix.Value);
			}

			s_entities = new HashSet<string>
			{
				"nbsp",
				"ndash",
				"middot",
				"thinsp"
			};

			s_beforePage = "<page "
				+ String.Join(" ", s_prefixes.Select(p => String.Format(@"xmlns:{0}=""{1}""", p.Key, p.Value)))
				+ ">";

			s_afterPage = "</page>";
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public PageDocument(string content = null)
		{
			if (content == null)
				content = String.Empty;

			m_document = ParseDocument(content);
		}

		/// <summary>
		/// Gets internal XML namespaces.
		/// </summary>
		public static XmlNamespaceManager Ns
		{
			get { return s_namespaces; }
		}

		/// <summary>
		/// Gets page root element.
		/// </summary>
		public XElement Root
		{
			get { return m_document.Root; }
		}

		/// <summary>
		/// Builds XML name using internal namespace prefixes.
		/// </summary>
		public static XName Name(string name)
		{
			if (!name.Contains(':'))
				return name;

			foreach (var prefix in s_prefixes)
			{
				if (!name.StartsWith(prefix.Key + ":"))
					continue;

				var local = name.Substring(prefix.Key.Length + 1);
				return XName.Get(local, prefix.Value);
			}

			return name;
		}

		/// <summary>
		/// Creates XML document for specified page content.
		/// </summary>
		private static XDocument ParseDocument(string content)
		{
			if (content == null)
				throw new ArgumentNullException("content");

			content = content.CleanWhitespaces();
			content = EncodeEntities(content);

			return XDocument.Parse(s_beforePage + content + s_afterPage);
		}

		/// <summary>
		/// Encodes all HTML entities to avoid confusions for XML parser.
		/// </summary>
		public static string EncodeEntities(string content)
		{
			foreach (var entity in s_entities)
			{
				content = content.Replace('&' + entity + ';', '$' + entity + '$');
			}

			return content;
		}

		/// <summary>
		/// Encodes all HTML entities back to their original representation.
		/// </summary>
		public static string DecodeEntities(string content)
		{
			foreach (var entity in s_entities)
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
				.RemoveFromStart(s_beforePage)
				.RemoveFromEnd(s_afterPage);

			xml = DecodeEntities(xml);
			return xml.CleanWhitespaces();
		}
	}
}
