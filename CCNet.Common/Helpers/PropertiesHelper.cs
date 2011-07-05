using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace CCNet.Common
{
	/// <summary>
	/// Common methods for working with properties collections.
	/// </summary>
	public static class PropertiesHelper
	{
		#region Parsing from XML

		/// <summary>
		/// Gets properties collection from an XML node.
		/// </summary>
		public static Dictionary<string, string> ParseFromXml(XmlNode node)
		{
			Contract.Requires(node != null);

			Dictionary<string, string> properties = new Dictionary<string, string>();
			ResearchNode(properties, String.Empty, node);

			return properties;
		}

		/// <summary>
		/// Gets properties collection from an XML document.
		/// </summary>
		public static Dictionary<string, string> ParseFromXml(XmlDocument document)
		{
			Contract.Requires(document != null);

			return ParseFromXml(document.DocumentElement);
		}

		/// <summary>
		/// Reseraches specified node.
		/// </summary>
		private static void ResearchNode(Dictionary<string, string> properties, string prefix, XmlNode node)
		{
			RemoveXmlnsAttributes(node);
			RemoveCommentNodes(node);

			if (node.NodeType == XmlNodeType.Text)
			{
				properties.Add(
					prefix,
					node.Value.Trim());
				return;
			}

			string subPrefix = "{0}/{1}".Display(prefix, node.Name);
			if (node.ChildNodes.Count == 0 && node.Attributes.Count == 0)
			{
				properties.Add(
					subPrefix,
					String.Empty);
				return;
			}

			foreach (XmlAttribute attr in node.Attributes)
			{
				properties.Add(
					"{0}/{1}[@{2}]".Display(prefix, node.Name, attr.Name),
					attr.Value);
			}

			foreach (XmlNode child in node.ChildNodes)
			{
				ResearchNode(properties, subPrefix, child);
			}
		}

		/// <summary>
		/// Removes all namespace attributes from specified node.
		/// </summary>
		private static void RemoveXmlnsAttributes(XmlNode node)
		{
			if (node.Attributes == null)
				return;

			foreach (XmlAttribute attr in node.Attributes
				.Cast<XmlAttribute>()
				.Where(attr => attr.Name == "xmlns" || attr.Name.StartsWith("xmlns:", StringComparison.Ordinal))
				.ToList())
			{
				node.Attributes.Remove(attr);
			}
		}

		/// <summary>
		/// Removes all comment nodes from specified node.
		/// </summary>
		private static void RemoveCommentNodes(XmlNode node)
		{
			if (node.ChildNodes.Count == 0)
				return;

			foreach (XmlNode child in node.ChildNodes
				.Cast<XmlNode>()
				.Where(child => child.NodeType == XmlNodeType.Comment)
				.ToList())
			{
				node.RemoveChild(child);
			}
		}

		#endregion

		#region Parsing from assembly information file

		private static readonly Regex s_usingRegex = new Regex(@"^using [\w.]+;$");
		private static readonly Regex s_commentRegex = new Regex(@"^\s*//.*$");
		private static readonly Regex s_propertyRegex = new Regex(@"^\[assembly: (?<Name>\w+)\((?<Value>.*)\)]$");

		private static readonly List<string> s_wpfIgnore = new List<string>
		{
			"[assembly: ThemeInfo(",
			"	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located",
			"	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located",
			")]"
		};

		/// <summary>
		/// Gets properties collection from an assembly information file.
		/// </summary>
		public static Dictionary<string, string> ParseFromAssemblyInfo(IEnumerable<string> fileLines)
		{
			Contract.Requires(fileLines != null);
			Contract.Requires(fileLines.Any());

			Dictionary<string, string> properties = new Dictionary<string, string>();
			foreach (string line in fileLines)
			{
				ResearchLine(properties, line);
			}

			return properties;
		}

		/// <summary>
		/// Reseraches specified line.
		/// </summary>
		private static void ResearchLine(Dictionary<string, string> properties, string line)
		{
			if (String.IsNullOrWhiteSpace(line))
				return;

			if (s_usingRegex.IsMatch(line))
				return;

			if (s_commentRegex.IsMatch(line))
				return;

			// temporatily ignoring WPF features
			if (s_wpfIgnore.Contains(line))
				return;

			if (s_propertyRegex.IsMatch(line))
			{
				Match match = s_propertyRegex.Match(line);
				string name = match.Groups["Name"].Value;
				string value = match.Groups["Value"].Value.Trim('"');

				if (name == "InternalsVisibleTo")
					return;

				properties.Add(name, value);

				return;
			}

			throw new InvalidOperationException(
				"Line '{0}' was not expected in assembly information file."
				.Display(line));
		}

		#endregion
	}
}
