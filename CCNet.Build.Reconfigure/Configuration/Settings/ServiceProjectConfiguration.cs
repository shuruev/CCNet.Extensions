using System;
using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class ServiceProjectConfiguration : BasicProjectConfiguration
	{
		public string Title { get; set; }

		public override ProjectType Type
		{
			get { return ProjectType.Service; }
		}

		public string WorkingDirectoryPublish
		{
			get { return String.Format(@"$(projectsPath)\{0}\publish", UniqueName); }
		}

		public string ReleaseFileName
		{
			get { return String.Format(@"{0}.zip", Name); }
		}

		public string ReleaseFileLocal
		{
			get { return String.Format(@"{0}\{1}", WorkingDirectoryPublish, ReleaseFileName); }
		}

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// replace P04 (ProjectOutputTypeLibrary) with P05 (ProjectOutputTypeExe)
			var p04 = checks.IndexOf("P04");
			checks.Insert(p04, "P05");
			checks.Remove("P04");

			return checks;
		}
	}
}
