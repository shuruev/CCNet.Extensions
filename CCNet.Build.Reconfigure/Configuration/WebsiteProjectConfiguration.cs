using System;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class WebsiteProjectConfiguration : BasicProjectConfiguration
	{
		public ProjectType Type
		{
			get { return ProjectType.Website; }
		}

		public string WorkingDirectoryPublish
		{
			get { return String.Format(@"$(projectsPath)\{0}\publish", UniqueName); }
		}

		public string PublishFileName
		{
			get { return String.Format(@"{0}\{1}.publish.zip", WorkingDirectoryPublish, Name); }
		}
	}
}
