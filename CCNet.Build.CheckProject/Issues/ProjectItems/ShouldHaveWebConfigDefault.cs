using CCNet.Build.Common;

namespace CCNet.Build.CheckProject
{
	public class ShouldHaveWebConfigDefault : ShouldHaveWebConfig
	{
		public override void Check(CheckContext context)
		{
			var config = RequiredProjectFile(context, "Web.config.default");
			CheckBuildAction(config, BuildAction.Content);
			CheckCopyToOutput(config, CopyToOutputDirectory.None);
		}
	}
}
