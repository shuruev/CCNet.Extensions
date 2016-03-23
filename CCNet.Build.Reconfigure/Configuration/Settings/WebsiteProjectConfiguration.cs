using System;
using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class WebsiteProjectConfiguration : PublishProjectConfiguration
	{
		public override ProjectType Type
		{
			get { return ProjectType.Website; }
		}

		public override string PublishFileName
		{
			get { return String.Format(@"{0}.publish.zip", Name); }
		}

		public string ReleaseDirectoryPublished
		{
			get { return String.Format(@"{0}\_PublishedWebsites\{1}", WorkingDirectoryRelease, Name); }
		}

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// replace I01 (ShouldHaveAppConfig) and I02 (ShouldHaveAppConfigDefault)
			// with I03 (ShouldHaveWebConfig) and I04 (ShouldHaveWebConfigDefault)
			var i01 = checks.IndexOf("I01");
			checks.Insert(i01, "I03");
			checks.Insert(i01, "I04");
			checks.Remove("I01");
			checks.Remove("I02");

			return checks;
		}
	}
}
