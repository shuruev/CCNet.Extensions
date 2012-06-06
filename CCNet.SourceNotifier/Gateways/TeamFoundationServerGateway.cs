using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CCNet.SourceNotifier.Gateways
{
	/// <summary>
	/// Incapsulates all the TFS-related functionality.
	/// </summary>
	public class TeamFoundationServerGateway : IDisposable
	{
		/// <summary>
		/// The repository root dir.
		/// </summary>
		private const string c_tfsRootDir = "$/";

		/// <summary>
		/// TFS projects collection.
		/// </summary>
		private readonly TfsTeamProjectCollection m_tfsTeamProjectCollection;

		/// <summary>
		/// TFS version control server.
		/// </summary>
		private readonly VersionControlServer m_versionControlServer;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public TeamFoundationServerGateway(Uri uri)
		{
			m_tfsTeamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(uri);
			m_versionControlServer = (VersionControlServer)m_tfsTeamProjectCollection.GetService(typeof(VersionControlServer));
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			m_tfsTeamProjectCollection.Dispose();
		}

		/// <summary>
		/// Returns the list of all old pending changes.
		/// </summary>
		public IEnumerable<Tuple<PendingSet, PendingChange>> GetOldPendingChanges(DateTime olderThan)
		{
			return
				from set in m_versionControlServer.GetPendingSets(new[] { c_tfsRootDir }, RecursionType.Full)
				from change in set.PendingChanges
				where change.CreationDate < olderThan
				select new Tuple<PendingSet, PendingChange>(set, change);
		}
	}
}
