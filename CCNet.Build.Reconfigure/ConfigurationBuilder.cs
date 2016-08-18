using System;
using System.Text;
using System.Xml;

namespace CCNet.Build.Reconfigure
{
	public partial class ConfigurationBuilder : IDisposable
	{
		private readonly XmlWriter m_writer;

		public ConfigurationBuilder(string serverName, string filePath)
		{
			m_writer = new XmlTextWriter(filePath, Encoding.UTF8)
			{
				Formatting = Formatting.Indented,
				IndentChar = '\t',
				Indentation = 1
			};

			m_writer.WriteStartDocument();
			m_writer.WriteStartElement("cruisecontrol");
			m_writer.WriteAttributeString("xmlns", "cb", null, "urn:ccnet.config.builder");

			WriteHeader(serverName);
		}

		public void Dispose()
		{
			m_writer.WriteEndElement();
			m_writer.WriteEndDocument();

			m_writer.Dispose();
		}

		private void WriteHeader(string serverName)
		{
			Comment("SERVER NAME");
			using (CbTag("define"))
			{
				Attr("serverName", serverName);
			}

			Comment("IMPORT GLOBAL");
			using (CbTag("include"))
			{
				Attr("href", "Global.config");
			}
		}

		public void Write(IProjectConfiguration config)
		{
			Comment($"PROJECT: {config.UniqueName()}");
			using (Tag("project"))
			{
				WriteProjectHeader(config);
			}
		}

		private void WriteProjectHeader(IProjectConfiguration config)
		{
			Tag("name", config.UniqueName());
			Tag("description", config.Description);
			Tag("queue", config.Area);
			Tag("category", config.Area);

			Tag("workingDirectory", config.ProjectDirectory());
			Tag("artifactDirectory", config.ProjectDirectory());

			using (Tag("state"))
			{
				Attr("type", "state");
				Attr("directory", config.ProjectDirectory());
			}

			Tag("webURL", config.WebUrl());
		}
	}
}
