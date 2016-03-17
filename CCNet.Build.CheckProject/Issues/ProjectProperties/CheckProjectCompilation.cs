using System;

namespace CCNet.Build.CheckProject
{
	public class CheckProjectCompilation : IChecker
	{
		private static readonly Guid s_webType = new Guid("349C5851-65DF-11DA-9384-00065B846F21");

		public void Check(CheckContext context)
		{
			var debug = context.ProjectDebugProperties.Result;
			debug.CheckRequired("DebugSymbols", "true");
			debug.CheckRequired("DebugType", "full");
			debug.CheckRequired("DefineConstants", "DEBUG;TRACE");
			debug.CheckRequired("ErrorReport", "prompt");
			debug.CheckRequired("Optimize", "false");
			debug.CheckOptional("PlatformTarget", "AnyCPU");
			debug.CheckRequired("WarningLevel", "4");

			var release = context.ProjectReleaseProperties.Result;
			release.CheckOptional("DebugSymbols", "true");
			release.CheckRequired("DebugType", "pdbonly");
			release.CheckRequired("DefineConstants", "TRACE");
			release.CheckRequired("ErrorReport", "prompt");
			release.CheckRequired("Optimize", "true");
			release.CheckOptional("PlatformTarget", "AnyCPU");
			release.CheckRequired("WarningLevel", "4");

			var web = context.ProjectDocument.Result.GetProjectTypeGuids().Contains(s_webType);
			if (web)
			{
				debug.CheckRequired("OutputPath", @"bin\");
				release.CheckRequired("OutputPath", @"bin\");
			}
			else
			{
				debug.CheckRequired("OutputPath", @"bin\Debug\");
				release.CheckRequired("OutputPath", @"bin\Release\");
			}
		}
	}
}
