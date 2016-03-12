using System;
using CCNet.Build.Common;
using CCNet.Build.Tfs;

namespace CCNet.Build.CheckProject
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Execute.DisplayUsage("Performs specified checks over a source code project.", typeof(Args));
				return 0;
			}

			try
			{
				Args.Current = new ArgumentProperties(args);
				Execute.DisplayCurrent(typeof(Args));

				CheckProject();
			}
			catch (Exception e)
			{
				return Execute.RuntimeError(e);
			}

			return 0;
		}

		private static void CheckProject()
		{
			var tfs = new TfsClient(Config.TfsUrl);
			var context = new CheckContext(tfs);

			foreach (var issue in Args.CheckIssues.Split('|'))
			{
				var checker = GetChecker(issue);

				try
				{
					checker.Check(context);
				}
				catch (FailedCheckException e)
				{
					throw new InvalidOperationException(
						String.Format(
							@"
                               
                               
*** FAILED CHECK {0} // {1} ***
                               
{2}
                               
                               
",
							issue,
							checker.GetType().Name,
							e.Message),
						e);
				}

				Console.WriteLine("{0}... OK", issue);
			}
		}

		private static IChecker GetChecker(string issue)
		{
			switch (issue)
			{
				// file structure
				case "F01": return new ProjectFolderShouldHaveProjectName();
				case "F02": return new ProjectFileShouldExist();
				case "F03": return new AssemblyInfoShouldExist();
				case "F04": return new PrimarySolutionShouldExist();
				case "F05": return new NugetConfigShouldExist();

				// file contents
				case "C01": return new CheckAssemblyInfo();
				case "C02": return new CheckPrimarySolution();
				case "C03": return new CheckNugetConfig();

				default:
					throw new InvalidOperationException(
						String.Format("Unknown issue '{0}'.", issue));
			}
		}
	}
}
