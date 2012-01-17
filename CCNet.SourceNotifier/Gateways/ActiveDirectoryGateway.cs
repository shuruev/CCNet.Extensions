using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using CCNet.SourceNotifier.UserInfo;

namespace CCNet.SourceNotifier.Gateways
{
	/// <summary>
	/// Incapsulates all the AD-related functionality.
	/// </summary>
	public class ActiveDirectoryGateway
	{
		/// <summary>
		/// Cache of retrieved UserInfos.
		/// </summary>
		private readonly Dictionary<string, IUserInfo> m_cache = new Dictionary<string, IUserInfo>();

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
		private static IUserInfo DoGetUserInfo(string userName)
		{
			using (var principalContext = new PrincipalContext(ContextType.Domain, GetDomain(userName)))
			{
				var result = UserPrincipal.FindByIdentity(principalContext, userName);
				if (result != null)
				{
					return UserInfoFactory.CreateUserInfo(result);
				}

				return UserInfoFactory.CreateUserInfo(userName);
			}
		}

		/// <summary>
		/// Retrieves the UserInfo by a specified userName, using the results caching.
		/// </summary>
		public IUserInfo GetUserInfo(string userName)
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
