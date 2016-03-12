using System;

namespace CCNet.Build.CheckProject
{
	public class CheckContextService<T>
	{
		private readonly Func<T> m_loader;
		private Tuple<T> m_result;

		public CheckContextService(Func<T> loader)
		{
			if (loader == null)
				throw new ArgumentNullException("loader");

			m_loader = loader;
			m_result = null;
		}

		public T Result
		{
			get
			{
				if (m_result != null)
					return m_result.Item1;

				var result = m_loader.Invoke();
				m_result = new Tuple<T>(result);
				return result;
			}
		}
	}
}
