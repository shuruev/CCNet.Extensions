using System;
using System.Linq;

namespace CCNet.Build.CheckProject
{
	public class CheckProjectPlatforms : IChecker
	{
		public void Check(CheckContext context)
		{
			var project = context.ProjectDocument.Result;
			var platforms = project.GetUsedPlatforms();

			if (platforms.Count != 1)
			{
				throw new FailedCheckException(
					"We agreed project should only have one platform defined, but {0} platforms were found: {1}.",
					platforms.Count,
					String.Join(", ", platforms.Select(name => "'" + name + "'")));
			}

			var platform = platforms[0];

			if (platform != "AnyCPU"
				&& platform != "x64"
				&& platform != "x86")
			{
				throw new FailedCheckException(
					@"Found unknown configuration '{0}'.
Please use standard platforms only, the most common and recommended one is 'Any CPU'.",
					platform);
			}

			var debug = context.ProjectDebugProperties.Result;
			debug.CheckOptional("PlatformTarget", platform);

			var release = context.ProjectReleaseProperties.Result;
			release.CheckOptional("PlatformTarget", platform);

			var condition = project.GetDefaultPlatform();
			if (condition != platform)
			{
				throw new FailedCheckException(
					@"According to the project configuration, the default value for the '$(Platform)' build variable should be set to '{0}', but now it's '{1}'.
This might happen, especially if project platform was changed after the project was initially created with another platform (e.g. from AnyCPU to x64).
You manually edit the project file, by locating the tag <Platform Condition="" '$(Platform)' == '' "">... and changing its value.",
					platform,
					condition);
			}
		}
	}
}
