using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Represents project file as XML document.
	/// </summary>
	public class ProjectDocument
	{
		private static readonly XNamespace s_ns;
		private static readonly XmlNamespaceManager s_namespaces;

		private readonly string m_projectFile;
		private XDocument m_document;

		static ProjectDocument()
		{
			s_ns = "http://schemas.microsoft.com/developer/msbuild/2003";
			s_namespaces = new XmlNamespaceManager(new NameTable());
			s_namespaces.AddNamespace("ms", s_ns.NamespaceName);
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ProjectDocument()
		{
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ProjectDocument(string projectFile)
			: this()
		{
			if (String.IsNullOrEmpty(projectFile))
				throw new ArgumentNullException("projectFile");

			m_projectFile = projectFile;
		}

		/// <summary>
		/// Gets standard namespace for a project XML document.
		/// </summary>
		public static XNamespace Ns
		{
			get { return s_ns; }
		}

		/// <summary>
		/// Ensures project is associated with local file.
		/// </summary>
		private void EnsureLocal()
		{
			if (String.IsNullOrEmpty(m_projectFile))
				throw new InvalidOperationException("Project is not associated with local file.");
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
			EnsureLocal();

			string xml = File.ReadAllText(m_projectFile);
			Load(xml);
		}

		/// <summary>
		/// Loads project file from external XML data.
		/// </summary>
		public void Load(string xml)
		{
			m_document = XDocument.Parse(xml);
		}

		/// <summary>
		/// Saves project file to disk.
		/// </summary>
		public void Save()
		{
			EnsureLocal();
			EnsureLoaded();

			m_document.Save(m_projectFile);
		}

		/// <summary>
		/// Executes XPath query over existing project document and namespace manager.
		/// </summary>
		private XElement SelectElement(string xpath)
		{
			EnsureLoaded();
			return m_document.XPathSelectElement(xpath, s_namespaces);
		}

		/// <summary>
		/// Executes XPath query over existing project document and namespace manager.
		/// </summary>
		private IEnumerable<XElement> SelectElements(string xpath)
		{
			EnsureLoaded();
			return m_document.XPathSelectElements(xpath, s_namespaces);
		}

		/// <summary>
		/// Returns all binary references from current project.
		/// </summary>
		public List<BinaryReference> GetBinaryReferences()
		{
			return SelectElements("/ms:Project/ms:ItemGroup/ms:Reference")
				.Select(e => new BinaryReference(e))
				.ToList();
		}

		/// <summary>
		/// Gets unique project ID.
		/// </summary>
		public Guid GetProjectGuid()
		{
			var node = SelectElement("/ms:Project/ms:PropertyGroup/ms:ProjectGuid");
			return new Guid(node.Value);
		}

		/// <summary>
		/// Gets project type IDs.
		/// </summary>
		public HashSet<Guid> GetProjectTypeGuids()
		{
			var node = SelectElement("/ms:Project/ms:PropertyGroup/ms:ProjectTypeGuids");
			if (node == null)
				return new HashSet<Guid>();

			return new HashSet<Guid>(
				node.Value
				.Split(';')
				.Select(id => new Guid(id)));
		}

		/// <summary>
		/// Gets a list of used conditions.
		/// </summary>
		private List<string> GetUsedConditions()
		{
			var result = new List<string>();
			foreach (var node in SelectElements("/ms:Project/ms:PropertyGroup[@Condition]"))
			{
				var condition = node.Attribute("Condition").Value;
				var value = condition
					.Replace("'$(Configuration)|$(Platform)' == ", String.Empty)
					.Trim('\'', ' ');

				result.Add(value);
			}

			var configuration = SelectElement("/ms:Project/ms:PropertyGroup/ms:Configuration").Value;
			var platform = SelectElement("/ms:Project/ms:PropertyGroup/ms:Platform").Value;
			result.Add(String.Format("{0}|{1}", configuration, platform));

			return result;
		}

		/// <summary>
		/// Gets a list of used configurations.
		/// </summary>
		public List<string> GetUsedConfigurations()
		{
			return GetUsedConditions()
				.Select(condition => condition.Split('|'))
				.Where(parts => parts.Length > 0)
				.Select(parts => parts[0])
				.Distinct()
				.ToList();
		}

		/// <summary>
		/// Gets a list of used platforms.
		/// </summary>
		public List<string> GetUsedPlatforms()
		{
			return GetUsedConditions()
				.Select(condition => condition.Split('|'))
				.Where(parts => parts.Length > 1)
				.Select(parts => parts[1])
				.Distinct()
				.ToList();
		}

		/// <summary>
		/// Gets all common properties.
		/// </summary>
		public Dictionary<string, string> GetCommonProperties()
		{
			return SelectElements("/ms:Project/ms:PropertyGroup[not(@Condition)]")
				.SelectMany(group => group.Elements())
				.ToDictionary(node => node.Name.LocalName, node => node.Value);
		}

		/// <summary>
		/// Gets all properties specific to condition.
		/// </summary>
		private Dictionary<string, string> GetConditionProperties(string condition)
		{
			var group = SelectElement(String.Format("/ms:Project/ms:PropertyGroup[contains(@Condition, '{0}')]", condition));
			if (group == null)
				return new Dictionary<string, string>();

			return group.Elements().ToDictionary(node => node.Name.LocalName, node => node.Value);
		}

		/// <summary>
		/// Gets all properties specific to Debug configuration.
		/// </summary>
		public Dictionary<string, string> GetDebugProperties()
		{
			return GetConditionProperties("Debug");
		}

		/// <summary>
		/// Gets all properties specific to Release configuration.
		/// </summary>
		public Dictionary<string, string> GetReleaseProperties()
		{
			return GetConditionProperties("Release");
		}

		/// <summary>
		/// Gets project files.
		/// </summary>
		public List<ProjectFile> GetProjectFiles()
		{
			return SelectElements("/ms:Project/ms:ItemGroup/ms:None")
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:Compile"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:Content"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:EmbeddedResource"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:EntityDeploy"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:Resource"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:Shadow"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:ApplicationDefinition"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:Page"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:ServiceDefinition"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:ServiceConfiguration"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:PublishProfile"))
				.Union(SelectElements("/ms:Project/ms:ItemGroup/ms:SplashScreen"))
				.Select(e => new ProjectFile(e))
				.ToList();
		}
	}
}
