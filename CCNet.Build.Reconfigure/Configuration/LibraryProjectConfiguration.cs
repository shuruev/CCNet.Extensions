using System;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class LibraryProjectConfiguration : BasicProjectConfiguration
	{
		public ProjectType Type
		{
			get { return ProjectType.Library; }
		}

		public string WorkingDirectoryNuget
		{
			get { return String.Format(@"$(projectsPath)\{0}\nuget", UniqueName); }
		}

		public string NugetPushUrl
		{
			get
			{
				if (String.IsNullOrEmpty(Branch))
					return "$(nugetUrl)/api/v2/package";

				return String.Format("$(nugetUrl)/private/{0}/api/v2/package", Branch);
			}
		}
	}
}
