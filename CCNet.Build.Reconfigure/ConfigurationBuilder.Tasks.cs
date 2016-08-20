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

		private void WriteCheckProject(IProjectConfiguration config)
		{
			var tfs = config as ITfsPath;
			if (tfs == null)
				return;

			var args = new List<Arg>
			{
				new Arg("ProjectName", config.Name),
				new Arg("ProjectPath", tfs.SourceDirectory()),
				new Arg("TfsPath", tfs.TfsPath)
			};

			var company = "CNET Content Solutions";

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

			args.Add(new Arg("CompanyName", company));

			ExecTask(
				"$(ccnetBuildCheckProject)",
				"Check project",
				args.ToArray());

			/*xxxusing (Tag("exec"))
			{
				args.Add(new Arg("CheckIssues", project.CheckIssues));

				writer.WriteBuildArgs(args.ToArray());
				writer.WriteElementString("description", );
			}*/
		}
	}
}
