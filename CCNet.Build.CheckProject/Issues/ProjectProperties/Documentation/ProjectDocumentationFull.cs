using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectDocumentationFull : IChecker
	{
		public void Check(CheckContext context)
		{
			var debug = context.ProjectDebugProperties.Result;
			debug.CheckOptional("NoWarn", String.Empty);
			debug.CheckRequired("DocumentationFile", String.Format(@"bin\Debug\{0}.xml", Args.ProjectName));

			var release = context.ProjectReleaseProperties.Result;
			release.CheckOptional("NoWarn", String.Empty);
			release.CheckRequired("DocumentationFile", String.Format(@"bin\Release\{0}.xml", Args.ProjectName));
		}
	}
}
