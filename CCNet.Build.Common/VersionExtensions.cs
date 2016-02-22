using System;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Extension methods for Version class.
	/// </summary>
	public static class VersionExtensions
	{
		/// <summary>
		/// Converts version to a form where all four components are specified.
		/// </summary>
		public static Version Normalize(this Version version)
		{
			if (version == null)
				return null;

			if (version.Build >= 0 && version.Revision >= 0)
				return version;

			return new Version(
				version.Major,
				version.Minor,
				Math.Max(version.Build, 0),
				Math.Max(version.Revision, 0));
		}
	}
}
