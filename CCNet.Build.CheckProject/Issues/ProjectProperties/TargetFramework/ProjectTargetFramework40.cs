using System;

namespace CCNet.Build.CheckProject
{
	public class ProjectTargetFramework40 : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckRequired("TargetFrameworkVersion", "v4.0");
			properties.CheckOptional("TargetFrameworkProfile", String.Empty);
		}
	}
}
