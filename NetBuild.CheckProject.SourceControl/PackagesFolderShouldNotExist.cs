namespace NetBuild.CheckProject.SourceControl
{
	public class PackagesFolderShouldNotExist : ICheckIssue
	{
		public string Issue => "S04";

		public void Check(CheckContext context)
		{
			var solutionItems = context.Of<SolutionPathItems>().Value;

			if (!solutionItems.Contains("packages/"))
				return;

			throw new CheckException(@"Solution folder should not contain 'packages' folder inside.
Please consider using 'nuget.config' and '.tfignore' files from repository root.");
		}
	}
}
