using System;
using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class WebsiteProjectConfiguration : PublishProjectConfiguration, IProjectPublish
	{
		public override ProjectType Type
		{
			get { return ProjectType.Website; }
		}

		public override string SourceDirectoryRelease
		{
			get { return String.Format(@"{0}\bin", WorkingDirectorySource); }
		}

		public string SourceDirectoryPublished
		{
			get { return String.Format(@"{0}\_PublishedWebsites\{1}", SourceDirectoryRelease, Name); }
		}

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// replace I01 (ShouldHaveAppConfig) and I02 (ShouldHaveAppConfigDefault)
			// with I03 (ShouldHaveWebConfig) and I04 (ShouldHaveWebConfigDefault)
			var i01 = checks.IndexOf("I01");
			checks.Insert(i01, "I04");
			checks.Insert(i01, "I03");
			checks.Remove("I01");
			checks.Remove("I02");

			return checks;
		}
	}
}
