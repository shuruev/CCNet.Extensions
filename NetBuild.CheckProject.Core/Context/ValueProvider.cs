using System;

namespace NetBuild.CheckProject
{
	/// <summary>
	/// Provides a single value to be used within consistency checks.
	/// </summary>
	public abstract class ValueProvider<T> : IContextProvider
	{
		/// <summary>
		/// Gets context value.
		/// </summary>
		public T Value { get; private set; }

		/// <summary>
		/// Loads required data.
		/// </summary>
		public void Load(CheckContext context)
		{
			Value = Get(context);

			if (context.Args.DebugMode)
				Console.WriteLine($"{GetType().Name} = {Value}");
		}

		/// <summary>
		/// Gets context value.
		/// </summary>
		public abstract T Get(CheckContext context);
	}
}
