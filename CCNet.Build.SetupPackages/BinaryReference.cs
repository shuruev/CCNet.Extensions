using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CCNet.Common
{
	/// <summary>
	/// Binary reference item.
	/// </summary>
	public class BinaryReference : ProjectElement
	{
		/// <summary>
		/// Gets fully qualified assembly name.
		/// </summary>
		public AssemblyName AssemblyName { get; private set; }

		/// <summary>
		/// Gets a value indicating whether assembly can be used with specific version only.
		/// </summary>
		public bool SpecificVersion { get; private set; }

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public BinaryReference(XElement element)
			: base(element)
		{
		}

		/// <summary>
		/// Gets assembly name.
		/// </summary>
		public string Name
		{
			get { return AssemblyName.Name; }
		}

		/// <summary>
		/// Gets assembly version.
		/// </summary>
		public Version Version
		{
			get { return AssemblyName.Version; }
		}

		/// <summary>
		/// Gets a value indicating whether the assembly is taken from global collection.
		/// </summary>
		public bool IsGlobal
		{
			get { return Version == null; }
		}

		/// <summary>
		/// Reloads all the inner properties.
		/// </summary>
		protected override void Reload()
		{
			var include = m_element.Attribute("Include").Value;
			AssemblyName = new AssemblyName(include);

			SpecificVersion = true;
			var specific = m_element.Element(m_ns + "SpecificVersion");
			if (specific != null)
			{
				SpecificVersion = Boolean.Parse(specific.Value);
			}
		}

		/// <summary>
		/// Marks assembly as not required to use specific version.
		/// </summary>
		public void ResetSpecificVersion()
		{
			var hint = m_element.Element(m_ns + "HintPath");
			if (hint == null)
				return;

			var specific = m_element.Element(m_ns + "SpecificVersion");
			if (specific == null)
			{
				specific = new XElement(m_ns + "SpecificVersion");
				m_element.Add(specific);
			}

			specific.Value = "False";

			Reload();
		}

		/// <summary>
		/// Updates version within assembly specification.
		/// </summary>
		public void UpdateVersion(Version newVersion)
		{
			if (IsGlobal)
				throw new InvalidOperationException("Global references cannot be updated.");

			var oldValue = String.Format(", Version={0},", Version);
			var newValue = String.Format(", Version={0},", newVersion);

			var include = m_element.Attribute("Include");
			include.Value = include.Value.Replace(oldValue, newValue);

			var hint = m_element.Element(m_ns + "HintPath");
			if (hint != null)
			{
				oldValue = String.Format(@"\\{0}\.[0-9\.?]+\\", Name);
				newValue = String.Format(@"\{0}.{1}\", Name, newVersion.ToString().TrimEnd('.', '0'));

				hint.Value = new Regex(oldValue).Replace(hint.Value, newValue);
			}

			Reload();
		}
	}
}
