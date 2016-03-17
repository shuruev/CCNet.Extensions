namespace CCNet.Build.CheckProject
{
	public class ProjectTargetFramework20 : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckRequired("TargetFrameworkVersion", "v2.0");
		}
	}
}
