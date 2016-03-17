using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectDocumentationPartial : IChecker
	{
		public void Check(CheckContext context)
		{
			var debug = context.ProjectDebugProperties.Result;
			debug.CheckRequired("NoWarn", "1591");
			debug.CheckRequired("DocumentationFile", String.Format(@"bin\Debug\{0}.xml", Args.ProjectName));

			var release = context.ProjectReleaseProperties.Result;
			release.CheckRequired("NoWarn", "1591");
			release.CheckRequired("DocumentationFile", String.Format(@"bin\Release\{0}.xml", Args.ProjectName));
		}
	}
}
