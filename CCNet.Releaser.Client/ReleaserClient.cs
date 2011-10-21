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

			ProjectData projects = m_engine.GetProjects();
			m_projectIds = projects.Project
				.Select()
				.Cast<ProjectData.ProjectRow>()
				.Where(row => row.StorageCode == "PublicationStorage")
				.ToDictionary(row => row.StoragePath, row => row.ProjectUid);
		}

		/// <summary>
		/// Returns the list of released builds for specified <paramref name="projectName"/>.
		/// </summary>
		public List<string> GetReleases(string projectName)
		{
			if (!m_projectIds.ContainsKey(projectName))
				throw new InvalidOperationException(
					String.Format("Unkonwn project {0}.", projectName));

			Guid projectUid = m_projectIds[projectName];
			ReleaseData releases = m_engine.GetReleases(projectUid);

			return releases.Release
					.Select()
					.Cast<ReleaseData.ReleaseRow>()
					.Select(row => row.VersionCode)
					.ToList();
		}
	}
}
