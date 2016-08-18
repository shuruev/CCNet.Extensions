using System;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Base properties representing build project configuration.
	/// </summary>
	public interface IProjectConfiguration : IAdminDirectory, IProjectConfigurationTemp
	{
		/// <summary>
		/// Gets server name.
		/// </summary>
		string Server { get; }

		/// <summary>
		/// Gets project area.
		/// </summary>
		string Area { get; }

		/// <summary>
		/// Gets project name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets branch name.
		/// </summary>
		string Branch { get; }

		/// <summary>
		/// Gets project description.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets owner email (e.g. will be notified about failed builds).
		/// </summary>
		string OwnerEmail { get; }
	}

	public static partial class ProjectConfigurationMethods
	{
		/// <summary>
		/// Gets unique name for the build scenario.
		/// One software project may have several build scenarios, e.g. V3.Storage, V3.Storage-FirstBranch, V3.Storage.SecondBranch, etc.
		/// </summary>
		public static string UniqueName(this IProjectConfiguration config)
		{
			if (String.IsNullOrEmpty(config.Branch))
				return config.Name;

			return $"{config.Name}-{config.Branch}";
		}

		/// <summary>
		/// Gets local name used for project files and folders.
		/// It is a project name where we omit some prefixes, like "CnetConent." for example.
		/// </summary>
		public static string LocalName(this IProjectConfiguration config)
		{
			return Util.ProjectNameToLocalName(config.Name);
		}

		/// <summary>
		/// Gets web URL for a build project.
		/// </summary>
		public static string WebUrl(this IProjectConfiguration config)
		{
			return $"$(serverUrl)/project/{config.UniqueName()}/ViewProjectReport.aspx";
		}

		/// <summary>
		/// Gets base directory for a build project.
		/// </summary>
		public static string ProjectDirectory(this IProjectConfiguration config)
		{
			return $@"$(projectsPath)\{config.UniqueName()}";
		}

		/// <summary>
		/// Gets working directory for a build project.
		/// </summary>
		public static string WorkingDirectory(this IProjectConfiguration config)
		{
			return $@"{config.ProjectDirectory()}\working";
		}
	}
}
