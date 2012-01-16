using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// Incapsulates all the TFS-related functionality.
	/// </summary>
	internal class TeamFoundationServerGateway : IDisposable
	{
		/// <summary>
		/// The repository root dir.
		/// </summary>
		private const string TFS_ROOT_DIR = "$/";

		/// <summary>
		/// TFS configuration server.
		/// </summary>
		private readonly TfsConfigurationServer m_tfsConfigurationServer;

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
		public TeamFoundationServerGateway(Uri uri, string collectionName)
		{
			m_tfsConfigurationServer = TfsConfigurationServerFactory.GetConfigurationServer(uri);

			try
			{
				var collectionsNodes = m_tfsConfigurationServer.CatalogNode.QueryChildren(
					new[] { CatalogResourceTypes.ProjectCollection },
					false,
					CatalogQueryOptions.None);
				var collectionNode = collectionsNodes.Single(node => node.Resource.DisplayName == collectionName);
				var collectionGuid = new Guid(collectionNode.Resource.Properties["InstanceId"]);
				m_tfsTeamProjectCollection = m_tfsConfigurationServer.GetTeamProjectCollection(collectionGuid);

				m_versionControlServer = m_tfsTeamProjectCollection.GetService<VersionControlServer>();
			}
			catch (Exception)
			{
				m_tfsConfigurationServer.Dispose();
				throw;
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			m_tfsConfigurationServer.Dispose();
		}

		/// <summary>
		/// Returns the list of all old pending changes.
		/// </summary>
		public IEnumerable<Tuple<PendingSet, PendingChange>> GetOldPendingChanges(DateTime olderThan)
		{
			return
				from set in m_versionControlServer.GetPendingSets(new[] { TFS_ROOT_DIR }, RecursionType.Full)
				from change in set.PendingChanges
				where change.CreationDate < olderThan
				select new Tuple<PendingSet, PendingChange>(set, change);
		}
	}
}
