using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectDocumentationPartial : IChecker
	{
		public void Check(CheckContext context)
		{
			var details = "That is because documentation type was set to 'Partial' for this project.";

			var debug = context.ProjectDebugProperties.Result;
			var debugXml = String.Format("{0}{1}.xml", debug["OutputPath"], Args.AssemblyName);
			debug.CheckRequired("NoWarn", "1591", details);
			debug.CheckRequired("DocumentationFile", debugXml, details);

			var release = context.ProjectReleaseProperties.Result;
			var releaseXml = String.Format("{0}{1}.xml", release["OutputPath"], Args.AssemblyName);
			release.CheckRequired("NoWarn", "1591", details);
			release.CheckRequired("DocumentationFile", releaseXml, details);
		}
	}
}
