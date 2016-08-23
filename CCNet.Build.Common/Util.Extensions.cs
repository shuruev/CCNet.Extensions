using System;
using System.IO;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Some utility methods which are hard to be categorized yet.
	/// </summary>
	public static partial class Util
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
