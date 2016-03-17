namespace CCNet.Build.CheckProject
{
	public class ProjectTargetFramework35 : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckRequired("TargetFrameworkVersion", "v3.5");
		}
	}
}
