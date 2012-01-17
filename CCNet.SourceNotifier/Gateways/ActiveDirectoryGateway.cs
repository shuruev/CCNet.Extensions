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
		private readonly Cache<string, IUserInfo> m_cache = new Cache<string, IUserInfo>();

		/// <summary>
		/// Parses the domain name out of a logon name.
		/// </summary>
		private static string GetDomain(string userName)
		{
			return userName.Contains('\\') ? userName.Substring(0, userName.IndexOf('\\')) : null;
		}

		/// <summary>
		/// Retrieves the UserInfo by a specified userName, using the results caching.
		/// </summary>
		public IUserInfo GetUserInfo(string userName)
		{
			return m_cache.Get(
				userName,
				() =>
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
				});
		}
	}
}
