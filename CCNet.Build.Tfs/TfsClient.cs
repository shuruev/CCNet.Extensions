using System;
using System.IO;
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
	}
}
