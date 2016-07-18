namespace CCNet.Build.CheckProject
{
	public class OutputPathBin : IChecker
	{
		public void Check(CheckContext context)
		{
			var debug = context.ProjectDebugProperties.Result;
			debug.CheckRequired("OutputPath", @"bin\");

			var release = context.ProjectReleaseProperties.Result;
			release.CheckRequired("OutputPath", @"bin\");
		}
	}
}
