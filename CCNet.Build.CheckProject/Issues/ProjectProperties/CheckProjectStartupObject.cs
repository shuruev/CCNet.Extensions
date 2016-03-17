using System;

namespace CCNet.Build.CheckProject
{
	public class CheckProjectStartupObject : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckOptional("StartupObject", String.Empty);
		}
	}
}
