using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectDocumentationFull : IChecker
	{
		public void Check(CheckContext context)
		{
			var details = "That is because documentation type was set to 'Full' for this project.";

			var debug = context.ProjectDebugProperties.Result;
			debug.CheckOptional("NoWarn", String.Empty, details);
			debug.CheckRequired("DocumentationFile", String.Format(@"bin\Debug\{0}.xml", Args.ProjectName), details);

			var release = context.ProjectReleaseProperties.Result;
			release.CheckOptional("NoWarn", String.Empty, details);
			release.CheckRequired("DocumentationFile", String.Format(@"bin\Release\{0}.xml", Args.ProjectName), details);
		}
	}
}
