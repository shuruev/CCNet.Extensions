using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectTargetFramework45 : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckRequired("TargetFrameworkVersion", "v4.5");
			properties.CheckOptional("TargetFrameworkProfile", String.Empty);
		}
	}
}
