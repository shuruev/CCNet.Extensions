using System;
using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class CloudServiceProjectConfiguration : BasicProjectConfiguration, IProjectRelated, IProjectPublish
	{
		public List<string> VmSizes { get; set; }

		public CloudServiceProjectConfiguration()
		{
			VmSizes = new List<string>();
		}

		public override ProjectType Type
		{
			get { return ProjectType.CloudService; }
		}

		public string SourceFileServiceDefinition
		{
			get { return String.Format(@"{0}\ServiceDefinition.csdef", WorkingDirectorySource); }
		}

		public string ReleaseFileServiceConfiguration
		{
			get { return String.Format(@"{0}\ServiceConfiguration.cscfg", SourceDirectoryRelease); }
		}

		public string SourceDirectoryPublished
		{
			get { return String.Format(@"{0}\app.publish", SourceDirectoryRelease); }
		}

		public string PublishedFilePackage
		{
			get { return String.Format(@"{0}\{1}.cspkg", SourceDirectoryPublished, Name); }
		}

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// replace F02 (ProjectFileShouldExist) with F08 (CloudProjectFileShouldExist)
			var f02 = checks.IndexOf("F02");
			checks.Insert(f02, "F08");
			checks.Remove("F02");

			// remove a couple of basic checks which are unrelated for *.ccproj file
			checks.Remove("F03");
			checks.Remove("C02");
			checks.Remove("P10");

			return checks;
		}
	}
}
