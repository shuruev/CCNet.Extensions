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
		/// Retrieves the XslCompiledTransform for the specified template name, using the results caching.
		/// </summary>
		public static XslCompiledTransform GetCompiledTransform(string templateName)
		{
			return Cache<string, XslCompiledTransform>.Instance.Get(
				templateName,
				() =>
				{
					XslCompiledTransform xslt = new XslCompiledTransform();
					xslt.Load(templateName, XsltSettings.Default, Templates.ResourcesManager.Resolver);
					return xslt;
				});
		}

		/// <summary>
		/// Writes the results of XSLT transformation into the specified TextWriter.
		/// </summary>
		public static void WriteProcessed(string templateName, XsltArgumentList argumentList, XDocument data, TextWriter outStream)
		{
			using (XmlReader reader = data.CreateReader())
			{
				GetCompiledTransform(templateName).Transform(reader, argumentList, outStream);
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
