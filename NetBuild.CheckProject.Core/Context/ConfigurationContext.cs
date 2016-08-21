using System.Configuration;
using Atom.Toolbox;
using NetBuild.ConsoleApp;

namespace NetBuild.CheckProject
{
	/// <summary>
	/// Provides values from the specified console arguments or from the local application config.
	/// </summary>
	public class ConfigurationContext : IContextProvider, IConfigReader
	{
		private ConsoleArgs m_args;

		/// <summary>
		/// Loads required data.
		/// </summary>
		public void Load(CheckContext context)
		{
			m_args = context.Args;
		}

		/// <summary>
		/// Gets configuration value by specified name.
		/// Should return null if value does not exist.
		/// </summary>
		public string GetValue(string name)
		{
			// try reading value from the specified arguments
			if (!m_args.IsNull(name))
				return m_args.GetValue(name);

			// read value from local config
			return ConfigurationManager.AppSettings[name];
		}
	}
}
