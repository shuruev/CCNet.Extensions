namespace CCNet.Build.CheckProject
{
	public class ProjectOutputTypeWinExe : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckRequired("OutputType", "WinExe");
		}
	}
}
