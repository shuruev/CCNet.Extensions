using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CCNet.Build.Common;
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

		private static void UpdateAssemblyInfo()
		{
			if (Args.ProjectType == ProjectType.Cloud)
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
			if (Args.ProjectType != ProjectType.Cloud)
				return;

			Console.Write("Converting paths for related projects... ");

			var project = new ProjectDocument(Paths.CloudProjectFile);
			project.Load();

			foreach (var reference in project.GetProjectReferences())
			{
				if (String.IsNullOrEmpty(Args.RelatedPath))
					throw new InvalidOperationException("Configuration argument 'RelatedPath' is not set.");

				var fileName = Path.GetFileName(reference.Include);
				var folderName = reference.Name;
				var includePath = Path.Combine(Args.RelatedPath, folderName, fileName);

				reference.UpdateLocation(includePath);
			}

			project.Save();
			Console.WriteLine("OK");
		}
	}
}
