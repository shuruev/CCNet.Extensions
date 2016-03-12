using System.IO;
using System.Linq;

namespace CCNet.Build.CheckProject
{
	public class PrimarySolutionShouldExist : IChecker
	{
		public void Check(CheckContext context)
		{
			var solutionName = Path.GetFileName(Paths.TfsSolutionFile);

			var solution = context.TfsSolutionItems.Result
				.Where(item => Path.GetFileName(item) == solutionName)
				.FirstOrDefault();

			if (solution == null)
				throw new FailedCheckException("Project should have a primary solution file '{0}' placed in parent folder.", solutionName);
		}
	}
}
