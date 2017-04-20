using System;
using System.Collections.Generic;
using System.Linq;

namespace CCNet.Build.CheckProject
{
	public class CheckPrimarySolution : IChecker
	{
		private List<string> m_lines;
		private Dictionary<string, string> m_projects;

		public void Check(CheckContext context)
		{
			m_lines = ParseLines(context);
			m_projects = ParseProjects(m_lines);

			CheckVisualStudioVersion();
			CheckTeamFoundationServer();
			CheckSolutionConfigurationPlatforms();
			CheckProjectConfigurationPlatforms();
		}

		private static List<string> ParseLines(CheckContext context)
		{
			var file = context.TfsSolutionFile.Result;
			return file.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		}

		private static Dictionary<string, string> ParseProjects(IEnumerable<string> lines)
		{
			return lines
				.Where(line => line.StartsWith("Project(\"{"))
				.Where(line => !line.StartsWith("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\")"))
				.Select(line => line.Split(' '))
				.ToDictionary(parts => parts[4].Trim('"'), parts => parts[2].Trim('"', ','));
		}

		private void CheckVisualStudioVersion()
		{
			if (IsVs2013() || IsVs2015() || IsVs2017())
			{
				return;
			}

			throw new FailedCheckException(
				@"It looks like solution file '{0}' was saved using Visual Studio older than 2013.
Please make sure solution file is saved with Visual Studio 2013 or above, so the others could work with it conveniently.",
				Paths.TfsSolutionName);
		}

		private bool IsVs2013()
		{
			return m_lines[0].StartsWith("Microsoft Visual Studio Solution File, Format Version 12.")
				&& m_lines[1] == "# Visual Studio 2013"
				&& m_lines[2].StartsWith("VisualStudioVersion = 12.");
		}

		private bool IsVs2015()
		{
			return m_lines[0].StartsWith("Microsoft Visual Studio Solution File, Format Version 12.")
				&& m_lines[1] == "# Visual Studio 14"
				&& m_lines[2].StartsWith("VisualStudioVersion = 14.");
		}
		
		private bool IsVs2017()
		{
			return m_lines[0].StartsWith("Microsoft Visual Studio Solution File, Format Version 12.")
				&& m_lines[1] == "# Visual Studio 15"
				&& m_lines[2].StartsWith("VisualStudioVersion = 15.");
		}

		private void CheckTeamFoundationServer()
		{
			var tfs = m_lines.Where(l => l.Contains("SccTeamFoundationServer")).ToList();
			if (tfs.Count == 1 && tfs[0] == "\t\tSccTeamFoundationServer = http://rufc-devbuild.cneu.cnwk:8080/tfs/sed")
				return;

			throw new FailedCheckException(
				@"Something looks wrong with source control configuration in solution file '{0}'.
Please make sure both solution and project are properly connected to 'http://rufc-devbuild.cneu.cnwk:8080/tfs/sed'.",
				Paths.TfsSolutionName);
		}

		private void CheckSolutionConfigurationPlatforms()
		{
			var index = m_lines.IndexOf("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
			if (index == -1)
				throw new FailedCheckException(
					"Something went wrong while trying to find platforms configuration within a solution file '{0}'.",
					Paths.TfsSolutionName);

			if (m_lines[index + 1] == "\t\tDebug|Any CPU = Debug|Any CPU"
				&& m_lines[index + 2] == "\t\tRelease|Any CPU = Release|Any CPU")
			{
				return;
			}

			throw new FailedCheckException(
				@"Something looks strange about solution file '{0}' for this project.
We agreed there should be just one solution platform, which should be named 'Any CPU'.
Please go to 'Configuration Manager' dialog and check 'Active solution platform' dropdown list. Also make sure that 'Build' checkbox is enabled for all the projects in both 'Debug' and 'Release' configurations.",
				Paths.TfsSolutionName);
		}

		private void CheckProjectConfigurationPlatforms()
		{
			foreach (var project in m_projects)
			{
				var debugPlatform = CheckProjectConfigurationPlatform(project.Key, "Debug");
				var releasePlatform = CheckProjectConfigurationPlatform(project.Key, "Release");

				if (debugPlatform != releasePlatform)
				{
					throw new FailedCheckException(
						@"Something looks strange about how project '{0}' is configured within its primary solution '{1}'.
It seems specifying different platforms for 'Debug' and 'Release' configurations: '{2}' and '{3}'.",
						project.Value,
						Paths.TfsSolutionName,
						debugPlatform,
						releasePlatform);
				}
			}
		}

		private string CheckProjectConfigurationPlatform(string projectUid, string configuration)
		{
			var prefix1 = String.Format("\t\t{0}.{1}|Any CPU.ActiveCfg", projectUid, configuration);
			var prefix2 = String.Format("\t\t{0}.{1}|Any CPU.Build.0", projectUid, configuration);

			var line1 = m_lines.FirstOrDefault(line => line.StartsWith(prefix1));
			var line2 = m_lines.FirstOrDefault(line => line.StartsWith(prefix2));

			if (line1 == null || line2 == null)
			{
				throw new FailedCheckException(
					@"Something looks strange about how project '{0}' is configured within its primary solution '{1}'.
Please go to 'Configuration Manager' dialog, then choose '{2}' as active solution configuration, and make sure 'Build' checkbox is enabled for this project.",
					m_projects[projectUid],
					Paths.TfsSolutionName,
					configuration);
			}

			var value1 = line1.Split(new[] { " = " }, StringSplitOptions.None)[1];
			var value2 = line2.Split(new[] { " = " }, StringSplitOptions.None)[1];

			var parts1 = value1.Split('|');
			var parts2 = value2.Split('|');

			if (parts1[0] != configuration || parts2[0] != configuration)
			{
				throw new FailedCheckException(
					@"Something looks strange about how project '{0}' is configured within its primary solution '{1}'.
Please go to 'Configuration Manager' dialog, then choose '{2}' as active solution configuration, and make sure this project is using '{2}' configuration there.",
					m_projects[projectUid],
					Paths.TfsSolutionName,
					configuration);
			}

			if (parts1[1] != parts2[1])
			{
				throw new FailedCheckException(
					@"Something looks strange about how project '{0}' is configured within its primary solution '{1}'.
For some reason it specifies different platforms for 'Build' and 'Active configuration' properties within '{2}' configuration: '{3}' and '{4}'.",
					m_projects[projectUid],
					Paths.TfsSolutionName,
					configuration,
					parts1[1],
					parts2[1]);
			}

			return parts1[1];
		}
	}
}
