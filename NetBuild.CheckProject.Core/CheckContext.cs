using System;
using System.Collections.Generic;
using Atom.Toolbox;
using NetBuild.ConsoleApp;

namespace NetBuild.CheckProject
{
	/// <summary>
	/// Context with all the data required to perform check.
	/// </summary>
	public class CheckContext
	{
		private readonly Dictionary<string, object> m_cached;

		/// <summary>
		/// Gets specified console arguments.
		/// </summary>
		public ConsoleArgs Args { get; }

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public CheckContext(ConsoleArgs args)
		{
			m_cached = new Dictionary<string, object>();

			Args = args;
		}

		/// <summary>
		/// Gets provider instance of a specified type.
		/// </summary>
		public T Of<T>() where T : IContextProvider, new()
		{
			var name = typeof(T).Name;
			if (m_cached.ContainsKey(name))
				return (T)m_cached[name];

			var context = (T)CreateContext<T>(name);
			if (typeof(T).FullName != context.GetType().FullName)
				Console.WriteLine($"Using {context.GetType().FullName} instead of {typeof(T).FullName}.");

			if (Args.DebugMode)
				Console.WriteLine($"{name} <-- {context.GetType().FullName}");

			context.Load(this);
			m_cached.Add(name, context);

			return context;
		}

		/// <summary>
		/// Creates provider instance of a specified type.
		/// </summary>
		private object CreateContext<T>(string name) where T : IContextProvider, new()
		{
			if (typeof(T).FullName != typeof(ConfigurationContext).FullName)
			{
				var config = Of<ConfigurationContext>();
				var key = $"Replace-{name}";

				if (!config.IsNull(key))
				{
					var replace = config.Get<string>(key);
					var type = Type.GetType(replace);
					if (type == null)
						throw new InvalidOperationException($"Cannot load type '{replace}'.");

					return Activator.CreateInstance(type);
				}
			}

			return new T();
		}
	}
}
