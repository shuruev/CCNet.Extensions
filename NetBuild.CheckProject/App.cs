using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetBuild.ConsoleApp;

namespace NetBuild.CheckProject
{
	public class App : ConsoleApp<Args>
	{
		private CheckContext m_context;
		private Dictionary<string, ICheckIssue> m_checkers;

		protected override void Run()
		{
			m_context = new CheckContext(m_args);

			LoadCheckers();

			if (m_args.CheckIssues.Length == 0)
			{
				Console.WriteLine("No issues to check.");
				return;
			}

			foreach (var issue in m_args.CheckIssues)
			{
				if (!m_checkers.ContainsKey(issue))
					throw new InvalidOperationException($"Unknown issue '{issue}'.");

				var checker = m_checkers[issue];

				try
				{
					checker.Check(m_context, m_args);
				}
				catch (CheckException e)
				{
					throw new InvalidOperationException(
						$@"
                               
                               
*** FAILED CHECK {issue} // {checker.GetType().Name} ***
                               
{e.Message}
                               
                               
",
						e);
				}

				Console.WriteLine($"{issue} // {checker.GetType().FullName} > OK");
			}
		}

		private void LoadCheckers()
		{
			if (m_args.DebugMode)
				Console.WriteLine("Loading checking modules...");

			var assembly = typeof(App).Assembly;
			if (!String.IsNullOrEmpty(m_args.DebugAssembly))
				assembly = Assembly.LoadFrom(m_args.DebugAssembly);

			var checkers = assembly.GetTypes()
				.Where(type => type.IsClass)
				.Where(type => typeof(ICheckIssue).IsAssignableFrom(type))
				.ToList();

			m_checkers = new Dictionary<string, ICheckIssue>();
			foreach (var type in checkers)
			{
				var checker = (ICheckIssue)Activator.CreateInstance(type);

				if (String.IsNullOrEmpty(checker.Issue))
					throw new InvalidOperationException($"Checking module '{checker.GetType().FullName}' does not specify issue code.");

				if (m_checkers.ContainsKey(checker.Issue))
				{
					throw new InvalidOperationException(
						$"Issue code '{checker.Issue}' is duplicated by modules" +
						$" '{checker.GetType().FullName}' and '{m_checkers[checker.Issue].GetType().FullName}'.");
				}

				m_checkers.Add(checker.Issue, checker);

				if (m_args.DebugMode)
					Console.WriteLine($"{checker.Issue} // {checker.GetType().FullName}");
			}

			if (m_args.DebugMode)
			{
				Console.WriteLine($"{checkers.Count} checking module(s) found.");
				Console.WriteLine();
			}
		}
	}
}
