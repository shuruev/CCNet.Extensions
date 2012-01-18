using System.DirectoryServices.AccountManagement;

namespace CCNet.SourceNotifier.UserInfo
{
	/// <summary>
	/// UserInfo factory.
	/// </summary>
	public static class UserInfoFactory
	{
		/// <summary>
		/// Creates UserInfo instance for the specified UserPrincipal information object from AD.
		/// </summary>
		public static IUserInfo CreateUserInfo(UserPrincipal userPrincipal)
		{
			return new RegisteredUserInfo(userPrincipal);
		}

		/// <summary>
		/// Creates UserInfo instance for the specified logonName, if no other information is available.
		/// </summary>
		public static IUserInfo CreateUserInfo(string logonName)
		{
			return new NotRegisteredUserInfo(logonName);
		}
	}
}
