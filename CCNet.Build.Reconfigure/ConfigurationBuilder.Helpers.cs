using System;
using System.Xml;

namespace CCNet.Build.Reconfigure
{
	public partial class ConfigurationBuilder : IDisposable
	{
		private class TagElement : IDisposable
		{
			private readonly XmlWriter m_writer;

			public TagElement(XmlWriter writer)
			{
				m_writer = writer;
			}

			public void Dispose()
			{
				m_writer.WriteEndElement();
			}
		}

		private void Comment(string text)
		{
			m_writer.WriteComment($" {text} ");
		}

		private TagElement Tag(string name)
		{
			m_writer.WriteStartElement(name);
			return new TagElement(m_writer);
		}

		private void Tag(string name, string value)
		{
			m_writer.WriteElementString(name, value);
		}

		private TagElement CbTag(string name)
		{
			m_writer.WriteStartElement("cb", name, null);
			return new TagElement(m_writer);
		}

		private void Attr(string name, string value)
		{
			m_writer.WriteAttributeString(name, value);
		}
	}
}
