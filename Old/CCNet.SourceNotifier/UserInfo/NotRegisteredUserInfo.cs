using System;

namespace CCNet.SourceNotifier.UserInfo
{
	/// <summary>
	/// UserInfo implementation for unregistered users (ones without accounts in AD).
	/// </summary>
	public class NotRegisteredUserInfo : IUserInfo, IEquatable<NotRegisteredUserInfo>
	{
		private readonly string m_logonName;

		/// <summary>
		/// Gets a value indicating whether there is some additional information on this user or not.
		/// Always returns false.
		/// </summary>
		public bool IsRegistered
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets display name.
		/// </summary>
		public string DisplayName
		{
			get
			{
				return m_logonName;
			}
		}

		/// <summary>
		/// Gets a value indicating whether there is some known email for this user or not.
		/// </summary>
		public bool HasEmail
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
		bool IEquatable<NotRegisteredUserInfo>.Equals(NotRegisteredUserInfo other)
		{
			return m_logonName == other.m_logonName;
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		bool IEquatable<IUserInfo>.Equals(IUserInfo other)
		{
			return (other is NotRegisteredUserInfo) && ((IEquatable<NotRegisteredUserInfo>)this).Equals((NotRegisteredUserInfo)other);
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (obj is NotRegisteredUserInfo) && ((IEquatable<NotRegisteredUserInfo>)this).Equals((NotRegisteredUserInfo)obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			return m_logonName.GetHashCode();
		}
	}
}
