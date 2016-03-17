namespace CCNet.Build.CheckProject
{
	public class ProjectOutputTypeLibrary : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckRequired("OutputType", "Library");
		}
	}
}
