using System;

namespace CCNet.Common.Helpers
{
	/// <summary>
	/// Set of parameters to check for one service.
	/// </summary>
	[Serializable]
	public class ServiceItem : IEquatable<ServiceItem>
	{
		/// <summary>
		/// Gets or sets service name.
		/// </summary>
		public string ServiceName { get; set; }

		/// <summary>
		/// Gets or sets display name.
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// Gets or sets binary path name.
		/// </summary>
		public string BinaryPathName { get; set; }

		/// <summary>
		/// Gets or sets target framework version.
		/// </summary>
		public TargetFramework TargetFrameWork { get; set; }

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ServiceItem other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Equals(other.ServiceName, ServiceName)
				&& Equals(other.DisplayName, DisplayName)
				&& Equals(other.BinaryPathName, BinaryPathName)
				&& Equals(other.TargetFrameWork, TargetFrameWork);
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ServiceItem))
				return false;
			return Equals((ServiceItem)obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				return (ServiceName != null ? ServiceName.GetHashCode() : 0)
					^ (DisplayName != null ? DisplayName.GetHashCode() : 0)
					^ (BinaryPathName != null ? BinaryPathName.GetHashCode() : 0)
					^ (int)TargetFrameWork;
			}
		}

		public static bool operator ==(ServiceItem left, ServiceItem right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ServiceItem left, ServiceItem right)
		{
			return !Equals(left, right);
		}
	}
}
