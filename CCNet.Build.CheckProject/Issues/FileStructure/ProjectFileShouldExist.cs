using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CCNet.Build.CheckProject
{
	public class ProjectFileShouldExist : IChecker
	{
		public void Check(CheckContext context)
		{
			Check(context.LocalFiles.Result, ".csproj");
		}

		public void Check(List<string> files, string extension)
		{
			if (!extension.StartsWith("."))
				throw new ArgumentException("Extension should start with '.' character.");

			var file = files
				.Where(f => Path.GetExtension(f) == extension)
				.ToList();

			if (file.Count == 0)
				throw new FailedCheckException("Cannot find project file '*{0}'.", extension);

			if (file.Count > 1)
				throw new FailedCheckException("There should be one project file '*{0}', but {1} files were found.", extension, file.Count);

			var projectFile = Args.ProjectName + extension;
			if (file[0] != projectFile)
				throw new FailedCheckException("Project file should be named '{0}' and placed in project root folder.", projectFile);
		}
	}
}
