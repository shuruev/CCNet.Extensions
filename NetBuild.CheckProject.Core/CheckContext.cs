using System.Collections.Generic;

namespace NetBuild.CheckProject
{
	public class CheckContext
	{
		private readonly CheckArgs m_args;
		private readonly Dictionary<string, object> m_cached;

		public CheckContext(CheckArgs args)
		{
			m_args = args;
			m_cached = new Dictionary<string, object>();
		}

		public T Of<T>() where T : IContextProvider, new()
		{
			var name = typeof(T).FullName;
			if (m_cached.ContainsKey(name))
				return (T)m_cached[name];

			var context = new T();
			context.Load(this, m_args);
			m_cached.Add(name, context);
			return context;
		}
	}
}
