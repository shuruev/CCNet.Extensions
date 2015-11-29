using System;
using System.Collections.Generic;
using System.Linq;
using Lean.Configuration;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Works with command line properties.
	/// </summary>
	public class ArgumentProperties : IConfigReader
	{
		private readonly Dictionary<string, string> m_values = new Dictionary<string, string>();

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ArgumentProperties()
		{
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ArgumentProperties(params string[] args)
		{
			Parse(args);
		}

		/// <summary>
		/// Gets all property keys.
		/// </summary>
		public List<string> Keys
		{
			get
			{
				return m_values.Keys.ToList();
			}
		}

		/// <summary>
		/// Gets configuration value by specified key.
		/// Returns null if value does not exist.
		/// </summary>
		public string GetValue(string key)
		{
			if (!Contains(key))
				return null;

			return m_values[key];
		}

		/// <summary>
		/// Adds a new property.
		/// </summary>
		public void Add(string key, string value)
		{
			if (String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException("key");

			m_values[key] = value;
		}

		/// <summary>
		/// Checks whether specified property exists.
		/// </summary>
		public bool Contains(string key)
		{
			return m_values.ContainsKey(key);
		}

		/// <summary>
		/// Removes specified property.
		/// </summary>
		public void Remove(string key)
		{
			m_values.Remove(key);
		}

		/// <summary>
		/// Parses a list of argument properties.
		/// </summary>
		public void Parse(IEnumerable<string> arguments)
		{
			if (arguments == null)
				throw new ArgumentNullException("arguments");

			m_values.Clear();
			foreach (var argument in arguments)
			{
				var parts = argument.Split(new[] { '=' }, 2);
				if (parts.Length != 2)
					throw new InvalidOperationException(
						String.Format("Argument '{0}' should define a property using 'Property1=Value1' format.", argument));

				var key = parts[0].Trim();
				var value = parts[1].Trim();
				if (String.IsNullOrEmpty(value))
					value = null;

				m_values.Add(key, value);
			}
		}
	}
}
