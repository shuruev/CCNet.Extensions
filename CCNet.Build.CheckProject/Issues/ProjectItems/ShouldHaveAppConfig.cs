using System;
using System.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.CheckProject
{
	public class ShouldHaveAppConfig : IChecker
	{
		protected ProjectFile RequiredProjectFile(CheckContext context, string fileName)
		{
			var files = context.ProjectFiles.Result;
			var file = files.FirstOrDefault(i => i.FullName == fileName);

			file.Check(
				i => i != null,
				String.Format("Project is expected to have '{0}' file.", fileName));

			return file;
		}

		protected void CheckBuildAction(ProjectFile file, BuildAction expected)
		{
			file.Check(
				i => i.BuildAction == expected,
				String.Format(
					"Project file '{0}' is expected to have 'Build Action' set to '{1}', but now it is '{2}'.",
					file.FullName,
					expected,
					file.BuildAction));
		}

		protected void CheckCopyToOutput(ProjectFile file, CopyToOutputDirectory expected)
		{
			file.Check(
				i => i.CopyToOutput == expected,
				String.Format(
					"Project file '{0}' is expected to have 'Copy to Output Directory' set to '{1}', but now it is '{2}'.",
					file.FullName,
					expected,
					file.CopyToOutput));
		}

		public virtual void Check(CheckContext context)
		{
			var config = RequiredProjectFile(context, "App.config");
			CheckBuildAction(config, BuildAction.None);
			CheckCopyToOutput(config, CopyToOutputDirectory.None);
		}
	}
}
