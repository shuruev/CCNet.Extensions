using System.Collections.Generic;

namespace CCNet.Releaser.Client
{
	/// <summary>
	/// Describes a client of Releaser application.
	/// </summary>
	public interface IReleaserClient
	{
		/// <summary>
		/// Returns the list of released builds for specified <paramref name="projectName"/>.
		/// </summary>
		List<string> GetReleases(string projectName);
	}
}
