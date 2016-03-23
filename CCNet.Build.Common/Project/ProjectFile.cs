using System;
using System.Xml.Linq;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Project file item.
	/// </summary>
	public class ProjectFile : ProjectElement
	{
		/// <summary>
		/// Gets associated build action.
		/// </summary>
		public BuildAction BuildAction { get; private set; }

		/// <summary>
		/// Gets or sets copying to output directory option.
		/// </summary>
		public CopyToOutputDirectory CopyToOutput { get; private set; }

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

		/// <summary>
		/// Reloads all the inner properties.
		/// </summary>
		protected override void Reload()
		{
			BuildAction action;
			if (BuildAction.TryParse(m_element.Name.LocalName, out action))
			{
				BuildAction = action;
			}
			else
			{
				BuildAction = BuildAction.Other;
			}

			var copy = m_element.Element(Ns + "CopyToOutputDirectory");
			if (copy != null)
			{
				CopyToOutput = (CopyToOutputDirectory)Enum.Parse(typeof(CopyToOutputDirectory), copy.Value);
			}
		}
	}
}
