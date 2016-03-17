namespace CCNet.Build.CheckProject
{
	public class CheckProjectPlatforms : IChecker
	{
		public void Check(CheckContext context)
		{
			var project = context.ProjectDocument.Result;
			var platforms = project.GetUsedPlatforms();
			platforms.Remove("AnyCPU");
			platforms.Remove("x86");

			if (platforms.Count == 0)
				return;

			throw new FailedCheckException(
				@"Found unknown configuration '{0}'.
Please use standard platforms only, the most common and recommended one is 'Any CPU'.",
				platforms[0]);
		}
	}
}
