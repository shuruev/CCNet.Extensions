using System.Xml.Linq;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Project file item.
	/// </summary>
	public class ProjectFile : ProjectElement
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ProjectFile(XElement element)
			: base(element)
		{
		}

		/// <summary>
		/// Gets full name.
		/// </summary>
		public string FullName
		{
			get { return m_element.Attribute("Include").Value; }
		}
	}
}
