﻿using System;
using System.Collections.Concurrent;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// Caches data.
	/// </summary>
	public class Cache<TKey, TValue> where TKey : IEquatable<TKey>
	{
		/// <summary>
		/// Private data holder.
		/// </summary>
		private readonly ConcurrentDictionary<TKey, TValue> m_data = new ConcurrentDictionary<TKey, TValue>();

		/// <summary>
		/// Private lock object.
		/// </summary>
		private readonly object m_locker = new object();

		/// <summary>
		/// Gets a value for a given key, calling the creator() when needed.
		/// </summary>
		public TValue Get(TKey key, Func<TValue> creator)
		{
			if (!m_data.ContainsKey(key))
			{
				lock (m_locker)
				{
					if (!m_data.ContainsKey(key))
					{
						m_data[key] = creator();
					}
				}
			}

			return m_data[key];
		}
	}
}
