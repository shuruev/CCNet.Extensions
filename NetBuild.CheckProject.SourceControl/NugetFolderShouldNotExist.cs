namespace NetBuild.CheckProject.SourceControl
{
	public class NugetFolderShouldNotExist : ICheckIssue
	{
		public string Issue => "S03";

		public void Check(CheckContext context)
		{
			var solutionItems = context.Of<SolutionPathItems>().Value;

			if (!solutionItems.Contains(".nuget/"))
				return;

			throw new CheckException(@"Solution folder should not contain '.nuget' folder inside.
Please consider using 'nuget.config' file from repository root.");
		}
	}
}
