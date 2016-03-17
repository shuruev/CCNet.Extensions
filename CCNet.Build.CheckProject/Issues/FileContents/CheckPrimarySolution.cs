using System;
using System.Collections.Generic;
using System.Linq;

namespace CCNet.Build.CheckProject
{
	public class CheckPrimarySolution : IChecker
	{
		public void Check(CheckContext context)
		{
			var file = context.TfsSolutionFile.Result;
			var lines = file.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			CheckVisualStudioVersion(lines);
			CheckTeamFoundationServer(lines);
		}

		private void CheckVisualStudioVersion(IList<string> lines)
		{
			if (lines[0].StartsWith("Microsoft Visual Studio Solution File, Format Version 12.")
				&& lines[1] == "# Visual Studio 2013"
				&& lines[2].StartsWith("VisualStudioVersion = 12."))
			{
				return;
			}

			throw new FailedCheckException(@"It looks like solution file was saved using Visual Studio older than 2013.
Please make sure solution file is saved with Visual Studio 2013 or above, so the others could work with it conveniently.");
		}

		private void CheckTeamFoundationServer(IEnumerable<string> lines)
		{
			var tfs = lines.Where(l => l.Contains("SccTeamFoundationServer")).ToList();
			if (tfs.Count == 1 && tfs[0] == "\t\tSccTeamFoundationServer = http://rufc-devbuild.cneu.cnwk:8080/tfs/sed")
				return;

			throw new FailedCheckException(@"Something looks wrong with source control configuration in solution file.
Please make sure both solution and project are properly connected to 'http://rufc-devbuild.cneu.cnwk:8080/tfs/sed'.");
		}
	}
}
