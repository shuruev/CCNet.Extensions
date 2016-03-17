namespace CCNet.Build.CheckProject
{
	public class CheckProjectConfigurations : IChecker
	{
		public void Check(CheckContext context)
		{
			var project = context.ProjectDocument.Result;
			var configurations = project.GetUsedConfigurations();
			configurations.Remove("Debug");
			configurations.Remove("Release");

			if (configurations.Count == 0)
				return;

			throw new FailedCheckException(
				@"Found unknown configuration '{0}'.
Please use standard configurations only: 'Debug' and 'Release'.",
				configurations[0]);
		}
	}
}
