using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace CCNet.SourceNotifier.XmlProcessor
{
	/// <summary>
	/// Incapsulates XSLT transformations.
	/// </summary>
	public static class TemplateEngine
	{
		/// <summary>
		/// Caches compiled XSLTs.
		/// </summary>
		private static class TemplateCacher
		{
			private static readonly Dictionary<string, XslCompiledTransform> s_cache = new Dictionary<string, XslCompiledTransform>();
			private static readonly object s_locker = new object();

			/// <summary>
			/// Retrieves the XslCompiledTransform for the specified template name, using the results caching.
			/// </summary>
			public static XslCompiledTransform GetCompiledTransform(string templateName)
			{
				if (!s_cache.ContainsKey(templateName))
				{
					lock (s_locker)
					{
						if (!s_cache.ContainsKey(templateName))
						{
							XslCompiledTransform xslt = new XslCompiledTransform();
							xslt.Load(templateName, XsltSettings.Default, Resources.ResourcesManager.Resolver);
							s_cache[templateName] = xslt;
						}
					}
				}

				return s_cache[templateName];
			}
		}

		/// <summary>
		/// Writes the results of XSLT transformation into the specified TextWriter.
		/// </summary>
		public static void WriteProcessed(string templateName, XsltArgumentList argumentList, XDocument data, TextWriter outStream)
		{
			using (XmlReader reader = data.CreateReader())
			{
				TemplateCacher.GetCompiledTransform(templateName).Transform(reader, argumentList, outStream);
			}
		}

		/// <summary>
		/// Returns the results of XSLT transformation as a string.
		/// </summary>
		public static string GetProcessedString(string templateName, XsltArgumentList argumentList, XDocument data)
		{
			using (var writer = new StringWriter())
			{
				WriteProcessed(templateName, argumentList, data, writer);
				return writer.ToString();
			}
		}
	}
}
