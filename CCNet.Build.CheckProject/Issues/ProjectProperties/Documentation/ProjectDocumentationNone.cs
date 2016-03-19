using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectDocumentationNone : IChecker
	{
		public void Check(CheckContext context)
		{
			var details = "That is because documentation type was set to 'None' for this project.";

			var debug = context.ProjectDebugProperties.Result;
			debug.CheckOptional("NoWarn", String.Empty, details);
			debug.CheckOptional("DocumentationFile", String.Empty, details);

			var release = context.ProjectReleaseProperties.Result;
			release.CheckOptional("NoWarn", String.Empty, details);
			release.CheckOptional("DocumentationFile", String.Empty, details);
		}
	}
}
