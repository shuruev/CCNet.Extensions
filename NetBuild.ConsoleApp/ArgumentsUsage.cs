using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetBuild.ConsoleApp
{
	public class ArgumentsUsage<T> where T : ConsoleArgs
	{
		private readonly Dictionary<string, ArgumentAttribute> m_attrs;
		private readonly Dictionary<string, PropertyInfo> m_props;

		public ArgumentsUsage(T args)
		{
			var attrs = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.ToDictionary(
					pi => pi,
					pi => (ArgumentAttribute)pi.GetCustomAttributes(typeof(ArgumentAttribute)).FirstOrDefault());

			m_attrs = attrs.Values
				.Where(val => val != null)
				.ToDictionary(val => val.Name, val => val);

			m_props = attrs
				.Where(item => item.Value != null)
				.ToDictionary(item => item.Value.Name, item => item.Key);
		}

		public List<string> GetProperties()
		{
			return m_props
				.Where(item => item.Value.PropertyType != typeof(bool))
				.Select(item => item.Key)
				.ToList();
		}

		public List<string> GetFlags()
		{
			return m_props
				.Where(item => item.Value.PropertyType == typeof(bool))
				.Select(item => item.Key)
				.ToList();
		}

		public string GetDescription(string name)
		{
			if (!m_attrs.ContainsKey(name))
				return null;

			return m_attrs[name].Description;
		}

		public bool GetRequired(string name)
		{
			if (!m_attrs.ContainsKey(name))
				return false;

			return m_attrs[name].Required;
		}
	}
}
