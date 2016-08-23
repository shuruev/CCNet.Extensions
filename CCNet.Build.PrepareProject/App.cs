using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CCNet.Build.Common;
using NetBuild.ConsoleApp;
using NetBuild.Tfs;

namespace CCNet.Build.PrepareProject
{
	public class App : ConsoleApp<Args>
	{
		private Config m_config;

		protected override void Run()
		{
			m_config = new Config();

			SaveSource();
			SaveVersion();

			if (m_args.UpdateAssemblyInfo)
				UpdateAssemblyInfo();
		}

		private void SaveSource()
		{
			Console.Write("Saving TFS summary... ");

			var client = new TfsClient(m_config.TfsUrl);
			var changeset = client.GetLatestChangeset(m_args.TfsPath);

			var sb = new StringBuilder();
			sb.AppendLine("Source:");
			sb.AppendLine(m_args.TfsPath);
			sb.Append($"Changeset #{changeset.Id}").AppendLine();
			sb.Append($"User: {changeset.UserDisplay}").AppendLine();
			sb.Append($"Date: {changeset.Date.ToDetailedString()}").AppendLine();

			File.WriteAllText(m_args.SourceFile(), sb.ToString());

			Console.WriteLine("OK");

			if (m_args.DebugMode)
				Console.WriteLine(sb.ToString());
		}

		private void SaveVersion()
		{
			Console.Write("Saving current version... ");

			File.WriteAllText(m_args.VersionFile(), m_args.CurrentVersion.ToString());

			Console.WriteLine("OK");

			if (m_args.DebugMode)
				Console.WriteLine(m_args.CurrentVersion.ToString());
		}

		private void UpdateAssemblyInfo()
		{
			Console.Write("Updating assembly information... ");

			var text = File.ReadAllText(m_args.AssemblyInfoFile());

			text = new Regex(@"^\[assembly: AssemblyVersion\(""[0-9\.?]+""\)]", RegexOptions.Multiline)
				.Replace(text, $"[assembly: AssemblyVersion(\"{m_args.CurrentVersion}\")]");

			text = new Regex(@"^\[assembly: AssemblyFileVersion\(""[0-9\.?]+""\)]", RegexOptions.Multiline)
				.Replace(text, $"[assembly: AssemblyFileVersion(\"{m_args.CurrentVersion}\")]");

			File.WriteAllText(m_args.AssemblyInfoFile(), text, Encoding.UTF8);
			Console.WriteLine("OK");
		}
	}
}
