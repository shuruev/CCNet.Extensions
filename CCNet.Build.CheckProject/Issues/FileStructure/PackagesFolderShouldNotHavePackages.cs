using System.IO;
using System.Linq;

namespace CCNet.Build.CheckProject
{
	public class PackagesFolderShouldNotHavePackages : IChecker
	{
		public void Check(CheckContext context)
		{
			var packages = context.TfsSolutionItems.Result
				.Where(item => Path.GetFileName(item) == "packages")
				.FirstOrDefault();

			if (packages == null)
				return;

			var files = context.TfsPackagesItems.Result;
			if (files.Count == 1 && files[0] == Paths.TfsRepositoriesConfig)
				return;

			throw new FailedCheckException(@"Solution folder 'packages' should contain only a single file named 'repositories.config'.
It should not contain binary contents from other packages as it leads to TFS overfill.");
		}
	}
}
