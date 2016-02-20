using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using CCNet.Common;

namespace CCNet.Build.SetupPackages
{
	/// <summary>
	/// Represents project file as XML document.
	/// </summary>
	public class ProjectDocument
	{
		private readonly string m_projectFile;
		private readonly XmlNamespaceManager m_namespaces;

		private XDocument m_document;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ProjectDocument(string projectFile)
		{
			if (String.IsNullOrEmpty(projectFile))
				throw new ArgumentNullException("projectFile");

			m_projectFile = projectFile;

			m_namespaces = new XmlNamespaceManager(new NameTable());
			m_namespaces.AddNamespace("ms", "http://schemas.microsoft.com/developer/msbuild/2003");
		}

		/// <summary>
		/// Ensures project document is loaded.
		/// </summary>
		private void EnsureLoaded()
		{
			if (m_document == null)
				throw new InvalidOperationException("Project document is not loaded.");
		}

		/// <summary>
		/// Loads project file from disk.
		/// </summary>
		public void Load()
		{
			string xml = File.ReadAllText(m_projectFile);
			m_document = XDocument.Parse(xml);
		}

		/// <summary>
		/// Saves project file to disk.
		/// </summary>
		public void Save()
		{
			EnsureLoaded();
			m_document.Save(m_projectFile);
		}

		/// <summary>
		/// Executes XPath query over existing project document and namespace manager.
		/// </summary>
		private XElement SelectElement(string xpath)
		{
			EnsureLoaded();
			return m_document.XPathSelectElement(xpath, m_namespaces);
		}

		/// <summary>
		/// Executes XPath query over existing project document and namespace manager.
		/// </summary>
		private IEnumerable<XElement> SelectElements(string xpath)
		{
			EnsureLoaded();
			return m_document.XPathSelectElements(xpath, m_namespaces);
		}

		public List<BinaryReference> GetBinaryReferences()
		{
			return SelectElements("/ms:Project/ms:ItemGroup/ms:Reference")
				.Select(e => new BinaryReference(e))
				.ToList();
		}
	}
}
