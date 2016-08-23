using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCNet.Build.Reconfigure
{
	public partial class ConfigurationBuilder
	{
		private class Arg
		{
			public string Name { get; }
			public string Value { get; }

			public Arg(string name, string value)
			{
				if (String.IsNullOrEmpty(name))
					throw new ArgumentNullException(nameof(name));

				if (String.IsNullOrEmpty(value))
					throw new ArgumentNullException(nameof(value));

				Name = name;
				Value = value;
			}
		}

		private void ExecTaskLegacy(string executable, string description, params Arg[] arguments)
		{
			using (Tag("exec"))
			{
				Tag("executable", executable);
				Tag("buildTimeoutSeconds", "45");

				var args = arguments.Where(arg => arg != null).ToList();
				if (args.Count > 0)
				{
					var sb = new StringBuilder();
					foreach (var arg in args)
					{
						var line = $"{arg.Name}={arg.Value}".Replace("\"", "\"\"");
						sb.AppendFormat("\r\n\t\t\t\t\t\"{0}\"", line);
					}

					sb.Append("\r\n\t\t\t\t");
					Tag("buildArgs", sb.ToString());
				}

				if (!String.IsNullOrEmpty(description))
				{
					Tag("description", description);
				}
			}
		}

		private void ExecTask(string executable, string description, params Arg[] arguments)
		{
			using (Tag("exec"))
			{
				Tag("executable", executable);
				Tag("buildTimeoutSeconds", "45");

				var args = arguments.Where(arg => arg != null).ToList();
				if (args.Count > 0)
				{
					var sb = new StringBuilder();
					foreach (var arg in args)
					{
						var line = $"/{arg.Name}:{arg.Value}";
						sb.Append($"\r\n\"{line.Replace("\"", "\"\"")}\"");
					}

					sb.Append("\r\n\t\t\t\t");
					Tag("buildArgs", sb.ToString());
				}

				if (!String.IsNullOrEmpty(description))
				{
					Tag("description", description);
				}
			}
		}

		private void WriteCheckProject(IProjectConfiguration config)
		{
			var check = config as ICheckProject;
			if (check == null)
				return;

			var args = new List<Arg>
			{
				new Arg("projectName", config.Name),
				new Arg("localPath", check.SourceDirectory()),
				new Arg("remotePath", check.TfsPath)
			};

			/*var company = "CNET Content Solutions";

			var assembly = config as IAssembly;
			if (assembly != null)
			{
				if (assembly.RootNamespace != null)
					args.Add(new Arg("RootNamespace", assembly.RootNamespace));

				if (assembly.CustomAssemblyName != null)
					args.Add(new Arg("AssemblyName", assembly.CustomAssemblyName));

				if (assembly.CustomCompanyName != null)
					company = assembly.CustomCompanyName;
			}

			args.Add(new Arg("CompanyName", company));*/

			var issues = new List<string>();

			issues.Add(F01_ProjectFileShouldExist);
			issues.Add(F02_AssemblyInfoShouldExist);

			issues.Add(S01_ProjectFolderShouldHaveProjectName);
			issues.Add(S02_PrimarySolutionShouldExist);
			issues.Add(S03_NugetFolderShouldNotExist);
			issues.Add(S04_PackagesFolderShouldNotExist);

			if (!String.IsNullOrWhiteSpace(check.CustomIssues))
			{
				var all = check.CustomIssues.Split('|');
				var force = all.Where(code => code.StartsWith("+")).Select(code => code.Substring(1)).ToList();
				var ignore = all.Where(code => code.StartsWith("+")).Select(code => code.Substring(1)).ToList();
				issues = issues.Union(force).Except(ignore).ToList();
			}

			args.Add(new Arg("issues", String.Join("|", issues)));

			ExecTask(
				"$(netBuildCheckProject)",
				"Check project",
				args.ToArray());
		}

		private void WritePrepareProject(IProjectConfiguration config)
		{
			var prepare = config as IPrepareProject;
			if (prepare == null)
				return;

			var args = new List<Arg>
			{
				new Arg("path", prepare.SourceDirectory()),
				new Arg("version", "$[$CCNetLabel]"),
				new Arg("tfs", prepare.TfsPath),
				new Arg("output", prepare.TempDirectory())
			};

			if (config is ICsProj)
			{
				args.Add(new Arg("updateAssemblyInfo", "true"));
			}

			ExecTask(
				"$(ccnetBuildPrepareProject)",
				"Prepare project",
				args.ToArray());
		}
	}
}
