using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectDocumentationNone : IChecker
	{
		public void Check(CheckContext context)
		{
			var debug = context.ProjectDebugProperties.Result;
			debug.CheckOptional("NoWarn", String.Empty);
			debug.CheckOptional("DocumentationFile", String.Empty);

			var release = context.ProjectReleaseProperties.Result;
			release.CheckOptional("NoWarn", String.Empty);
			release.CheckOptional("DocumentationFile", String.Empty);
		}
	}
}
