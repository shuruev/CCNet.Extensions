namespace CCNet.Build.CheckProject
{
	public class CheckNugetConfig : IChecker
	{
		public void Check(CheckContext context)
		{
			var config = context.TfsNugetConfig.Result;

			var compare = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <solution>
    <add key=""disableSourceControlIntegration"" value=""true"" />
  </solution>
</configuration>";

			if (config != compare)
				throw new FailedCheckException(@"File 'nuget.config' has a strange contents.
Please try to copy it from a different solution.");
		}
	}
}
