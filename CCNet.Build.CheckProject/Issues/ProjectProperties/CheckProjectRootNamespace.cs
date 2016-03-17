namespace CCNet.Build.CheckProject
{
	public class CheckProjectRootNamespace : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckRequired("RootNamespace", Args.RootNamespace);
		}
	}
}
