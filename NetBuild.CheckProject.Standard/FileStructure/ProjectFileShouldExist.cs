using System.IO;
using System.Linq;

namespace NetBuild.CheckProject.Standard
{
	public class ProjectFileShouldExist : ICheckIssue
	{
		public string Issue => "F01";

		public virtual void Check(CheckContext context)
		{
			var projectFile = context.Of<ProjectFile>().Value;
			var localItems = context.Of<LocalPathItems>().Value;
			var projectExtension = context.Of<ProjectExtension>().Value;

			var file = localItems
				.Where(f => Path.GetExtension(f) == "." + projectExtension)
				.ToList();

			if (file.Count == 0)
				throw new CheckException($"Cannot find project file '*.{projectExtension}'.");

			if (file.Count > 1)
				throw new CheckException($"There should be only one project file '*.{projectExtension}', but {file.Count} files were found.");

			if (file[0] == projectFile)
				return;

			throw new CheckException($"Project file should be named '{projectFile}' and placed in project root folder.");
		}
	}
}
