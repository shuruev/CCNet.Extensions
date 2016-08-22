using System.IO;
using NetBuild.CheckProject.Standard;

namespace NetBuild.CheckProject.SourceControl
{
	public class ProjectFolderShouldHaveProjectName : ICheckIssue
	{
		public string Issue => "S01";

		public void Check(CheckContext context)
		{
			var projectName = context.Of<ProjectName>().Value;
			var localName = context.Of<LocalName>().Value;
			var remotePath = context.Of<RemotePath>().Value;

			var projectFolder = Path.GetFileName(remotePath);
			if (projectFolder == localName)
				return;

			if (localName == projectName)
				throw new CheckException($"Project folder '{projectFolder}' does not conform with project name '{projectName}'.");

			throw new CheckException($"Project folder '{projectFolder}' does not conform with project name '{projectName}' (expected to be '{localName}').");
		}
	}
}
