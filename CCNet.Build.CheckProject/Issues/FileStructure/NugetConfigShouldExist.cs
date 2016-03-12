using System.IO;
using System.Linq;

namespace CCNet.Build.CheckProject
{
	public class NugetConfigShouldExist : IChecker
	{
		public void Check(CheckContext context)
		{
			var nuget = context.TfsSolutionItems.Result
				.Where(item => Path.GetFileName(item) == ".nuget")
				.FirstOrDefault();

			if (nuget == null)
				throw new FailedCheckException(@"There should be a folder named '.nuget' under solution folder.
Please refer to a different solution and copy it from there.");

			var config = context.TfsNugetItems.Result;
			if (config.Count == 1 && config[0] == Paths.TfsNugetConfig)
				return;

			throw new FailedCheckException(@"Solution folder '.nuget' should contain only a single file named 'nuget.config'.
Please refer to a different solution and copy it from there.");
		}
	}
}
