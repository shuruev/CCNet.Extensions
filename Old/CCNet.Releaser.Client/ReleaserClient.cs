using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using Releaser.Interfaces;

namespace CCNet.Releaser.Client
{
	/// <summary>
	/// Implementation of a client of Releaser application.
	/// </summary>
	public class ReleaserClient : IReleaserClient
	{
		private static readonly string s_url = "tcp://rufrt-vxbuild:8888/ReleaserServer/IReleaserEngine";

		private readonly IReleaserEngine m_engine;
		private readonly Dictionary<string, Guid> m_projectIds;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ReleaserClient()
		{
			RemotingConfiguration.Configure(
				AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
				true);

			m_engine = (IReleaserEngine)Activator.GetObject(typeof(IReleaserEngine), s_url);

			ProjectData projectData = m_engine.GetProjects();
			m_projectIds = projectData.Project
				.Select()
				.Cast<ProjectData.ProjectRow>()
				.Where(row => row.StorageCode == "PublicationStorage")
				.ToDictionary(row => row.StoragePath, row => row.ProjectUid);
		}

		/// <summary>
		/// Gets the list of released builds for specified <paramref name="projectName"/>.
		/// Returns false if project name is unknown.
		/// </summary>
		public bool GetReleases(string projectName, out List<string> releases)
		{
			releases = new List<string>();

			if (!m_projectIds.ContainsKey(projectName))
				return false;

			Guid projectUid = m_projectIds[projectName];
			ReleaseData releaseData = m_engine.GetReleases(projectUid);

			releases = releaseData.Release
					.Select()
					.Cast<ReleaseData.ReleaseRow>()
					.Select(row => row.VersionCode)
					.ToList();

			return true;
		}
	}
}
