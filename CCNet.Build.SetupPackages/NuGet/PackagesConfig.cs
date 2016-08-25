using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace CCNet.Build.SetupPackages
{
	public class PackagesConfig
	{
		private readonly string m_file;
		private readonly XDocument m_xml;

		public PackagesConfig(string filePath)
		{
			if (String.IsNullOrEmpty(filePath))
				throw new ArgumentNullException(nameof(filePath));

			m_file = filePath;

			var xml = File.ReadAllText(m_file);
			m_xml = XDocument.Parse(xml);
		}

		public void Save()
		{
			m_xml.Save(m_file);
		}

		public IEnumerable<XElement> AllPackages()
		{
			return m_xml.Root.Elements("package");
		}
	}
}
