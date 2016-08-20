using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atom.Toolbox;

namespace NetBuild.ConsoleApp
{
	/// <summary>
	/// Holds named properties from console arguments.
	/// Names are stored without whitespace characters in case-insensitive way.
	/// Values cannot be null.
	/// </summary>
	public class ConsoleArgumentsReader : IConfigReader
	{
		private readonly Dictionary<string, string> m_values;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ConsoleArgumentsReader()
		{
			m_values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gets all property names.
		/// </summary>
		public List<string> Names => m_values.Keys.ToList();

		/// <summary>
		/// Converts name for an internal usage.
		/// </summary>
		protected static string ConvertName(string name)
		{
			var sb = new StringBuilder();
			foreach (var c in name)
			{
				if (Char.IsWhiteSpace(c))
					continue;

				sb.Append(c);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Adds a new property.
		/// </summary>
		public void Add(string name, string value)
		{
			if (name == null)
				throw new ArgumentException("Property name cannot be null.");

			if (value == null)
				throw new ArgumentException("Property value cannot be null.");

			var key = ConvertName(name);
			if (key.Length == 0)
				throw new ArgumentException($"Specified value '{name}' cannot be used as property name.");

			if (m_values.ContainsKey(key))
				throw new InvalidOperationException($"Another value was already specified for property name '{key}'.");

			m_values.Add(key, value);
		}

		/// <summary>
		/// Gets property value by specified name.
		/// Returns null if value does not exist.
		/// </summary>
		public string GetValue(string name)
		{
			if (name == null)
				throw new ArgumentException("Property name cannot be null.");

			var key = ConvertName(name);
			if (!m_values.ContainsKey(key))
				return null;

			return m_values[key];
		}

		/// <summary>
		/// Removes specified property.
		/// </summary>
		public bool Remove(string name)
		{
			if (name == null)
				throw new ArgumentException("Property name cannot be null.");

			var key = ConvertName(name);
			return m_values.Remove(key);
		}

		/// <summary>
		/// Removes all properties.
		/// </summary>
		public void Clear()
		{
			m_values.Clear();
		}
	}
}
