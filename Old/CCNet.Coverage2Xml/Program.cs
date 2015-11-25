using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using CCNet.Common;
using CCNet.Coverage2Xml.Properties;
using Microsoft.VisualStudio.Coverage.Analysis;

namespace CCNet.Coverage2Xml
{
	public class Program
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return 0;
			}

			try
			{
				Arguments.Default = ArgumentProperties.Parse(args);

				if (!File.Exists(Arguments.DataCoverageFile))
					return 0;

				using (CoverageInfo info = CoverageInfo.CreateFromFile(Arguments.DataCoverageFile))
				{
					CoverageDS data = info.BuildDataSet();

					string outputXml = Transform(data.GetXml());

					XDocument doc = XDocument.Parse(outputXml);
					ShortenMethodNames(doc);
					File.WriteAllText(Arguments.XmlCoverageFile, doc.ToString());
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.ToString());
				return 1;
			}

			return 0;
		}

		/// <summary>
		/// Displays usage text.
		/// </summary>
		private static void DisplayUsage()
		{
			Console.WriteLine();
			Console.WriteLine(Resources.UsageInfo);
			Console.WriteLine();
		}

		/// <summary>
		/// Deletes unnecessary sections.
		/// </summary>
		private static string Transform(string xml)
		{
			XslCompiledTransform xslt = new XslCompiledTransform();
			using (XmlTextReader xtr = new XmlTextReader(new StringReader(Xslt.Coverage)))
			{
				xslt.Load(xtr);
			}

			StringBuilder resultXml = new StringBuilder();
			using (XmlTextReader xtr = new XmlTextReader(new StringReader(xml)))
			{
				using (StringWriter sw = new StringWriter(resultXml))
				{
					XsltArgumentList args = new XsltArgumentList();
					xslt.Transform(xtr, args, sw);
				}
			}

			return resultXml.ToString();
		}

		/// <summary>
		/// Shortens arguments in method names.
		/// </summary>
		private static void ShortenMethodNames(XDocument doc)
		{
			foreach (var methodNameNode in doc.Descendants("MethodName"))
			{
				methodNameNode.Value = methodNameNode.Value
					.RemoveWrongSymbols()
					.ShortenMethodName();
			}
		}
	}
}
