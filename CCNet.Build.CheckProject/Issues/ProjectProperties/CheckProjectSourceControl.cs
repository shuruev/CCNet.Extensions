namespace CCNet.Build.CheckProject
{
	public class CheckProjectSourceControl : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;

			properties.CheckRequired("SccAuxPath", "SAK");
			properties.CheckRequired("SccLocalPath", "SAK");
			properties.CheckRequired("SccProjectName", "SAK");
			properties.CheckRequired("SccProvider", "SAK");
		}
	}
}
