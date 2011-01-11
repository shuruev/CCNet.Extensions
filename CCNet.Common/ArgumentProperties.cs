using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace CCNet.Common
{
	/// <summary>
	/// Works with command line properties.
	/// </summary>
	public class ArgumentProperties
	{
		private readonly Dictionary<string, string> m_values = new Dictionary<string, string>();

		#region Properties

		/// <summary>
		/// Gets all property keys.
		/// </summary>
		public List<string> Keys
		{
			get
			{
				return new List<string>(m_values.Keys);
			}
		}

		#endregion

		#region Parsing arguments list

		/// <summary>
		/// Adds a new property.
		/// </summary>
		public void Add(string key, string value)
		{
			Contract.Requires(!String.IsNullOrEmpty(key));
			Contract.Requires(value != null);

			m_values[key] = value;
		}

		/// <summary>
		/// Removes specified property.
		/// </summary>
		public void Remove(string key)
		{
			Contract.Requires(!String.IsNullOrEmpty(key));

			m_values.Remove(key);
		}

		/// <summary>
		/// Parses a list of argument properties.
		/// </summary>
		public static ArgumentProperties Parse(params string[] args)
		{
			Contract.Requires(args != null);

			ArgumentProperties result = new ArgumentProperties();
			foreach (string arg in args)
			{
				string[] parts = arg.Split('=');
				if (parts.Length != 2)
					throw new InvalidOperationException(
						"Argument {0} doesn't define a property."
						.Display(arg));

				string key = parts[0].Trim();
				string value = parts[1].Trim();
				result.Add(key, value);
			}

			return result;
		}

		#endregion

		#region Getting property values

		/// <summary>
		/// Checks whether specified property exists.
		/// </summary>
		public bool Contains(string key)
		{
			Contract.Requires(!String.IsNullOrEmpty(key));

			return m_values.ContainsKey(key);
		}

		/// <summary>
		/// Gets property value for specified key.
		/// </summary>
		public string GetValue(string key)
		{
			if (!Contains(key))
				throw new ArgumentException(
					"Property {0} is not specified."
					.Display(key));

			return m_values[key];
		}

		/// <summary>
		/// Reads enumeration value.
		/// </summary>
		public T GetEnumValue<T>(string key)
		{
			string value = GetValue(key);
			try
			{
				return (T)Enum.Parse(typeof(T), value);
			}
			catch
			{
				throw new ArgumentException(
					"Property {0} contains unknown value {1}."
					.Display(key, value));
			}
		}

		/// <summary>
		/// Gets boolean property value for specified key.
		/// </summary>
		public bool GetBooleanValue(string key)
		{
			string value = GetValue(key);
			return Boolean.Parse(value);
		}

		#endregion
	}
}
