using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectTargetFramework461 : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckRequired("TargetFrameworkVersion", "v4.6.1");
			properties.CheckOptional("TargetFrameworkProfile", String.Empty);
		}
	}
}
