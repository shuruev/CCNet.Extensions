using Atom.Toolbox;

namespace NetBuild.CheckProject
{
	/// <summary>
	/// Context value which is taken from console arguments or from local application config.
	/// </summary>
	public class ConfigurationValue<T> : ArgumentValue<T>
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ConfigurationValue(string name)
			: base(name)
		{
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ConfigurationValue(string name, T defaultValue)
			: base(name, defaultValue)
		{
		}

		/// <summary>
		/// Gets context value.
		/// </summary>
		public override T Get(CheckContext context)
		{
			var reader = context.Of<ConfigurationContext>();

			if (m_required)
				return reader.Get<T>(m_name);

			return reader.Get(m_name, m_default);
		}
	}
}
