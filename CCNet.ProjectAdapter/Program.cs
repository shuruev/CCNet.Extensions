using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CCNet.Common;
using CCNet.ProjectAdapter.Properties;

namespace CCNet.ProjectAdapter
{
	/// <summary>
	/// Updates project during build process.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// Main program.
		/// </summary>
		public static int Main(string[] args)
		{
			/*xxxargs = new[]
			{
				@"ProjectName=CustomerExtranet",
				@"CurrentVersion=1.2.3.4",
				@"WorkingDirectorySource=\\rufrt-vxbuild\d$\CCNET\CustomerExtranet\WorkingDirectory\Source",
				@"WorkingDirectoryRelated=\\rufrt-vxbuild\d$\CCNET\CustomerExtranet\WorkingDirectory\Related",
				@"ExternalReferencesPath=\\rufrt-vxbuild\ExternalReferences",
				@"InternalReferencesPath=\\rufrt-vxbuild\InternalReferences",
				@"PinnedReferencesPath=\\rufrt-vxbuild\PinnedReferences",
				@"ProjectType=WebSite",
				@"UsePinned=Extranet 2012-01"
			};*/

			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return 0;
			}

			try
			{
				Arguments.Default = ArgumentProperties.Parse(args);

				CancelReadonly(Arguments.WorkingDirectorySource);
				CancelReadonly(Arguments.WorkingDirectoryRelated);

				UpdateAssemblyInfo();
				UpdateProjectProperties();
				UpdateBinaryReferences();
				UpdateProjectReferences();
			}
			catch (Exception e)
			{
				return ErrorHandler.Runtime(e);
			}

