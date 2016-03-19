using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectDocumentationPartial : IChecker
	{
		public void Check(CheckContext context)
		{
			var details = "That is because documentation type was set to 'Partial' for this project.";

			var debug = context.ProjectDebugProperties.Result;
			debug.CheckRequired("NoWarn", "1591", details);
			debug.CheckRequired("DocumentationFile", String.Format(@"bin\Debug\{0}.xml", Args.ProjectName), details);

			var release = context.ProjectReleaseProperties.Result;
			release.CheckRequired("NoWarn", "1591", details);
			release.CheckRequired("DocumentationFile", String.Format(@"bin\Release\{0}.xml", Args.ProjectName), details);
		}
	}
}
