using System.IO;

namespace CCNet.Build.CheckProject
{
	public class ProjectFolderShouldHaveProjectName : IChecker
	{
		public void Check(CheckContext context)
		{
			var projectFolder = Path.GetFileName(Args.TfsPath);
			if (projectFolder == Args.ProjectName)
				return;

			const string prefix = "CnetContent.";
			if (Args.ProjectName.StartsWith(prefix))
			{
				var custom = Args.ProjectName.Substring(prefix.Length);
				if (projectFolder == custom)
					return;
			}

			throw new FailedCheckException("Project folder '{0}' does not conform with project name '{1}'.", projectFolder, Args.ProjectName);
		}
	}
}