			return 0;
		}

		/// <summary>
		/// Displays usage text.
		/// </summary>
		private static void DisplayUsage()
		{
			Console.WriteLine();
			Console.WriteLine(Resources.UsageInfo);
			Console.WriteLine();
		}

		#region Performing update

		/// <summary>
		/// Cancels readonly flag for all files in specified directory.
		/// </summary>
		private static void CancelReadonly(string path)
		{
			if (!Directory.Exists(path))
				return;

			DirectoryInfo dir = new DirectoryInfo(path);
			foreach (FileInfo fi in dir.GetFiles("*", SearchOption.AllDirectories))
			{
				fi.Attributes = FileAttributes.Normal;
			}
		}

		/// <summary>
		/// Updates assembly information file.
		/// </summary>
		private static void UpdateAssemblyInfo()
		{
			if (Arguments.ProjectType == ProjectType.Azure)
				return;

			string text = File.ReadAllText(Paths.AssemblyInfoFile);

			text = new Regex("\\[assembly: AssemblyVersion\\(\"[0-9\\.\\*]+\"\\)]")
				.Replace(text, "[assembly: AssemblyVersion(\"" + Arguments.CurrentVersion + "\")]");

			text = new Regex("\\[assembly: AssemblyFileVersion\\(\"[0-9\\.\\*]+\"\\)]")
				.Replace(text, "[assembly: AssemblyFileVersion(\"" + Arguments.CurrentVersion + "\")]");

			File.WriteAllText(Paths.AssemblyInfoFile, text, Encoding.UTF8);
		}

		/// <summary>
		/// Updates project properties.
		/// </summary>
		private static void UpdateProjectProperties()
		{
			string text = File.ReadAllText(Paths.ProjectFile);

			Regex regex = new Regex("<MinimumRequiredVersion>[0-9\\.\\*]+</MinimumRequiredVersion>");
			text = regex.Replace(text, "<MinimumRequiredVersion>" + Arguments.CurrentVersion + "</MinimumRequiredVersion>");

			regex = new Regex("<ApplicationVersion>[0-9\\.\\*]+</ApplicationVersion>");
			text = regex.Replace(text, "<ApplicationVersion>" + Arguments.CurrentVersion + "</ApplicationVersion>");

			text = text.Replace(
				"<GenerateManifests>false</GenerateManifests>",
				"<GenerateManifests>true</GenerateManifests>");

			text = text.Replace(
				"<Import Project=\"$(CloudExtensionsDir)Microsoft.CloudService.targets\" />",
				"<Import Project=\"$(CloudExtensionsDir)Microsoft.WindowsAzure.targets\" />");

			text = text.Replace(
				"<CloudExtensionsDir Condition=\" '$(CloudExtensionsDir)' == '' \">$(MSBuildExtensionsPath)\\Microsoft\\Cloud Service\\1.0\\Visual Studio 10.0\\</CloudExtensionsDir>",
				"<VisualStudioVersion Condition=\" '$(VisualStudioVersion)' == '' \">10.0</VisualStudioVersion>\r\n<CloudExtensionsDir Condition=\" '$(CloudExtensionsDir)' == '' \">$(MSBuildExtensionsPath)\\Microsoft\\VisualStudio\\v$(VisualStudioVersion)\\Windows Azure Tools\\1.5\\</CloudExtensionsDir>");

			File.WriteAllText(Paths.ProjectFile, text, Encoding.UTF8);
		}

		/// <summary>
		/// Updates binary references.
		/// </summary>
		private static void UpdateBinaryReferences()
		{
			UpdateBinaryReferences(
				Paths.ProjectFile,
				true);
		}

		/// <summary>
		/// Updates binary references for specified project.
		/// </summary>
		private static void UpdateBinaryReferences(
			string projectPath,
			bool reportReferences)
		{
			string text = File.ReadAllText(projectPath);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(text);

			XmlNamespaceManager xnm = new XmlNamespaceManager(doc.NameTable);
			xnm.AddNamespace("ms", "http://schemas.microsoft.com/developer/msbuild/2003");

			foreach (XmlNode node in doc.SelectNodes("/ms:Project/ms:ItemGroup/ms:Reference/ms:HintPath", xnm))
			{
				node.ParentNode.RemoveChild(node);
			}

			List<ReferenceFile> allExternals = ReferenceFolder.GetAllFiles(Arguments.ExternalReferencesPath);
			List<ReferenceFile> allInternals = ReferenceFolder.GetAllFiles(Arguments.InternalReferencesPath);

			UpdateHints(doc, xnm, allExternals, reportReferences);
			UpdateHints(doc, xnm, allInternals, reportReferences);

			using (XmlTextWriter xtw = new XmlTextWriter(projectPath, Encoding.UTF8))
			{
				xtw.Formatting = Formatting.Indented;
				doc.WriteTo(xtw);
			}
		}

		/// <summary>
		/// Updates hint paths for resolved references.
		/// </summary>
		private static void UpdateHints(
			XmlDocument doc,
			XmlNamespaceManager xnm,
			IEnumerable<ReferenceFile> references,
			bool reportReferences)
		{
			foreach (ReferenceFile reference in references)
			{
				XmlNode node = doc.SelectSingleNode(
					@"
						/ms:Project/ms:ItemGroup/ms:Reference[starts-with(@Include, '{0},')]
						| /ms:Project/ms:ItemGroup/ms:Reference[@Include = '{0}']
					"
					.Display(reference.AssemblyName),
					xnm);

				if (node == null)
					continue;

				XmlNode hint = node.SelectSingleNode("ms:HintPath", xnm);
				if (hint != null)
					node.RemoveChild(hint);

				// use pinned reference if needed
				bool pinned = false;
				string referencePath = reference.FilePath;
				if (!String.IsNullOrEmpty(Arguments.UsePinned))
				{
					string pinnedPath = Path.Combine(
						Paths.PinnedReferencesFolder,
						Path.GetFileName(referencePath));

					if (File.Exists(pinnedPath))
					{
						pinned = true;
						referencePath = pinnedPath;
					}
				}

				hint = doc.CreateElement("HintPath", xnm.LookupNamespace("ms"));
				hint.InnerXml = referencePath;

				node.AppendChild(hint);

				if (reportReferences)
				{
					Console.WriteLine(
						Resources.LogReferencesTo,
						reference.FileName,
						reference.ProjectName,
						pinned
							? String.Format("Pinned as {0}", Arguments.UsePinned)
							: reference.Version);
				}
			}
		}

		/// <summary>
		/// Updates project references.
		/// </summary>
		private static void UpdateProjectReferences()
		{
			string text = File.ReadAllText(Paths.ProjectFile);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(text);

			XmlNamespaceManager xnm = new XmlNamespaceManager(doc.NameTable);
			xnm.AddNamespace("ms", "http://schemas.microsoft.com/developer/msbuild/2003");

			foreach (XmlNode node in doc.SelectNodes("/ms:Project/ms:ItemGroup/ms:ProjectReference", xnm))
			{
				string include = node.Attributes["Include"].Value;
				include = include.Replace("..", Arguments.WorkingDirectoryRelated);
				node.Attributes["Include"].Value = include;

				UpdateBinaryReferences(include, false);

				string relatedProjectFile = Path.GetFileName(include);
				string relatedProjectName = Path.GetFileNameWithoutExtension(include);
				string relatedProjectVersion = ReferenceFolder.GetLatestVersion(
					Arguments.InternalReferencesPath,
					relatedProjectName);

				Console.WriteLine(
					Resources.LogReferencesTo,
					relatedProjectFile,
					relatedProjectName,
					relatedProjectVersion);
			}

			using (XmlTextWriter xtw = new XmlTextWriter(Paths.ProjectFile, Encoding.UTF8))
			{
				xtw.Formatting = Formatting.Indented;
				doc.WriteTo(xtw);
			}
		}

		#endregion
	}
}
