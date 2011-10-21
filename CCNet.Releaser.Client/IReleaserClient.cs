using System.Collections.Generic;

namespace CCNet.Releaser.Client
{
	/// <summary>
	/// Describes a client of Releaser application.
	/// </summary>
	public interface IReleaserClient
	{
		/// <summary>
		/// Gets the list of released builds for specified <paramref name="projectName"/>.
		/// Returns false if project name is unknown.
		/// </summary>
		bool GetReleases(string projectName, out List<string> releases);
	}
}
