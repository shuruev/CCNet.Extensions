using System;
using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class FabricApplicationProjectConfiguration : BasicProjectConfiguration, IProjectRelated, IProjectPublish
	{
		public override ProjectType Type => ProjectType.FabricApplication;

		/*xxxpublic string SourceFileServiceDefinition
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
		}*/

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// replace F02 (ProjectFileShouldExist) with F09 (FabricProjectFileShouldExist)
			var f02 = checks.IndexOf("F02");
			checks.Insert(f02, "F09");
			checks.Remove("F02");

			// remove a couple of basic checks which are unrelated for *.sfproj file
			checks.Remove("F03");
			checks.Remove("C02");
			checks.Remove("P02");
			checks.Remove("P04");
			checks.Remove("P07");
			checks.Remove("P08");
			checks.Remove("P10");
			checks.Remove("P14");
			checks.Remove("P20");

			return checks;
		}
	}
}
