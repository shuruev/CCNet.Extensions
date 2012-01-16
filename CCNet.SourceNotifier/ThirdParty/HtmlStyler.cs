using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace VX.Sys
{
	public static class HtmlStyler
	{
		private static readonly Regex s_extractStylesRegex = new Regex("<style[^>]*>(?<css>[^<]+)</style>");
		private static readonly Regex s_commentedCssRegex = new Regex(@"/\*.*\s*.*\*/");
		private static readonly Regex s_stylesRegex = new Regex(@"^\s*(?'name'[^{]+){(?'definition'[^}]+)}", RegexOptions.Multiline | RegexOptions.IgnoreCase);
		private static readonly Regex s_styleKeyRegex = new Regex(@"[A-z\-]+(?=\:)");

		/// <summary>
		/// Converts HTML document with CSS style into HTML documend with inlined styles.
		/// Version 1.0.0.2
		/// </summary>
		/// <param name="html">Source HTML document.</param>
		/// <returns>
		/// Inline-styled HTML document.
		/// </returns>
		/// <remarks>
		/// Please ensure that your html compatible with XHTML™ 1.1 http://www.w3.org/TR/xhtml11.
		/// Be informed that priority of applying CSS is not descendant but simply the order of style declaration.
		/// </remarks>
		public static string InlineCss(string html)
		{
			string css = ExtractCssFromHtml(html);
			IEnumerable<CssStyle> cssStyles = GetCssStyleList(css);

			string beforeBodyTagText = GetTextBeforeBodyTag(html);
			string afterBodyTagText = GetTextAfterBodyTag(html);

			html = html.Substring(
				beforeBodyTagText.Length,
				html.Length - beforeBodyTagText.Length - afterBodyTagText.Length);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(PrepareXml(html));

			LowercaseInlineStylePropertyName(doc);

			// Save CSS styles into temporary "new_style" attribute to not override own inline styles.
			foreach (var selector in cssStyles)
			{
				var htmlNodes = doc.SelectNodes(selector.XPathSelector);
				if (htmlNodes != null)
					foreach (XmlNode htmlNode in htmlNodes)
						AppendNewStyleAttribute(doc, selector, htmlNode);
			}

			// Merge CSS "new_style" and "style" attribute with high priprity of "style".
			var updatedNodes = doc.SelectNodes(@"//*[@new_style]");
			if (updatedNodes != null)
				foreach (XmlNode htmlNode in updatedNodes)
					MergeStyles(doc, htmlNode);

			// Remove elements styled with "display:none".
			RemoveHiddenElements(doc);

			StringBuilder sb = new StringBuilder();

			if (!string.IsNullOrEmpty(beforeBodyTagText))
				sb.Append(beforeBodyTagText);

			sb.Append(FormatOutXmlDocument(doc));

			if (!string.IsNullOrEmpty(afterBodyTagText))
				sb.Append(afterBodyTagText);

			return sb.ToString();
		}

		private static string GetTextBeforeBodyTag(string text)
		{
			const string openBodyTag = "<body";
			int index = text.IndexOf(openBodyTag, StringComparison.OrdinalIgnoreCase);

			if (index < 0)
				return string.Empty;

			return text.Substring(0, index);
		}

		private static string GetTextAfterBodyTag(string text)
		{
			const string closeBodyTag = "</body>";
			int index = text.IndexOf(closeBodyTag, StringComparison.OrdinalIgnoreCase);

			if (index < 0)
				return string.Empty;

			return text.Substring(index + closeBodyTag.Length);
		}

		/// <summary>
		/// Removes redundant spaces from result html text.
		/// </summary>
		private static string FormatOutXmlDocument(XmlDocument xmlDocument)
		{
			using (StringWriter sw = new StringWriter())
			{
				XmlWriterSettings settings = new XmlWriterSettings
				{
					OmitXmlDeclaration = true,
					IndentChars = "\t",
					Indent = true,
					NewLineChars = "\r\n\t"
				};

				using (XmlWriter xtw = XmlWriter.Create(sw, settings))
				{
					xmlDocument.Save(xtw);
				}

				return sw.ToString();
			}
		}

		/// <summary>
		/// Removes nodes with style "display: none;" from HTML document.
		/// </summary>
		private static void RemoveHiddenElements(XmlNode doc)
		{
			var htmlNodesToDelete = doc.SelectNodes("//*[contains(translate(@style,\" \",\"\"),\"display:none\")]");
			if (htmlNodesToDelete != null)
				foreach (XmlNode htmlNode in htmlNodesToDelete)
				{
					htmlNode.RemoveAll();
					if (htmlNode.ParentNode != null)
						htmlNode.ParentNode.RemoveChild(htmlNode);
					else
						doc.RemoveChild(htmlNode);
				}
		}

		/// <summary>
		/// Save CSS style into "".
		/// </summary>
		private static void AppendNewStyleAttribute(XmlDocument doc, CssStyle selector, XmlNode htmlNode)
		{
			CssStyleProperties style;

			if (htmlNode.Attributes == null)
				return;

			if (htmlNode.Attributes["new_style"] != null)
			{
				var oldStyleProperties = new CssStyleProperties { Text = htmlNode.Attributes["new_style"].Value };
				var newStyleProperties = new CssStyleProperties { Dictionary = selector.Properties.Dictionary };
				style = oldStyleProperties + newStyleProperties;
			}
			else
			{
				htmlNode.Attributes.Append(doc.CreateAttribute("new_style"));
				style = new CssStyleProperties { Dictionary = selector.Properties.Dictionary };
			}

			htmlNode.Attributes["new_style"].Value = style.Text;
		}

		/// <summary>
		/// Apply CSS style to HTML node.
		/// </summary>
		private static void MergeStyles(XmlDocument doc, XmlNode htmlNode)
		{
			CssStyleProperties style;

			if (htmlNode.Attributes == null)
				return;

			if (htmlNode.Attributes["style"] != null)
			{
				var oldStyleProperties = new CssStyleProperties { Text = htmlNode.Attributes["style"].Value };
				var newStyleProperties = new CssStyleProperties { Text = htmlNode.Attributes["new_style"].Value };
				style = newStyleProperties + oldStyleProperties;
			}
			else
			{
				htmlNode.Attributes.Append(doc.CreateAttribute("style"));
				style = new CssStyleProperties { Text = htmlNode.Attributes["new_style"].Value };
			}

			if (htmlNode.Attributes["class"] != null)
				htmlNode.Attributes.Remove(htmlNode.Attributes["class"]);

			htmlNode.Attributes.Remove(htmlNode.Attributes["new_style"]);
			htmlNode.Attributes["style"].Value = style.Text;
		}

		/// <summary>
		/// Parses CSS style text into separate styles.
		/// </summary>
		private static IEnumerable<CssStyle> GetCssStyleList(string css)
		{
			List<CssStyle> cssCache = new List<CssStyle>();

			s_commentedCssRegex.Replace(css, string.Empty);

			MatchCollection matchCollection = s_stylesRegex.Matches(css);
			for (int i = 0; i < matchCollection.Count; i++)
			{
				string attributes = matchCollection[i].Groups["definition"].Value;
				if (attributes.Trim() == string.Empty)
					continue;

				string name = matchCollection[i].Groups["name"].Value;

				// split style definition like 'a, a.href {???}' or '.t1, .t2 {???}'
				string[] names = name.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

				foreach (string singleName in names)
				{
					if (singleName == null || singleName.Trim() == string.Empty || singleName.Contains(":"))
						continue;

					cssCache.Add(
						new CssStyle
						{
							CssSelector = singleName.Trim(),
							Properties = new CssStyleProperties { Text = attributes },
							OrderIndex = i
						});
				}
			}
			cssCache.Sort();
			return cssCache;
		}

		/// <summary>
		/// Lowercase of inline CSS properties in "style" attributes of HTML document.
		/// </summary>
		private static void LowercaseInlineStylePropertyName(XmlNode doc)
		{
			var nodes = doc.SelectNodes("//*[@style]");
			if (nodes != null)
				for (int i = 0; i < nodes.Count; i++)
				{
					XmlNode node = nodes[i];
					if (node.Attributes == null)
						continue;
					string style = node.Attributes["style"].Value;
					style = s_styleKeyRegex.Replace(
						style,
						m => m.Value.ToLower());
					node.Attributes["style"].Value = style;
				}
		}

		/// <summary>
		/// Extracts CSS style from HTML document.
		/// </summary>
		private static string ExtractCssFromHtml(string html)
		{
			StringBuilder css = new StringBuilder();
			foreach (Match match in s_extractStylesRegex.Matches(html))
			{
				css.AppendLine(match.Groups["css"].Value);
			}
			return css.ToString();
		}

		/// <summary>
		/// Prepare valid XML text.
		/// </summary>
		private static string PrepareXml(string text)
		{
			if (text == null)
				return null;

			#region declare unescape dictionary
			Dictionary<string, string> unascape = new Dictionary<string, string>
			{
				{ "&apos;", "&#39;" },
				{ "&nbsp;", "&#160;" },
				{ "&iexcl;", "&#161;" },
				{ "&cent;", "&#162;" },
				{ "&pound;", "&#163;" },
				{ "&curren;", "&#164;" },
				{ "&yen;", "&#165;" },
				{ "&brvbar;", "&#166;" },
				{ "&sect;", "&#167;" },
				{ "&uml;", "&#168;" },
				{ "&copy;", "&#169;" },
				{ "&ordf;", "&#170;" },
				{ "&laquo;", "&#171;" },
				{ "&not;", "&#172;" },
				{ "&shy;", "&#173;" },
				{ "&reg;", "&#174;" },
				{ "&macr;", "&#175;" },
				{ "&deg;", "&#176;" },
				{ "&plusmn;", "&#177;" },
				{ "&sup2;", "&#178;" },
				{ "&sup3;", "&#179;" },
				{ "&acute;", "&#180;" },
				{ "&micro;", "&#181;" },
				{ "&para;", "&#182;" },
				{ "&middot;", "&#183;" },
				{ "&cedil;", "&#184;" },
				{ "&sup1;", "&#185;" },
				{ "&ordm;", "&#186;" },
				{ "&raquo;", "&#187;" },
				{ "&frac14;", "&#188;" },
				{ "&frac12;", "&#189;" },
				{ "&frac34;", "&#190;" },
				{ "&iquest;", "&#191;" },
				{ "&times;", "&#215;" },
				{ "&divide;", "&#247;" },
				{ "&Agrave;", "&#192;" },
				{ "&Aacute;", "&#193;" },
				{ "&Acirc;", "&#194;" },
				{ "&Atilde;", "&#195;" },
				{ "&Auml;", "&#196;" },
				{ "&Aring;", "&#197;" },
				{ "&AElig;", "&#198;" },
				{ "&Ccedil;", "&#199;" },
				{ "&Egrave;", "&#200;" },
				{ "&Eacute;", "&#201;" },
				{ "&Ecirc;", "&#202;" },
				{ "&Euml;", "&#203;" },
				{ "&Igrave;", "&#204;" },
				{ "&Iacute;", "&#205;" },
				{ "&Icirc;", "&#206;" },
				{ "&Iuml;", "&#207;" },
				{ "&ETH;", "&#208;" },
				{ "&Ntilde;", "&#209;" },
				{ "&Ograve;", "&#210;" },
				{ "&Oacute;", "&#211;" },
				{ "&Ocirc;", "&#212;" },
				{ "&Otilde;", "&#213;" },
				{ "&Ouml;", "&#214;" },
				{ "&Oslash;", "&#216;" },
				{ "&Ugrave;", "&#217;" },
				{ "&Uacute;", "&#218;" },
				{ "&Ucirc;", "&#219;" },
				{ "&Uuml;", "&#220;" },
				{ "&Yacute;", "&#221;" },
				{ "&THORN;", "&#222;" },
				{ "&szlig;", "&#223;" },
				{ "&agrave;", "&#224;" },
				{ "&aacute;", "&#225;" },
				{ "&acirc;", "&#226;" },
				{ "&atilde;", "&#227;" },
				{ "&auml;", "&#228;" },
				{ "&aring;", "&#229;" },
				{ "&aelig;", "&#230;" },
				{ "&ccedil;", "&#231;" },
				{ "&egrave;", "&#232;" },
				{ "&eacute;", "&#233;" },
				{ "&ecirc;", "&#234;" },
				{ "&euml;", "&#235;" },
				{ "&igrave;", "&#236;" },
				{ "&iacute;", "&#237;" },
				{ "&icirc;", "&#238;" },
				{ "&iuml;", "&#239;" },
				{ "&eth;", "&#240;" },
				{ "&ntilde;", "&#241;" },
				{ "&ograve;", "&#242;" },
				{ "&oacute;", "&#243;" },
				{ "&ocirc;", "&#244;" },
				{ "&otilde;", "&#245;" },
				{ "&ouml;", "&#246;" },
				{ "&oslash;", "&#248;" },
				{ "&ugrave;", "&#249;" },
				{ "&uacute;", "&#250;" },
				{ "&ucirc;", "&#251;" },
				{ "&uuml;", "&#252;" },
				{ "&yacute;", "&#253;" },
				{ "&thorn;", "&#254;" },
				{ "&yuml;", "&#255;" },
				{ "&OElig;", "&#338;" },
				{ "&oelig;", "&#339;" },
				{ "&Scaron;", "&#352;" },
				{ "&scaron;", "&#353;" },
				{ "&Yuml;", "&#376;" },
				{ "&circ;", "&#710;" },
				{ "&tilde;", "&#732;" },
				{ "&ensp;", "&#8194;" },
				{ "&emsp;", "&#8195;" },
				{ "&thinsp;", "&#8201;" },
				{ "&zwnj;", "&#8204;" },
				{ "&zwj;", "&#8205;" },
				{ "&lrm;", "&#8206;" },
				{ "&rlm;", "&#8207;" },
				{ "&ndash;", "&#8211;" },
				{ "&mdash;", "&#8212;" },
				{ "&lsquo;", "&#8216;" },
				{ "&rsquo;", "&#8217;" },
				{ "&sbquo;", "&#8218;" },
				{ "&ldquo;", "&#8220;" },
				{ "&rdquo;", "&#8221;" },
				{ "&bdquo;", "&#8222;" },
				{ "&dagger;", "&#8224;" },
				{ "&Dagger;", "&#8225;" },
				{ "&hellip;", "&#8230;" },
				{ "&permil;", "&#8240;" },
				{ "&lsaquo;", "&#8249;" },
				{ "&rsaquo;", "&#8250;" },
				{ "&euro;", "&#8364;" }
			};
			#endregion

			// change every key in unescape dictionary into its value.
			foreach (var ch in unascape)
				text = ReplaceEx(text, ch.Key, ch.Value);

			// change every '&' not a part of (&quot; | &amp; | &lt; | &gt; | &#DIGITS; ) to '&amp';
			text = Regex.Replace(
				text,
				"&(?!quot;|amp;|lt;|gt;|#[0-9]+;)",
				"&amp;");

			return text;
		}

		/// <summary>
		/// Fast string replace method.
		/// </summary>
		private static string ReplaceEx(string original, string pattern, string replacement)
		{
			int count, position0, position1;
			count = position0 = 0;
			int inc = (original.Length / pattern.Length) *
						 (replacement.Length - pattern.Length);
			char[] chars = new char[original.Length + Math.Max(0, inc)];
			while ((position1 = original.IndexOf(pattern,
														 position0)) != -1)
			{
				for (int i = position0; i < position1; ++i)
					chars[count++] = original[i];
				for (int i = 0; i < replacement.Length; ++i)
					chars[count++] = replacement[i];
				position0 = position1 + pattern.Length;
			}
			if (position0 == 0) return original;
			for (int i = position0; i < original.Length; ++i)
				chars[count++] = original[i];
			return new string(chars, 0, count);
		}

		/// <summary>
		/// CSS Style
		/// </summary>
		private class CssStyle : IComparable
		{
			/// <summary>
			/// Gets or sets CSS selector.
			/// </summary> 
			public string CssSelector { get; set; }

			/// <summary>
			/// Gets or sets text of style attributes.
			/// </summary>
			public CssStyleProperties Properties { get; set; }

			/// <summary>
			/// Gets or sets order of applying styles
			/// </summary>
			public int OrderIndex { get; set; }

			/// <summary>
			/// Gets XPath selector translated from CSS selector.
			/// </summary>
			public string XPathSelector
			{
				get
				{
					CssSelector = CssSelector.Trim();

					// Matches any F element that is a child of an element E.
					CssSelector = (new Regex(@"\s+>\s+")).Replace(CssSelector, "/");

					// Matches any F element that is a child of an element E.
					CssSelector = (new Regex(@"(\w+)\s+\+\s+(\w+)")).Replace(
						CssSelector,
						m => string.Format(
							"{0}/following-sibling::*[1]/self::{1}",
							m.Groups[1].Value,
							m.Groups[2].Value));

					// Matches any F element that is a descendant of an E element.
					CssSelector = (new Regex(@"\s+")).Replace(CssSelector, "//");

					// Matches element with attribute
					CssSelector = (new Regex(@"(\w)\[(\w+)\]")).Replace(
						CssSelector,
						m => string.Format(
							"{0}[@{1}]",
							m.Groups[1].Value,
							m.Groups[2].Value));

					// Matches element with EXACT attribute
					CssSelector = (new Regex("(\\w)\\[(\\w+)\\=[\\\'\"]?(\\w+)[\\\'\"]?\\]")).Replace(
						CssSelector,
						m => string.Format(
							"{0}[@{1}=\"{2}\"]",
							m.Groups[1].Value,
							m.Groups[2].Value,
							m.Groups[3].Value));

					// Matches id attributes
					CssSelector = (new Regex("(\\w+)?\\#([\\w\\-]+)")).Replace(
						CssSelector,
						m => string.Format(
							"{0}[@id=\"{1}\"]",
							string.IsNullOrEmpty(m.Groups[1].Value)
								? "*"
								: m.Groups[1].Value,
							m.Groups[2].Value));

					// Matches class attributes
					CssSelector = (new Regex("(\\w+|\\*)?((\\.[\\w\\-]+)+)")).Replace(
						CssSelector,
						delegate(Match match)
						{
							string prefix =
								string.IsNullOrEmpty(match.Groups[1].Value)
									? "*"
									: match.Groups[1].Value;

							string className =
								match.Groups[2].Value.Substring(1);

							string[] arr =
								className.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

							const string a = "[contains(concat(\" \",@class,\" \"),concat(\" \",\"";
							const string b = "\",\" \"))]";

							string suffix = a + string.Join(b + a, arr) + b;
							return prefix + suffix;
						});

					return "//" + CssSelector;
				}
			}

			#region IComparable Members

			public int CompareTo(object obj)
			{
				return OrderIndex.CompareTo(((CssStyle)obj).OrderIndex);
			}

			#endregion
		}

		/// <summary>
		/// CSS Style Properties
		/// </summary>
		private class CssStyleProperties
		{
			public CssStyleProperties()
			{
				Dictionary = new Dictionary<string, string>();
			}

			/// <summary>
			/// Gets or sets content of "style" attribute.
			/// </summary>
			public string Text
			{
				get
				{
					StringBuilder sb = new StringBuilder();
					foreach (KeyValuePair<string, string> keyValuePair in Dictionary)
					{
						sb.AppendFormat("{0}:{1};", keyValuePair.Key.Trim(), keyValuePair.Value.Trim());
					}
					return sb.ToString();
				}
				set
				{
					Dictionary = new Dictionary<string, string>();

					if (value == null)
						return;

					var stylePieces =
						value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

					foreach (var piece in stylePieces)
					{
						if (piece == null || !piece.Contains(":"))
							continue;

						var arr = piece.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

						if (arr.Length < 2 || arr[1] == null || arr[1].Trim() == string.Empty)
							continue;

						Dictionary.Add(arr[0].Trim(), arr[1].Trim());
					}
				}
			}

			/// <summary>
			/// Gets or sets dictionary of separate style properties.
			/// </summary>
			public Dictionary<string, string> Dictionary { get; set; }

			/// <summary>
			/// Summ operator overload. 
			/// </summary>
			/// <remarks>
			/// If both operands have the properties with the same name the second operand will override such properties of the first.
			/// </remarks>
			public static CssStyleProperties operator +(CssStyleProperties oldProperties, CssStyleProperties newProperties)
			{
				List<string> keysToDelete = new List<string>();
				foreach (var property in newProperties.Dictionary.Keys)
					if (oldProperties.Dictionary.ContainsKey(property))
						keysToDelete.Add(property);
				foreach (var key in keysToDelete)
					oldProperties.Dictionary.Remove(key);

				Dictionary<string, string> result = new Dictionary<string, string>();
				foreach (var element in newProperties.Dictionary)
					result.Add(element.Key, element.Value);

				foreach (var element in oldProperties.Dictionary)
					result.Add(element.Key, element.Value);

				return new CssStyleProperties { Dictionary = result };
			}
		}
	}
}
