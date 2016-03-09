using System;

namespace CCNet.Build.Reconfigure
{
	public abstract class ProjectConfiguration
	{
		public string Branch { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public string TfsPath { get; set; }
		public string OwnerEmail { get; set; }

		public string UniqueName
		{
			get
			{
				if (String.IsNullOrEmpty(Branch))
					return Name;

				return String.Format("{0}-{1}", Name, Branch);
			}
		}

		public string WebUrl
		{
			get { return String.Format("$(serverUrl)/project/{0}/ViewProjectReport.aspx", UniqueName); }
		}

		public string WorkingDirectory
		{
			get { return String.Format(@"$(projectsPath)\{0}", UniqueName); }
		}

		public string WorkingDirectoryReferences
		{
			get { return String.Format(@"$(projectsPath)\{0}\references", UniqueName); }
		}

		public string WorkingDirectorySource
		{
			get { return String.Format(@"$(projectsPath)\{0}\source", UniqueName); }
		}

		public string WorkingDirectoryRelease
		{
			get { return String.Format(@"$(projectsPath)\{0}\release", UniqueName); }
		}

		public string WorkingDirectoryPackages
		{
			get { return String.Format(@"$(projectsPath)\{0}\packages", UniqueName); }
		}

		public string WorkingFileSummary
		{
			get { return String.Format(@"{0}\summary.txt", WorkingDirectoryPackages); }
		}
	}
}
