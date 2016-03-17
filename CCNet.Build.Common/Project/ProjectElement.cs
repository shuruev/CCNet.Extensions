using System;
using System.Xml.Linq;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Represents element from project XML document.
	/// </summary>
	public abstract class ProjectElement
	{
		/// <summary>
		/// Current XML element.
		/// </summary>
		protected readonly XElement m_element;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		protected ProjectElement(XElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			m_element = element;

			Reload();
		}

		/// <summary>
		/// Gets standard namespace for a project XML document.
		/// </summary>
		public static XNamespace Ns
		{
			get { return ProjectDocument.Ns; }
		}

		/// <summary>
		/// Reloads all the inner properties.
		/// </summary>
		protected virtual void Reload()
		{
		}
	}
}
