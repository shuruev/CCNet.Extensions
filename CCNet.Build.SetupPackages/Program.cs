using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Execute.DisplayUsage("Restores and updates all referenced packages to their most recent versions.", typeof(Args));
				return 0;
			}

			try
			{
				Args.Current = new ArgumentProperties(args);
				Execute.DisplayCurrent(typeof(Args));

				SetupPackages();
			}
			catch (Exception e)
			{
				return Execute.RuntimeError(e);
			}

			return 0;
		}

		private static void SetupPackages()
		{
			var checker = new PackageChecker(Config.NuGetDbConnection, Args.CustomVersions);

			using (Execute.Step("INIT"))
			{
				Console.Write("Loading local packages... ");
				checker.Load();
				Console.WriteLine("{0} found.", checker.PackageCount);
			}

			var log = new LogPackages();
			var references = new ReferencesHelper(checker);
			var packages = new PackagesHelper(checker, log);
			var nuget = new NuGetHelper(checker);

			using (Execute.Step("PRE ADJUST"))
			{
				references.PreAdjust();
				packages.PreAdjust();
			}

			using (Execute.Step("RESTORE & UPDATE"))
			{
				nuget.RestoreAll();
				nuget.UpdateAll();
			}

			using (Execute.Step("POST ADJUST"))
			{
				references.PostAdjust();
				packages.PostAdjust();
			}

			using (Execute.Step("REPORT & SAVE"))
			{
				foreach (var reference in log.Values.OrderBy(i => i.Location).ThenBy(i => i.Name))
				{
					reference.Report();
				}

				SaveReferences(log);
			}
		}

		private static void SaveReferences(LogPackages log)
		{
			Console.Write("Saving local references... ");

			if (!Directory.Exists(Args.ReferencesPath))
			{
				Directory.CreateDirectory(Args.ReferencesPath);
			}

			var after = log.Values.Where(i => i.IsLocal).ToDictionary(i => i.Name);
			var before = GetReferences().ToDictionary(Path.GetFileNameWithoutExtension);

			var toAdd = new List<string>();
			foreach (var reference in after)
			{
				if (before.ContainsKey(reference.Key))
					continue;

				var name = String.Format("{0}.txt", reference.Key);
				var path = Path.Combine(Args.ReferencesPath, name);
				toAdd.Add(path);
			}

			var toRemove = new List<string>();
			foreach (var reference in before)
			{
				if (after.ContainsKey(reference.Key))
					continue;

				toRemove.Add(reference.Value);
			}

			foreach (var file in toRemove)
			{
				File.Delete(file);
			}

			foreach (var file in toAdd)
			{
				File.WriteAllText(file, DateTime.Now.ToDetailedString());
			}

			Console.WriteLine("OK");
		}

		private static List<string> GetReferences()
		{
			return Directory.GetFiles(Args.ReferencesPath).ToList();
		}
	}
}
