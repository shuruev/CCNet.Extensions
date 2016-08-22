using System.IO;
using System.Linq;

namespace NetBuild.CheckProject.Standard
{
	public class AssemblyInfoShouldExist : ICheckIssue
	{
		public string Issue => "F02";

		public virtual void Check(CheckContext context)
		{
			var localItems = context.Of<LocalPathItems>().Value;

			var file = localItems
				.Where(f => Path.GetFileName(f) == "AssemblyInfo.cs")
				.ToList();

			if (file.Count == 0)
				throw new CheckException("Cannot find assembly information file.");

			if (file.Count > 1)
				throw new CheckException($"There should be only one assembly information file, but {file.Count} files were found.");

			if (file[0] == @"Properties\AssemblyInfo.cs")
				return;

			throw new CheckException("Project file should be named 'AssemblyInfo.cs' and placed in 'Properties' folder.");
		}
	}
}
