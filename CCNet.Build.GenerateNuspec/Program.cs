using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CCNet.Build.Common;

namespace CCNet.Build.GenerateNuspec
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Execute.DisplayUsage("Generates .nuspec file for specified project.", typeof(Args));
				return 0;
			}

			try
			{
				Args.Current = new ArgumentProperties(args);
				Execute.DisplayCurrent(typeof(Args));

				GenerateNuspec();
			}
			catch (Exception e)
			{
				return Execute.RuntimeError(e);
			}

			return 0;
		}

		private static void GenerateNuspec()
		{
			GenerateNuspecLibrary();
		}

		private static void GenerateNuspecLibrary()
		{
			Args.OutputDirectory.CreateDirectoryIfNotExists();

			using (var xtw = new XmlTextWriter(Paths.NuspecFile, Encoding.UTF8))
			{
				xtw.Formatting = Formatting.Indented;
				xtw.IndentChar = '\t';
				xtw.Indentation = 1;

				xtw.WriteStartDocument();
				xtw.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

				xtw.WriteStartElement("metadata");

				xtw.WriteElementString("id", Args.ProjectName);
				xtw.WriteElementString("version", Args.CurrentVersion);
				xtw.WriteElementString("authors", Args.CompanyName);
				xtw.WriteElementString("description", Args.ProjectDescription);
				xtw.WriteElementString("requireLicenseAcceptance", "false");

				AddReleaseNotes(xtw);

				xtw.WriteElementString("projectUrl", String.Format("https://owl.cbsi.com/confluence/display/CCSSEDRU/{0}+library", Args.ProjectName));
				xtw.WriteElementString("iconUrl", "https://owl.cbsi.com/confluence/download/attachments/12795231/CCSSEDRU");

				xtw.WriteEndElement();

				AddFiles(xtw);
				AddDependencies(xtw);

				xtw.WriteEndElement();
				xtw.WriteEndDocument();
			}
		}

		private static void AddReleaseNotes(XmlTextWriter xtw)
		{
			var sb = new StringBuilder();

			foreach (var file in Args.ReleaseNotes.Split('|'))
			{
				if (!File.Exists(file))
					continue;

				if (sb.Length > 0)
					sb.AppendLine();

				var text = File.ReadAllText(file).Trim();
				sb.AppendLine(text);
			}

			if (sb.Length == 0)
				return;

			xtw.WriteElementString("releaseNotes", sb.ToString());
		}

		private static void AddFiles(XmlTextWriter xtw)
		{
			xtw.WriteStartElement("files");

			AddNuspecLibraryCoreFile(xtw, "dll");
			AddNuspecLibraryCoreFile(xtw, "pdb");

			if (Args.IncludeXmlDocumentation)
				AddNuspecLibraryCoreFile(xtw, "xml");

			xtw.WriteEndElement();
		}

		private static void AddNuspecLibraryCoreFile(XmlTextWriter xtw, string extension)
		{
			var src = String.Format(@"{0}\{1}.{2}", Args.ReleasePath, Args.ProjectName, extension.ToLowerInvariant());
			var target = String.Format(@"lib\{0}", Args.TargetFramework.ToString().ToLowerInvariant());

			xtw.WriteStartElement("file");
			xtw.WriteAttributeString("src", src);
			xtw.WriteAttributeString("target", target);
			xtw.WriteEndElement();
		}

		private static void AddDependencies(XmlTextWriter xtw)
		{
			var dependencies = ParseDependencies(Args.Dependencies);
			if (dependencies.Count == 0)
				return;

			xtw.WriteStartElement("dependencies");
			xtw.WriteStartElement("group");

			foreach (var dependency in dependencies)
			{
				xtw.WriteStartElement("dependency");
				xtw.WriteAttributeString("id", dependency.Key);
				xtw.WriteAttributeString("version", dependency.Value.ToString());
				xtw.WriteEndElement();
			}

			xtw.WriteEndElement();
			xtw.WriteEndElement();
		}

		private static Dictionary<string, Version> ParseDependencies(string dependencies)
		{
			var result = new Dictionary<string, Version>();

			foreach (var item in dependencies.Split('|'))
			{
				if (!item.Contains('+'))
					throw new InvalidOperationException("Invalid syntax for dependencies string.");

				var parts = item.Split('+');
				var name = parts[0];
				var version = parts[1];
				result.Add(name, new Version(version));
			}

			return result;
		}
	}
}
