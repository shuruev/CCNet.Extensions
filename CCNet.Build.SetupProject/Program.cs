using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CCNet.Build.Common;
using CCNet.Build.SetupPackages;
using CCNet.Build.Tfs;

namespace CCNet.Build.SetupProject
{
	public static partial class Program
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Execute.DisplayUsage("Makes required changes to a project file and source code.", typeof(Args));
				return 0;
			}

			try
			{
				Args.Current = new ArgumentProperties(args);
				Execute.DisplayCurrent(typeof(Args));

				SetupProject();
			}
			catch (Exception e)
			{
				return Execute.RuntimeError(e);
			}

			return 0;
		}

		private static void SetupProject()
		{
			SaveSource();
			SaveVersion();
			RenderLinks();
			UpdateAssemblyInfo();
			UpdateProjectProperties();
			SetupRelatedProjects();
		}

		private static void SaveSource()
		{
			Console.Write("Saving TFS location... ");

			Args.TempPath.CreateDirectoryIfNotExists();

			var client = new TfsClient(Config.TfsUrl);
			var changeset = client.GetLatestChangeset(Args.TfsPath);

			var sb = new StringBuilder();
			sb.AppendLine("Source:");
			sb.AppendLine(Args.TfsPath);
			sb.AppendFormat("Changeset #{0}", changeset.Id).AppendLine();
			sb.AppendFormat("User: {0}", changeset.UserDisplay).AppendLine();
			sb.AppendFormat("Date: {0}", changeset.Date.ToDetailedString()).AppendLine();

			var file = Path.Combine(Args.TempPath, "source.txt");
			File.WriteAllText(file, sb.ToString());

			Console.WriteLine("OK");
		}

		private static void SaveVersion()
		{
			Console.Write("Saving current version... ");

			Args.TempPath.CreateDirectoryIfNotExists();

			var file = Path.Combine(Args.TempPath, "version.txt");
			File.WriteAllText(file, Args.CurrentVersion);

			Console.WriteLine("OK");
		}

		private static void UpdateAssemblyInfo()
		{
			if (Args.ProjectType == ProjectType.CloudService
				|| Args.ProjectType == ProjectType.FabricApplication)
				return;

			Console.Write("Updating assembly information... ");

			var version = new Version(Args.CurrentVersion).Normalize();
			var text = File.ReadAllText(Paths.AssemblyInfoFile);

			text = new Regex(@"^\[assembly: AssemblyVersion\(""[0-9\.?]+""\)]", RegexOptions.Multiline)
				.Replace(text, String.Format("[assembly: AssemblyVersion(\"{0}\")]", version));

			text = new Regex(@"^\[assembly: AssemblyFileVersion\(""[0-9\.?]+""\)]", RegexOptions.Multiline)
				.Replace(text, String.Format("[assembly: AssemblyFileVersion(\"{0}\")]", version));

			File.WriteAllText(Paths.AssemblyInfoFile, text, Encoding.UTF8);
			Console.WriteLine("OK");
		}

		private static void UpdateProjectProperties()
		{
			if (Args.ProjectType != ProjectType.Windows)
				return;

			Console.Write("Updating project properties... ");

			string text = File.ReadAllText(Paths.WindowsProjectFile);

			Regex regex = new Regex("<MinimumRequiredVersion>[0-9\\.\\*]+</MinimumRequiredVersion>");
			text = regex.Replace(text, "<MinimumRequiredVersion>" + Args.CurrentVersion + "</MinimumRequiredVersion>");

			regex = new Regex("<ApplicationVersion>[0-9\\.\\*]+</ApplicationVersion>");
			text = regex.Replace(text, "<ApplicationVersion>" + Args.CurrentVersion + "</ApplicationVersion>");

			text = text.Replace(
				"<GenerateManifests>false</GenerateManifests>",
				"<GenerateManifests>true</GenerateManifests>");

			File.WriteAllText(Paths.WindowsProjectFile, text, Encoding.UTF8);
			Console.WriteLine("OK");
		}

		private static void SetupRelatedProjects()
		{
			string path;
			switch (Args.ProjectType)
			{
				case ProjectType.CloudService:
					path = Paths.CloudProjectFile;
					break;

				case ProjectType.FabricApplication:
					path = Paths.FabricProjectFile;
					break;

				default:
					return;
			}

			if (String.IsNullOrEmpty(Args.RelatedPath))
				throw new InvalidOperationException("Configuration argument 'RelatedPath' is not set.");

			if (String.IsNullOrEmpty(Args.ReferencesPath))
				throw new InvalidOperationException("Configuration argument 'ReferencesPath' is not set.");

			Args.RelatedPath.CreateDirectoryIfNotExists();
			Args.ReferencesPath.CreateDirectoryIfNotExists();

			Console.Write("Converting paths for related projects... ");

			var project = new ProjectDocument(path);

			var references = project.GetProjectReferences();
			if (references.Count == 0)
				throw new InvalidOperationException("It is strange that cloud service or fabric application does not have any referenced projects.");

			var log = new LogPackages();
			foreach (var reference in references)
			{
				SetupRelatedProject(reference, log);
			}

			project.Save();
			Console.WriteLine("OK");

			log.Report();

			Console.Write("Saving local references... ");
			log.SaveReferences(Args.ReferencesPath);
			Console.WriteLine("OK");

			Console.Write("Saving packages summary... ");
			log.SaveSummary(Args.TempPath);
			Console.WriteLine("OK");
		}

		private static void SetupRelatedProject(ProjectReference reference, LogPackages log)
		{
			var fileName = Path.GetFileName(reference.Include);
			var localName = Path.GetFileName(Path.GetDirectoryName(reference.Include));
			var referenceName = reference.Name;

			// another hardcode for resolving the names quickly
			if (String.IsNullOrEmpty(referenceName))
			{
				referenceName = localName;

				if (referenceName.StartsWith("Metro.")
					|| referenceName.StartsWith("FlexQueue."))
				{
					referenceName = "CnetContent." + localName;
				}
			}

			var includePath = Path.Combine(Args.RelatedPath, localName, fileName);

			// quick dirty hardcode below with calling tools with specific paths and arguments

			var blobVersion = String.Format("{0}/version.txt", referenceName);
			var localVersion = String.Format(@"{0}\{1}.txt", Args.RelatedPath, localName);

			Execute.Run(
				"CCNet.Build.AzureDownload.exe",
				String.Format(
					@"Storage=Devbuild Container=build ""BlobFile={0}"" ""LocalFile={1}""",
					blobVersion,
					localVersion));

			var version = File.ReadAllText(localVersion);

			var blobSnapshot = String.Format("{0}/{1}/{0}.snapshot.zip", referenceName, version);
			var localSnapshot = String.Format(@"{0}\{1}.zip", Args.RelatedPath, localName);

			Execute.Run(
				"CCNet.Build.AzureDownload.exe",
				String.Format(
					@"Storage=Devbuild Container=snapshot ""BlobFile={0}"" ""LocalFile={1}""",
					blobSnapshot,
					localSnapshot));

			var localFolder = String.Format(@"{0}\{1}", Args.RelatedPath, localName);

			Execute.Run(
				@"C:\Program Files\7-Zip\7z.exe",
				String.Format(@"x ""-o{0}"" ""{1}""", localFolder, localSnapshot));

			reference.UpdateLocation(includePath);

			log.Add(
				referenceName,
				new LogPackage
				{
					PackageId = referenceName,
					ProjectName = referenceName,
					ProjectUrl = String.Format("http://rufc-devbuild.cneu.cnwk/ccnet/server/Azure/project/{0}/ViewProjectReport.aspx", referenceName),
					IsLocal = true,
					SourceVersion = null,
					BuildVersion = new Version(version),
					ProjectReference = true
				});
		}
	}
}
