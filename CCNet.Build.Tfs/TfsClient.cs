using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CCNet.Build.Tfs
{
	public class TfsClient
	{
		private readonly VersionControlServer m_server;

		public TfsClient(string url)
		{
			if (String.IsNullOrEmpty(url))
				throw new ArgumentNullException("url");

			var credentials = new TfsClientCredentials();
			var collection = new TfsTeamProjectCollection(new Uri(url), credentials);

			m_server = collection.GetService<VersionControlServer>();
		}

		public string ReadFile(string path)
		{
			var item = m_server.GetItem(path);
			using (var sr = new StreamReader(item.DownloadFile(), Encoding.UTF8))
			{
				return sr.ReadToEnd();
			}
		}

		public ChangesetSummary GetLatestChangeset(string path)
		{
			var history = m_server.QueryHistory(
				path + "/",
				VersionSpec.Latest,
				0,
				RecursionType.Full,
				null,
				null,
				null,
				1,
				false,
				false,
				false,
				false);

			var changeset = history.Cast<Changeset>().First();
			return new ChangesetSummary
			{
				Id = changeset.ChangesetId,
				User = changeset.Committer,
				UserDisplay = changeset.CommitterDisplayName,
				Date = changeset.CreationDate.ToUniversalTime()
			};
		}

		public List<string> GetChildItems(string path)
		{
			var items = m_server.GetItems(path, RecursionType.OneLevel);
			return items
				.Items
				.Select(i => i.ServerItem)
				.Where(i => i != path)
				.ToList();
		}

		public Dictionary<string, int> GetAllFileEncodings(string path)
		{
			var items = m_server.GetItems(path, VersionSpec.Latest, RecursionType.Full, DeletedState.NonDeleted, ItemType.File);
			return items
				.Items
				.ToDictionary(i => i.ServerItem, i => i.Encoding);
		}
	}
}
