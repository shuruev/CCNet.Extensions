namespace CCNet.Build.CheckProject
{
	public class OutputPathDefault : IChecker
	{
		public void Check(CheckContext context)
		{
			var debug = context.ProjectDebugProperties.Result;
			debug.CheckRequired("OutputPath", @"bin\Debug\");

			var release = context.ProjectReleaseProperties.Result;
			release.CheckRequired("OutputPath", @"bin\Release\");
		}
	}
}
