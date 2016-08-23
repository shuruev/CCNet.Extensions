using System;
using System.Collections.Generic;
using System.Linq;
using Atom.Toolbox;

namespace NetBuild.ConsoleApp
{
	/// <summary>
	/// Works with command line arguments.
	/// Expected arguments represent unnamed properties which can be specified at the beginning, before named properties.
	/// </summary>
	public class ConsoleArguments : MemoryConfigReader
	{
		private readonly string[] m_expected;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ConsoleArguments(params string[] expected)
		{
			if (expected == null)
			{
				m_expected = new string[0];
				return;
			}

			if (expected.Any(String.IsNullOrWhiteSpace))
				throw new ArgumentException("Expected arguments should not contain null or whitespace elements.");

			if (expected.GroupBy(ConvertName, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1))
				throw new ArgumentException("Expected arguments should not contain duplicate elements.");

			m_expected = expected;
		}

		/// <summary>
		/// Gets all expected arguments.
		/// </summary>
		public List<string> Expected => m_expected.ToList();

		/// <summary>
		/// Parses specified console arguments, completely replacing all internal properies.
		/// </summary>
		public void Parse(IEnumerable<string> args)
		{
			Clear();

			if (args == null)
				return;

			var input = args.ToList();
			if (input.Any(arg => arg == null))
				throw new ArgumentException("Input arguments should not contain null elements.");

			// look through expected arguments
			foreach (var arg in m_expected)
			{
				if (input.Count == 0)
					return;

				if (input[0].StartsWith("/") || input[0].StartsWith("-"))
					break;

				Add(arg, input[0]);
				input.RemoveAt(0);
			}

			// look through remaining '/...' or '-...' arguments
			foreach (var arg in input)
			{
				Parse(arg);
			}
		}

		/// <summary>
		/// Parses specified value as one of the arguments.
		/// </summary>
		private void Parse(string arg)
		{
			if (arg.StartsWith("/") && arg.Contains(":"))
			{
				var parts = arg.Substring(1).Split(new[] { ':' }, 2);
				var name = parts[0];
				var value = parts[1];

				Add(name, value);
				return;
			}

			if (arg.StartsWith("-"))
			{
				var name = arg.Substring(1);
				Add(name, true.ToString());
				return;
			}

			throw new ArgumentException($"Cannot parse property name from specified value '{arg}'.");
		}
	}
}
