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

				xtw.WriteElementString("id", Args.PackageId);
				xtw.WriteElementString("version", Args.CurrentVersion);

				if (Args.PackageId != Args.ProjectName)
				{
					xtw.WriteElementString("title", Args.ProjectName);
				}

				xtw.WriteElementString("authors", Args.CompanyName);
				xtw.WriteElementString("description", Args.ProjectDescription);
				xtw.WriteElementString("requireLicenseAcceptance", "false");

				AddDependencies(xtw);
				AddReleaseNotes(xtw);
				AddUrls(xtw);

				AddTags(xtw);

				xtw.WriteEndElement();

				AddFiles(xtw);

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

		private static void AddUrls(XmlTextWriter xtw)
		{
			var projectUrl = String.Format("https://owl.cbsi.com/confluence/display/CCSSEDRU/{0}+library", Args.ProjectName);
			var iconUrl = "https://owl.cbsi.com/confluence/download/attachments/12795232/cnet.png";

			if (Args.MarkAsCustom)
			{
				iconUrl = "https://owl.cbsi.com/confluence/download/attachments/12795232/cnet-bw.png";
			}

			xtw.WriteElementString("projectUrl", projectUrl);
			xtw.WriteElementString("iconUrl", iconUrl);
		}

		private static void AddTags(XmlTextWriter xtw)
		{
			var tags = new List<string>();

			if (Args.MarkAsCustom)
				tags.Add("custom");

			if (Args.MarkAsStatic)
				tags.Add("static");

			if (tags.Count == 0)
				return;

			xtw.WriteElementString("tags", String.Join(",", tags));
		}

		private static void AddFiles(XmlTextWriter xtw)
		{
			xtw.WriteStartElement("files");

			AddNuspecLibraryCoreFile(xtw, "dll");
			AddNuspecLibraryCoreFile(xtw, "pdb");

			if (Args.IncludeXmlDocumentation)
				AddNuspecLibraryCoreFile(xtw, "xml");

			AddNuspecLibrarySatelliteAssemblies(xtw);

			xtw.WriteEndElement();
		}

		private static void AddNuspecLibraryCoreFile(XmlTextWriter xtw, string extension)
		{
			var src = String.Format(@"{0}\{1}.{2}", Args.ReleasePath, Args.PackageId, extension.ToLowerInvariant());
			var target = String.Format(@"lib\{0}", Args.TargetFramework.ToString().ToLowerInvariant());

			xtw.WriteStartElement("file");
			xtw.WriteAttributeString("src", src);
			xtw.WriteAttributeString("target", target);
			xtw.WriteEndElement();
		}

		private static void AddNuspecLibrarySatelliteAssemblies(XmlTextWriter xtw)
		{
			var directories = Directory.GetDirectories(Args.ReleasePath);
			foreach (var dir in directories)
			{
				var locale = Path.GetFileName(dir);
				var src = String.Format(@"{0}\{1}.resources.dll", dir, Args.PackageId);
				var target = String.Format(@"lib\{0}\{1}", Args.TargetFramework.ToString().ToLowerInvariant(), locale);

				xtw.WriteStartElement("file");
				xtw.WriteAttributeString("src", src);
				xtw.WriteAttributeString("target", target);
				xtw.WriteEndElement();
			}
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

			if (String.IsNullOrEmpty(dependencies))
				return result;

			// xxx quick dirty workaround
			var packages = Args.ReleaseNotes.Split('|')
				.Where(File.Exists)
				.SelectMany(File.ReadLines)
				.Where(line => line.StartsWith("- "))
				.Select(line => line.Split(' '))
				.ToDictionary(parts => parts[1], parts => new Version(parts[2]));

			foreach (var item in dependencies.Split('|'))
			{
				if (!packages.ContainsKey(item))
					throw new InvalidOperationException(
						String.Format("Cannot find dependency '{0}' within used packages.", item));

				result.Add(item, packages[item]);
			}

			return result;
		}
	}
}
