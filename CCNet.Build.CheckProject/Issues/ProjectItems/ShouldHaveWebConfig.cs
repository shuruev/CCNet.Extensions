using CCNet.Build.Common;

namespace CCNet.Build.CheckProject
{
	public class ShouldHaveWebConfig : ShouldHaveAppConfig
	{
		public override void Check(CheckContext context)
		{
			var config = RequiredProjectFile(context, "Web.config");
			CheckBuildAction(config, BuildAction.Content);
			CheckCopyToOutput(config, CopyToOutputDirectory.None);
		}
	}
}
