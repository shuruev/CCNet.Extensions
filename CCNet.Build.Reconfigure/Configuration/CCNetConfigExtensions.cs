using System;
using System.Linq;
using System.Text;
using System.Xml;

namespace CCNet.Build.Reconfigure
{
	public static class CCNetConfigExtensions
	{
		public class Element : IDisposable
		{
			private readonly XmlWriter m_writer;

			public Element(XmlWriter writer)
			{
				m_writer = writer;
			}

			public void Dispose()
			{
				m_writer.WriteEndElement();
			}
		}

		public static void Begin(this XmlWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("cruisecontrol");
			writer.WriteAttributeString("xmlns", "cb", null, "urn:ccnet.config.builder");
		}

		public static void End(this XmlWriter writer)
		{
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}

		public static void Comment(this XmlWriter writer, string text)
		{
			writer.WriteComment(String.Format(" {0} ", text));
		}

		private static void StartTag(this XmlWriter writer, string tagPrefix, string tagName, params string[] attributesAndValues)
		{
			writer.WriteStartElement(tagPrefix, tagName, null);

			for (int i = 0; i < attributesAndValues.Length; i += 2)
			{
				var attribute = attributesAndValues[i];
				var value = attributesAndValues[i + 1];
				writer.WriteAttributeString(attribute, value);
			}
		}

		public static Element OpenTag(this XmlWriter writer, string tagName, params string[] attributesAndValues)
		{
			writer.StartTag(null, tagName, attributesAndValues);
			return new Element(writer);
		}

		public static void CbTag(this XmlWriter writer, string tagName, params string[] attributesAndValues)
		{
			writer.StartTag("cb", tagName, attributesAndValues);
			writer.WriteEndElement();
		}

		public static void Tag(this XmlWriter writer, string tagName, params string[] attributesAndValues)
		{
			writer.StartTag(null, tagName, attributesAndValues);
			writer.WriteEndElement();
		}

		public static void WriteBuildArgs(this XmlWriter writer, params Tuple<string, object>[] arguments)
		{
			if (arguments.Length == 0)
				throw new ArgumentException("Arguments are missing.");

			var sb = new StringBuilder();

			for (int i = 0; i < arguments.Length; i++)
			{
				var arg = arguments[i];
				if (arg.Item1 == null)
					continue;

				if (arg.Item1.Length == 0)
					throw new ArgumentException("Empty argument name.");

				sb.AppendFormat("\r\n\t\t\t\t\t\"{0}={{{1}}}\"", arg.Item1, i);
			}

			if (sb.Length == 0)
				throw new ArgumentException("Arguments are missing.");

			sb.Append("\r\n\t\t\t\t");

			var value = String.Format(sb.ToString(), arguments.Select(a => a.Item2).ToArray());
			writer.WriteElementString("buildArgs", value);
		}
	}
}
