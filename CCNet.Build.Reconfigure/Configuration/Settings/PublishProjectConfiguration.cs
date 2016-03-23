using System;
using System.Collections.Generic;

namespace CCNet.Build.Reconfigure
{
	public abstract class PublishProjectConfiguration : BasicProjectConfiguration
	{
		public string Title { get; set; }

		public string WorkingDirectoryPublish
		{
			get { return String.Format(@"$(projectsPath)\{0}\publish", UniqueName); }
		}

		public virtual string PublishFileName
		{
			get { return String.Format(@"{0}.zip", Name); }
		}

		public string PublishFileLocal
		{
			get { return String.Format(@"{0}\{1}", WorkingDirectoryPublish, PublishFileName); }
		}

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// project items
			checks.Add("I01"); // ShouldHaveAppConfig
			checks.Add("I02"); // ShouldHaveAppConfigDefault

			return checks;
		}
	}
}
