using System.IO;
using System.Linq;

namespace CCNet.Build.CheckProject
{
	public class AssemblyInfoShouldExist : IChecker
	{
		public void Check(CheckContext context)
		{
			var file = context.LocalFiles.Result
				.Where(f => Path.GetFileName(f) == "AssemblyInfo.cs")
				.ToList();

			if (file.Count == 0)
				throw new FailedCheckException("Cannot find assembly information file.");

			if (file.Count > 1)
				throw new FailedCheckException("There should be one assembly information file, but {0} files were found.", file.Count);

			if (file[0] != @"Properties\AssemblyInfo.cs")
				throw new FailedCheckException("Project file should be named 'AssemblyInfo.cs' and placed in 'Properties' folder.");
		}
	}
}
