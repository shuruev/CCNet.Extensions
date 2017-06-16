using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CCNet.Build.Reconfigure
{
	public partial class ConfigurationBuilder
	{
		internal const string F01_ProjectFileShouldExist = "F01";
		internal const string F02_AssemblyInfoShouldExist = "F02";

		internal const string S01_ProjectFolderShouldHaveProjectName = "S01";
		internal const string S02_PrimarySolutionShouldExist = "S02";
		internal const string S03_NugetFolderShouldNotExist = "S03";
		internal const string S04_PackagesFolderShouldNotExist = "S04";

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
			ExecTaskLegacy(executable, description, TimeSpan.FromSeconds(45), arguments);
		}

		private void ExecTaskLegacy(string executable, string description, TimeSpan timeout, params Arg[] arguments)
		{
			using (Tag("exec"))
			{
				Tag("executable", executable);
				Tag("buildTimeoutSeconds", Convert.ToInt32(timeout.TotalSeconds).ToString());

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

				if (arguments.Length > 0)
				{
					var sb = new StringBuilder();
					foreach (var arg in arguments)
					{
						var line = $"/{arg.Name}:{arg.Value}";
						sb.Append($"\r\n\t\t\t\t\t\"{line.Replace("\"", "\"\"")}\"");
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

		private void AzureUpload(IProjectConfiguration config, string container, string localFile, bool includeVersion = true)
		{
			var fileName = Path.GetFileName(localFile);

			var blobFile = includeVersion
				? $"{config.UniqueName()}/$[$CCNetLabel]/{fileName}"
				: $"{config.UniqueName()}/{fileName}";

			ExecTaskLegacy(
				"$(ccnetBuildAzureUpload)",
				$@"Upload ""{fileName}"" to ""{container}""",
				new Arg("Storage", "Devbuild"),
				new Arg("Container", container),
				new Arg("LocalFile", localFile),
				new Arg("BlobFile", blobFile));
		}
	}
}
