using System;
using System.IO;
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
			switch (Args.PackageType)
			{
				case PackageType.Library:
					GenerateNuspecLibrary();
					break;

				default:
					throw new InvalidOperationException(
						String.Format("Unknown package type '{0}'.", Args.PackageType));
			}
		}

		private static void GenerateNuspecLibrary()
		{
			PrepareDirectory(Paths.NuspecFile);
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
				xtw.WriteEndElement();

				xtw.WriteStartElement("files");

				AddNuspecLibraryCoreFile(xtw, "dll");
				AddNuspecLibraryCoreFile(xtw, "pdb");

				if (Args.IncludeXmlDocumentation)
					AddNuspecLibraryCoreFile(xtw, "xml");

				xtw.WriteEndElement();

				xtw.WriteEndElement();
				xtw.WriteEndDocument();
			}
		}

		private static void AddNuspecLibraryCoreFile(XmlTextWriter xtw, string extension)
		{
			xtw.WriteStartElement("file");
			xtw.WriteAttributeString("src", String.Format(@"..\release\{0}.{1}", Args.ProjectName, extension.ToLowerInvariant()));
			xtw.WriteAttributeString("target", String.Format(@"lib\{0}", Args.TargetFramework.ToString().ToLowerInvariant()));
			xtw.WriteEndElement();
		}

		private static void PrepareDirectory(string filePath)
		{
			var dir = Path.GetDirectoryName(filePath);
			if (Directory.Exists(dir))
				return;

			Directory.CreateDirectory(dir);
		}
	}
}
