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
		/// Removes project reference from a project document replacing it with the binary one.
		/// </summary>
		public void ConvertToBinary(TargetFramework framework)
		{
			var binary = new XElement(
				Ns + "Reference",
				new XAttribute("Include", String.Format("{0}, Version=1.0.0.0, Culture=neutral, processorArchitecture=MSIL", Name)),
				new XElement(
					Ns + "HintPath",
					String.Format(@"..\packages\{0}.1.0.0.0\lib\net10\{0}.dll", Name)));

			m_element.AddAfterSelf(binary);
			m_element.Remove();
		}
	}
}
