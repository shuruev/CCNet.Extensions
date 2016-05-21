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
			if (Args.ProjectType == ProjectType.CloudService)
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

		private static void SetupRelatedProjects()
		{
			if (Args.ProjectType != ProjectType.CloudService)
				return;

			if (String.IsNullOrEmpty(Args.RelatedPath))
				throw new InvalidOperationException("Configuration argument 'RelatedPath' is not set.");

			if (String.IsNullOrEmpty(Args.ReferencesPath))
				throw new InvalidOperationException("Configuration argument 'ReferencesPath' is not set.");

			Args.RelatedPath.CreateDirectoryIfNotExists();
			Args.ReferencesPath.CreateDirectoryIfNotExists();

			Console.Write("Converting paths for related projects... ");

			var project = new ProjectDocument(Paths.CloudProjectFile);
			project.Load();

			var references = project.GetProjectReferences();
			if (references.Count == 0)
				throw new InvalidOperationException("It is strange that cloud service does not have any referenced projects.");

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
			var folderName = reference.Name;
			var includePath = Path.Combine(Args.RelatedPath, folderName, fileName);

			// xxx quick dirty hardcode below with calling tools with specific paths and arguments

			var blobVersion = String.Format("{0}/version.txt", reference.Name);
			var localVersion = String.Format(@"{0}\{1}.txt", Args.RelatedPath, folderName);

			Execute.Run(
				"CCNet.Build.AzureDownload.exe",
				String.Format(
					@"Storage=Devbuild Container=build ""BlobFile={0}"" ""LocalFile={1}""",
					blobVersion,
					localVersion));

			var version = File.ReadAllText(localVersion);

			var blobSnapshot = String.Format("{0}/{1}/{2}.snapshot.zip", reference.Name, version, folderName);
			var localSnapshot = String.Format(@"{0}\{1}.zip", Args.RelatedPath, folderName);

			Execute.Run(
				"CCNet.Build.AzureDownload.exe",
				String.Format(
					@"Storage=Devbuild Container=snapshot ""BlobFile={0}"" ""LocalFile={1}""",
					blobSnapshot,
					localSnapshot));

			var localFolder = String.Format(@"{0}\{1}", Args.RelatedPath, folderName);

			Execute.Run(
				@"C:\Program Files\7-Zip\7z.exe",
				String.Format(@"x ""-o{0}"" ""{1}""", localFolder, localSnapshot));

			reference.UpdateLocation(includePath);

			log.Add(
				folderName,
				new LogPackage
				{
					PackageId = folderName,
					ProjectName = folderName,
					ProjectUrl = String.Format("http://rufc-devbuild.cneu.cnwk/ccnet/server/Azure/project/{0}/ViewProjectReport.aspx", folderName),
					IsLocal = true,
					SourceVersion = null,
					BuildVersion = new Version(version),
					ProjectReference = true
				});
		}
	}
}
