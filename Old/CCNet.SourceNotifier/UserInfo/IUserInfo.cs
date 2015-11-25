using System;

namespace CCNet.SourceNotifier.UserInfo
{
	/// <summary>
	/// Information about the TFS user.
	/// </summary>
	public interface IUserInfo : IEquatable<IUserInfo>
	{
		/// <summary>
		/// Gets display name.
		/// </summary>
		string DisplayName
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether there is some additional information on this user or not.
		/// Only RegisteredUserInfo has this property returning true.
		/// </summary>
		bool IsRegistered
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether there is some known email for this user or not.
		/// </summary>
		bool HasEmail
		{
			get;
		}
	}
}
