using System;
using CCNet.Build.Common;

namespace CCNet.Build.CheckProject
{
	public class ShouldHaveAppConfigDefault : ShouldHaveAppConfig
	{
		public override void Check(CheckContext context)
		{
			var config = RequiredProjectFile(context, String.Format("{0}.exe.config.default", Args.ProjectName));
			CheckBuildAction(config, BuildAction.Content);
			CheckCopyToOutput(config, CopyToOutputDirectory.Always);
		}
	}
}
