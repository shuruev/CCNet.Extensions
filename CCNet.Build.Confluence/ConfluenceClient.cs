using System;
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

		public string GetPageContent(string spaceCode, string pageName)
		{
			var page = m_client.getPage(m_token, spaceCode, pageName);
			return page.content;
		}
	}
}
