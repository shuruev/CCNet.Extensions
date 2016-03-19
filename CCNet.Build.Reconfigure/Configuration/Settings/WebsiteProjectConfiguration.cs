using System;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class WebsiteProjectConfiguration : BasicProjectConfiguration
	{
		public override ProjectType Type
		{
			get { return ProjectType.Website; }
		}

		public string WorkingDirectoryPublish
		{
			get { return String.Format(@"$(projectsPath)\{0}\publish", UniqueName); }
		}

		public string ReleaseDirectoryPublished
		{
			get { return String.Format(@"{0}\_PublishedWebsites\{1}", WorkingDirectoryRelease, Name); }
		}

		public string PublishFileName
		{
			get { return String.Format(@"{0}.publish.zip", Name); }
		}

		public string PublishFileLocal
		{
			get { return String.Format(@"{0}\{1}", WorkingDirectoryPublish, PublishFileName); }
		}
	}
}
