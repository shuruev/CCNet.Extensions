using Atom.Toolbox;

namespace NetBuild.CheckProject
{
	/// <summary>
	/// Context value which is taken from console arguments only.
	/// </summary>
	public class ArgumentValue<T> : ValueProvider<T>
	{
		/// <summary>
		/// Indicates whether argument is required or optional.
		/// </summary>
		protected readonly bool m_required;

		/// <summary>
		/// Argument name.
		/// </summary>
		protected readonly string m_name;

		/// <summary>
		/// Default value for an optional argument.
		/// </summary>
		protected readonly T m_default;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ArgumentValue(string name)
		{
			m_required = true;
			m_name = name;
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ArgumentValue(string name, T defaultValue)
		{
			m_required = false;
			m_name = name;
			m_default = defaultValue;
		}

		/// <summary>
		/// Gets context value.
		/// </summary>
		public override T Get(CheckContext context)
		{
			var reader = context.Args;

			if (m_required)
				return reader.Get<T>(m_name);

			return reader.Get(m_name, m_default);
		}
	}
}
