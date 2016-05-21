using System;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class LibraryProjectConfiguration : BasicProjectConfiguration, IProjectRelease
	{
		public string Dependencies { get; set; }
		public string CustomAssemblyName { get; set; }
		public string CustomCompanyName { get; set; }

		public override ProjectType Type
		{
			get { return ProjectType.Library; }
		}

		public bool MarkAsCustom
		{
			get
			{
				if (CustomAssemblyName != null)
					return true;

				if (CustomCompanyName != null)
					return true;

				return false;
			}
		}

		public string WorkingDirectoryNuget
		{
			get { return String.Format(@"{0}\nuget", WorkingDirectory); }
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
