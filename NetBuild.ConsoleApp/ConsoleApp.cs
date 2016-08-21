using System;
using System.Collections.Generic;
using System.Linq;

namespace NetBuild.ConsoleApp
{
	/// <summary>
	/// Typical console application.
	/// </summary>
	public abstract class ConsoleApp<T> where T : ConsoleArgs
	{
		protected T m_args;

		private ArgumentsUsage<T> m_usage;
		private AppInfo m_info;

		public int Run(T args)
		{
			m_args = args;

			m_usage = new ArgumentsUsage<T>(args);
			m_info = new AppInfo();

			if (m_args.Names.Count == 0)
			{
				DisplayUsage();
				return 0;
			}

			try
			{
				DisplayCurrent();
				Run();
			}
			catch (Exception e)
			{
				return RuntimeError(e);
			}

			return 0;
		}

		protected abstract void Run();

		/// <summary>
		/// Displays usage text.
		/// </summary>
		private void DisplayUsage()
		{
			Console.WriteLine($"{m_info.Title} ver. {m_info.Version}");
			Console.WriteLine($"{m_info.Copyright}".Replace("©", "(c)"));
			Console.WriteLine();
			Console.WriteLine(m_info.Description);
			Console.WriteLine();
			Console.WriteLine($"{m_info.Executable} <args> [<props>...] [<flags>...]");
			Console.WriteLine();
			Console.WriteLine("  <args>   argument1 argument2...");
			Console.WriteLine("  <props>  /property1:Value1 /property2:Value2...");
			Console.WriteLine("  <flags>  -flag1 -flag2...");

			DisplayArguments();
			DisplayProperties();
			DisplayFlags();

			Console.WriteLine();
		}

		/// <summary>
		/// Displays expected arguments.
		/// </summary>
		private void DisplayArguments()
		{
			var items = m_args.Expected
				.Select(arg => new UsageItem
				{
					Name = arg,
					Description = m_usage.GetDescription(arg),
					Required = m_usage.GetRequired(arg)
				})
				.ToList();

			DisplayUsages("Arguments", "  <{0}>", items);
		}

		/// <summary>
		/// Displays known properties.
		/// </summary>
		private void DisplayProperties()
		{
			var items = m_usage.GetProperties()
				.Except(m_args.Expected)
				.OrderBy(prop => prop)
				.Select(prop => new UsageItem
				{
					Name = prop,
					Description = m_usage.GetDescription(prop),
					Required = m_usage.GetRequired(prop)
				})
				.ToList();

			DisplayUsages("Properties", "  /{0}:<...>", items);
		}

		/// <summary>
		/// Displays known flags.
		/// </summary>
		private void DisplayFlags()
		{
			var items = m_usage.GetFlags()
				.Except(m_args.Expected)
				.OrderBy(flag => flag)
				.Select(flag => new UsageItem
				{
					Name = flag,
					Description = m_usage.GetDescription(flag),
					Required = m_usage.GetRequired(flag)
				})
				.ToList();

			DisplayUsages("Flags", "  -{0}", items);
		}

		private class UsageItem
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public bool Required { get; set; }
		}

		private void DisplayUsages(string header, string pattern, List<UsageItem> items)
		{
			if (items.Count == 0)
				return;

			var max = items.Max(item => item.Name.Length);

			Console.WriteLine();
			Console.WriteLine($"{header}:");

			foreach (var item in items)
			{
				Console.Write(pattern, item.Name);
				Console.Write(new string(' ', max - item.Name.Length + 2));
				Console.Write(
					String.IsNullOrWhiteSpace(item.Description)
						? "???"
						: $"{item.Description}");

				if (!item.Required)
					Console.Write(" (optional)");

				Console.WriteLine();
			}
		}

		/// <summary>
		/// Displays more detailed information about current execution in debug mode.
		/// </summary>
		private void DisplayCurrent()
		{
			if (!m_args.DebugMode)
				return;

			Console.WriteLine(m_info.Executable);

			DisplayValues();
			Console.WriteLine();
		}

		/// <summary>
		/// Displays known arguments and their current values.
		/// </summary>
		private void DisplayValues()
		{
			var items = m_args.Names
				.OrderBy(name => name)
				.Select(name => new ValueItem
				{
					Name = name,
					Value = m_args.GetValue(name)
				})
				.ToList();

			DisplayValues(items);
		}

		private class ValueItem
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}

		private void DisplayValues(List<ValueItem> items)
		{
			if (items.Count == 0)
				return;

			var max = items.Max(item => item.Name.Length);

			Console.WriteLine();
			Console.WriteLine("Runtime:");

			foreach (var item in items)
			{
				Console.Write("  /{0}", item.Name);
				Console.Write(new string(' ', max - item.Name.Length + 2));
				Console.WriteLine(item.Value);
			}
		}

		/// <summary>
		/// Handles run-time error.
		/// </summary>
		private int RuntimeError(Exception error)
		{
			if (!m_args.DebugMode)
			{
				Console.Error.WriteLine();
				Console.Error.WriteLine(error.Message);
			}
			else
			{
				Console.Error.WriteLine(
					$@"
Running {m_info.Executable} has failed:
{error}
");
			}

			return -1;
		}
	}
}
