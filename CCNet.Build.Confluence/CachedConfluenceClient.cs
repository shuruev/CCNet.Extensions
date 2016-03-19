using System;
using System.IO;
using CCNet.Build.Common;

namespace CCNet.Build.Confluence
{
	public class CachedConfluenceClient : ConfluenceClient
	{
		private readonly string m_cachePath;

		public CachedConfluenceClient(string user, string password, string cachePath)
			: base(user, password)
		{
			if (String.IsNullOrEmpty(cachePath))
				throw new ArgumentNullException("cachePath");

			m_cachePath = cachePath;
			m_cachePath.CreateDirectoryIfNotExists();
		}

		public Page GetCachedPage(PageSummary summary)
		{
			var cached = GetCachedContent(summary.Id, summary.Version);
			if (cached != null)
			{
				return new Page(summary)
				{
					Content = cached
				};
			}

			var page = GetPage(summary.Id);
			SetCachedContent(page.Id, page.Version, page.Content);

			return page;
		}

		private string GetCachedContent(long pageId, int versionId)
		{
			var file = GetCacheFile(pageId, versionId);
			if (!File.Exists(file))
				return null;

			return File.ReadAllText(file);
		}

		private void SetCachedContent(long pageId, int versionId, string content)
		{
			var file = GetCacheFile(pageId, versionId);
			Path.GetDirectoryName(file).CreateDirectoryIfNotExists();

			File.WriteAllText(file, content);
		}

		private string GetCacheFile(long pageId, int versionId)
		{
			var fileName = String.Format("{0}_{1}.xml", pageId, versionId);
			return Path.Combine(m_cachePath, pageId.ToString(), fileName);
		}
	}
}
