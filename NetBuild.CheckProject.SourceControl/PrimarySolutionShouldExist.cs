using System.IO;
using System.Linq;

namespace NetBuild.CheckProject.SourceControl
{
	public class PrimarySolutionShouldExist : ICheckIssue
	{
		public string Issue => "S02";

		public void Check(CheckContext context)
		{
			var solutionName = context.Of<RemoteSolution>().SolutionName;
			var solutionItems = context.Of<SolutionPathItems>().Value;

			var solution = solutionItems
				.Where(item => Path.GetFileName(item) == solutionName)
				.FirstOrDefault();

			if (solution != null)
				return;

			throw new CheckException($"Project should have a primary solution file '{solutionName}' placed in parent folder.");
		}
	}
}
