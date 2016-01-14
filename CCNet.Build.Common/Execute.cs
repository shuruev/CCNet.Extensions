using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Helper methods for executing command line tools.
	/// </summary>
	public static class Execute
	{
		/// <summary>
		/// Displays usage text.
		/// </summary>
		public static void DisplayUsage(string description, Type argsType)
		{
			Console.WriteLine();
			Console.WriteLine(description);
			Console.WriteLine();
			Console.WriteLine("{0} [Property1=Value1] [Property2=Value2] ...", Path.GetFileName(Assembly.GetEntryAssembly().Location));

			ReportArguments(argsType, false);
			Console.WriteLine();
		}

		/// <summary>
		/// Displays current execution.
		/// </summary>
		public static void DisplayCurrent(Type argsType)
		{
			Console.WriteLine();
			Console.WriteLine(Path.GetFileName(Assembly.GetEntryAssembly().Location));

			ReportArguments(argsType, true);
			Console.WriteLine();
		}

		/// <summary>
		/// Reports known arguments and their current values, if requested.
		/// </summary>
		public static void ReportArguments(Type argsType, bool displayValues)
		{
			if (argsType == null)
				return;

			var props = argsType.GetProperties(BindingFlags.Public | BindingFlags.Static)
				.Where(pi => pi.PropertyType != typeof(ArgumentProperties))
				.OrderBy(pi => pi.Name)
				.ToList();

			if (props.Count > 0)
			{
				Console.WriteLine();
				Console.WriteLine("Arguments:");

				foreach (var prop in props)
				{
					var name = prop.Name;
					Console.WriteLine("\t{0}", name);

					if (displayValues)
					{
						var value = prop.GetValue(null);
						Console.WriteLine("\t\t{0}", value);
					}
				}
			}
		}

		/// <summary>
		/// Handles run-time error.
		/// </summary>
		public static int RuntimeError(Exception error)
		{
			Console.Error.WriteLine(
				@"
Running {0} has failed:
{1}

Please contact build adminstrator for more information.
",
				Assembly.GetEntryAssembly().GetName().Name,
				error);

			return -1;
		}
	}
}
