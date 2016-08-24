using System;

namespace CCNet.Build.Reconfigure
{
	public class ProjectConfiguration : IProjectConfigurationTemp
	{
		public string Branch { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public string TfsPath { get; set; }
		public string OwnerEmail { get; set; }
		public TimeSpan BuildEvery { get; set; }

		public string UniqueName
		{
			get
			{
				if (String.IsNullOrEmpty(Branch))
					return Name;

				return $"{Name}-{Branch}";
			}
		}

		public string BuildQueue => Category;

		public string WebUrl => $"$(serverUrl)/project/{UniqueName}/ViewProjectReport.aspx";

		public string AdminDirectory => "$(adminPath)";

		public string AdminDirectoryRebuildAll => $@"{AdminDirectory}\RebuildAll";

		public string WorkingDirectory => $@"$(projectsPath)\{UniqueName}";

		public string WorkingDirectoryReferences => $@"{WorkingDirectory}\references";

		public string WorkingDirectorySource => $@"{WorkingDirectory}\source";

		public string WorkingDirectoryPackages => $@"{WorkingDirectory}\packages";

		public string WorkingDirectoryTemp => $@"{WorkingDirectory}\temp";

		public string TempFileSource => $@"{WorkingDirectoryTemp}\source.txt";

		public string TempFilePackages => $@"{WorkingDirectoryTemp}\packages.txt";

		public string TempFileVersion => $@"{WorkingDirectoryTemp}\version.txt";
	}
}
