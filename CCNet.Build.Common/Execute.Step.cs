using System;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Reporting simple command line steps.
	/// </summary>
	public static partial class Execute
	{
		/// <summary>
		/// Wraps step console output for better syntax.
		/// </summary>
		public class ExecuteStep : IDisposable
		{
			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			public ExecuteStep(string name)
			{
				if (String.IsNullOrEmpty(name))
					throw new ArgumentNullException("name");

				if (name != name.ToUpperInvariant())
					throw new ArgumentException("Name should be uppercased.");

				Console.WriteLine();
				Console.WriteLine("*** {0} ***", name);
				Console.WriteLine();
			}

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				Console.WriteLine();
			}
		}

		/// <summary>
		/// Marks execution step for console output.
		/// </summary>
		public static ExecuteStep Step(string name)
		{
			return new ExecuteStep(name);
		}
	}
}
