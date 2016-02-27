using System;
using System.Collections.Generic;
using System.Linq;
using CCNet.Build.Confluence.ConfluenceApi;

namespace CCNet.Build.Confluence
{
	public class ConfluenceClient
	{
		private readonly ConfluenceSoapServiceService m_client;
		private readonly string m_token;

		public ConfluenceClient(string user, string password)
		{
			if (String.IsNullOrEmpty(user))
				throw new ArgumentNullException("user");

			if (String.IsNullOrEmpty(password))
				throw new ArgumentNullException("password");

			m_client = new ConfluenceSoapServiceService();
			m_token = m_client.login(user, password);
		}

		public Page GetPage(string spaceCode, string pageName)
		{
			var page = m_client.getPage(m_token, spaceCode, pageName);
			return ToPage(page);
		}

		public Page GetPage(long pageId)
		{
			var page = m_client.getPage(m_token, pageId);
			return ToPage(page);
		}

		public List<PageSummary> GetChildren(long pageId)
		{
			var children = m_client.getChildren(m_token, pageId);
			return children.Select(ToPageSummary).ToList();
		}

		public List<PageSummary> GetSubtree(long pageId)
		{
			var children = m_client.getDescendents(m_token, pageId);
			return children.Select(ToPageSummary).ToList();
		}

		public void UpdatePage(long pageId, string content)
		{
			var page = m_client.getPage(m_token, pageId);
			if (page.content == content)
				return;

			page.content = content;
			m_client.updatePage(m_token, page, new RemotePageUpdateOptions());
		}

		private static PageSummary ToPageSummary(RemotePageSummary remotePageSummary)
		{
			return new PageSummary
			{
				Id = remotePageSummary.id,
				ParentId = remotePageSummary.parentId,
				Name = remotePageSummary.title
			};
		}

		private static Page ToPage(RemotePage remotePage)
		{
			return new Page
			{
				Id = remotePage.id,
				ParentId = remotePage.parentId,
				Name = remotePage.title,
				Content = remotePage.content
			};
		}
	}
}
