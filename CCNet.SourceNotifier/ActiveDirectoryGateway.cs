﻿using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// Incapsulates all the AD-related functionality.
	/// </summary>
	internal class ActiveDirectoryGateway
	{
		/// <summary>
		/// Cache of retrieved UserInfos.
		/// </summary>
		private readonly Dictionary<string, UserInfo> m_cache = new Dictionary<string, UserInfo>();

		/// <summary>
		/// Lock object for m_cache.
		/// </summary>
		private readonly object m_locker = new object();

		/// <summary>
		/// Parses the domain name out of a logon name.
		/// </summary>
		private static string GetDomain(string userName)
		{
			return userName.Contains('\\') ? userName.Substring(0, userName.IndexOf('\\')) : null;
		}

		/// <summary>
		/// Retrieves the UserInfo by a specified userName, bypassing the cache.
		/// </summary>
		private static UserInfo DoGetUserInfo(string userName)
		{
			using (var principalContext = new PrincipalContext(ContextType.Domain, GetDomain(userName)))
			{
				var result = UserPrincipal.FindByIdentity(principalContext, userName);
				if (result != null)
				{
					return UserInfo.CreateUserInfo(result);
				}

				return UserInfo.CreateUserInfo(userName);
			}
		}

		/// <summary>
		/// Retrieves the UserInfo by a specified userName, using the results caching.
		/// </summary>
		public UserInfo GetUserInfo(string userName)
		{
			if (!m_cache.ContainsKey(userName))
			{
				lock (m_locker)
				{
					if (!m_cache.ContainsKey(userName))
					{
						var userInfo = DoGetUserInfo(userName);
						m_cache[userName] = userInfo;
					}
				}
			}

			return m_cache[userName];
		}
	}
}
