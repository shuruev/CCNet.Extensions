using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectDocumentationFull : IChecker
	{
		public void Check(CheckContext context)
		{
			var details = "That is because documentation type was set to 'Full' for this project.";

			var debug = context.ProjectDebugProperties.Result;
			var debugXml = String.Format("{0}{1}.xml", debug["OutputPath"], Args.AssemblyName);
			debug.CheckOptional("NoWarn", String.Empty, details);
			debug.CheckRequired("DocumentationFile", debugXml, details);

			var release = context.ProjectReleaseProperties.Result;
			var releaseXml = String.Format("{0}{1}.xml", release["OutputPath"], Args.AssemblyName);
			release.CheckOptional("NoWarn", String.Empty, details);
			release.CheckRequired("DocumentationFile", releaseXml, details);
		}
	}
}
