using System;
using System.DirectoryServices.AccountManagement;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// Information about the TFS user.
	/// </summary>
	internal abstract class UserInfo : IEquatable<UserInfo>
	{
		public class NotRegisteredUserInfo : UserInfo, IEquatable<NotRegisteredUserInfo>
		{
			private readonly string m_logonName;

			/// <summary>
			/// Gets whether there is some additional information on this user or not.
			/// Always returns false.
			/// </summary>
			public override bool IsRegistered
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Gets display name.
			/// </summary>
			public override string DisplayName
			{
				get
				{
					return m_logonName;
				}
			}

			/// <summary>
			/// Gets whether there is some known email for this user or not.
			/// </summary>
			public override bool HasEmail
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			public NotRegisteredUserInfo(string logonName)
			{
				m_logonName = logonName;
			}

			/// <summary>
			/// Indicates whether the current object is equal to another object of the same type.
			/// </summary>
			public bool Equals(NotRegisteredUserInfo other)
			{
				return m_logonName == other.m_logonName;
			}

			/// <summary>
			/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
			/// </summary>
			public override bool Equals(object obj)
			{
				return (obj is NotRegisteredUserInfo) && Equals((NotRegisteredUserInfo)obj);
			}

			/// <summary>
			/// Serves as a hash function for a particular type.
			/// </summary>
			public override int GetHashCode()
			{
				return m_logonName.GetHashCode();
			}
		}

		/// <summary>
		/// UserInfo implementation for registered users (ones with accounts in AD).
		/// </summary>
		public class RegisteredUserInfo : UserInfo, IEquatable<RegisteredUserInfo>
		{
			private readonly UserPrincipal m_userPrincipal;

			/// <summary>
			/// Gets whether there is some additional information on this user or not.
			/// Always returns true.
			/// </summary>
			public override bool IsRegistered
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Gets display name.
			/// </summary>
			public override string DisplayName
			{
				get
				{
					return m_userPrincipal.DisplayName;
				}
			}

			/// <summary>
			/// Gets whether there is some known email for this user or not.
			/// </summary>
			public override bool HasEmail
			{
				get
				{
					return !String.IsNullOrEmpty(EmailAddress);
				}
			}

			/// <summary>
			/// Gets the user first name.
			/// </summary>
			public string FirstName
			{
				get
				{
					return m_userPrincipal.GivenName;
				}
			}

			/// <summary>
			/// Gets the email address.
			/// </summary>
			public string EmailAddress
			{
				get
				{
					return m_userPrincipal.EmailAddress;
				}
			}

			/// <summary>
			/// Gets the last AD account logon time, if any.
			/// </summary>
			public DateTime? LastLogon
			{
				get
				{
					return m_userPrincipal.LastLogon;
				}
			}

			/// <summary>
			/// Gets a value indicating whether the AD account is locked out.
			/// </summary>
			public bool IsLockedOut
			{
				get
				{
					return m_userPrincipal.IsAccountLockedOut();
				}
			}

			/// <summary>
			/// Gets user description.
			/// </summary>
			public string Description
			{
				get
				{
					return m_userPrincipal.Description;
				}
			}

			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			public RegisteredUserInfo(UserPrincipal userPrincipal)
			{
				m_userPrincipal = userPrincipal;
			}

			/// <summary>
			/// Indicates whether the current object is equal to another object of the same type.
			/// </summary>
			public bool Equals(RegisteredUserInfo other)
			{
				return m_userPrincipal.Equals(other.m_userPrincipal);
			}

			/// <summary>
			/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
			/// </summary>
			public override bool Equals(object obj)
			{
				return (obj is RegisteredUserInfo) && Equals((RegisteredUserInfo)obj);
			}

			/// <summary>
			/// Serves as a hash function for a particular type.
			/// </summary>
			public override int GetHashCode()
			{
				return m_userPrincipal.GetHashCode();
			}
		}

		/// <summary>
		/// Gets a value indicating whether there is some additional information on this user or not.
		/// Only RegisteredUserInfo has this property returning true.
		/// </summary>
		public abstract bool IsRegistered
		{
			get;
		}

		/// <summary>
		/// Gets display name.
		/// </summary>
		public abstract string DisplayName
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether there is some known email for this user or not.
		/// </summary>
		public abstract bool HasEmail
		{
			get;
		}

		/// <summary>
		/// Creates UserInfo instance for the specified UserPrincipal information object from AD.
		/// </summary>
		public static UserInfo CreateUserInfo(UserPrincipal userPrincipal)
		{
			return new RegisteredUserInfo(userPrincipal);
		}

		/// <summary>
		/// Creates UserInfo instance for the specified logonName, if no other information is available.
		/// </summary>
		public static UserInfo CreateUserInfo(string logonName)
		{
			return new NotRegisteredUserInfo(logonName);
		}

		public abstract override bool Equals(object obj);

		public abstract override int GetHashCode();

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		public bool Equals(UserInfo other)
		{
			return Equals((object)other);
		}
	}
}
