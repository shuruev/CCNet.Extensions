namespace CCNet.Build.CheckProject
{
	public class ProjectOutputTypeExe : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckRequired("OutputType", "Exe");
		}
	}
}
