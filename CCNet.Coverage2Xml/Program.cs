using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.VisualStudio.Coverage.Analysis;

namespace CCNet.Coverage2Xml
{
	public class Program
	{
		public static int Main(string[] args)
		{
			try
			{
				string file = args[0];
				string output = args[1];

				if (!File.Exists(file))
					return 0;

				using (CoverageInfo info = CoverageInfo.CreateFromFile(file))
				{
					CoverageDS data = info.BuildDataSet();

					string outputXml = Transform(data.GetXml());

					XDocument doc = XDocument.Parse(outputXml);
					File.WriteAllText(output, doc.ToString());
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.ToString());
				return 1;
			}

			return 0;
		}

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
	}
}
