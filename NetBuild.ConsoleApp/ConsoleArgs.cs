using System.Collections.Generic;
using Atom.Toolbox;

namespace NetBuild.ConsoleApp
{
	/// <summary>
	/// Contains typical predefined arguments for a console application.
	/// </summary>
	public class ConsoleArgs : ConsoleArguments
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ConsoleArgs(IEnumerable<string> input, params string[] expected)
			: base(expected)
		{
			Parse(input);
		}

		/// <summary>
		/// Gets a value indicating whether debug mode is enabled.
		/// </summary>
		[Optional("debug", "Enables debug output mode.")]
		public bool DebugMode => this.Get("debug", false);
	}
}
