using System;
using System.Xml.Linq;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Project reference item.
	/// </summary>
	public class ProjectReference : ProjectElement
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ProjectReference(XElement element)
			: base(element)
		{
		}

		/// <summary>
		/// Gets path to include.
		/// </summary>
		public string Include
		{
			get { return m_element.Attribute("Include").Value; }
		}

		/// <summary>
		/// Gets project GUID.
		/// </summary>
		public Guid Project
		{
			get
			{
				var project = m_element.Element(Ns + "Project");
				if (project == null)
					return Guid.Empty;

				return new Guid(project.Value);
			}
		}

		/// <summary>
		/// Gets project name.
		/// </summary>
		public string Name
		{
			get
			{
				var name = m_element.Element(Ns + "Name");
				if (name == null)
					return null;

				return name.Value;
			}
		}

		/// <summary>
		/// Updates the include location.
		/// </summary>
		public void UpdateLocation(string path)
		{
			m_element.Attribute("Include").Value = path;
		}

		/// <summary>
		/// Removes project reference from a project document replacing it with the binary one.
		/// </summary>
		public void ConvertToBinary(TargetFramework framework, string name)
		{
			var include = String.Format("{0}, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL", name);
			var dotNet = framework.ToString().ToLowerInvariant();
			var hintPath = String.Format(@"..\packages\{0}.1.0.0.0\lib\{1}\{0}.dll", name, dotNet);

			var binary = new XElement(
				Ns + "Reference",
				new XAttribute("Include", include),
				new XElement(Ns + "HintPath", hintPath));

			m_element.AddAfterSelf(binary);
			m_element.Remove();
		}
	}
}
