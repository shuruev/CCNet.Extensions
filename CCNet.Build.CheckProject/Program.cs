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
                               
If you beleive everything is correct please talk to Oleg Shuruev to improve this check.
                               
                               
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
				case "F06": return new PackagesFolderShouldNotHavePackages();
				case "F07": return new LocalFilesShouldMatchProjectFiles();

				// file contents
				case "C01": return new AllFilesShouldUseUtf8();
				case "C02": return new CheckAssemblyInfo();
				case "C03": return new CheckPrimarySolution();
				case "C04": return new CheckNugetConfig();

				// project properties
				case "P01": return new CheckProjectConfigurations();
				case "P02": return new CheckProjectPlatforms();
				case "P03": return new CheckProjectSourceControl();
				case "P04": return new ProjectOutputTypeLibrary();
				case "P05": return new ProjectOutputTypeExe();
				case "P06": return new ProjectOutputTypeWinExe();
				case "P07": return new CheckProjectAssemblyName();
				case "P08": return new CheckProjectRootNamespace();
				case "P09": return new CheckProjectStartupObject();
				case "P10": return new ProjectTargetFramework20();
				case "P11": return new ProjectTargetFramework35();
				case "P12": return new ProjectTargetFramework40();
				case "P13": return new ProjectTargetFramework45();
				case "P14": return new CheckProjectCompilation();
				case "P15": return new ProjectDocumentationNone();
				case "P16": return new ProjectDocumentationPartial();
				case "P17": return new ProjectDocumentationFull();

				// project items
				case "I01": return new ShouldHaveAppConfig();
				case "I02": return new ShouldHaveAppConfigDefault();
				case "I03": return new ShouldHaveWebConfig();
				case "I04": return new ShouldHaveWebConfigDefault();

				default:
					throw new InvalidOperationException(
						String.Format("Unknown issue '{0}'.", issue));
			}
		}
	}
}
