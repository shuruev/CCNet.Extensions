using System.Collections.Generic;

namespace CCNet.Releaser.Client
{
	/// <summary>
	/// Implementation of a client of Releaser application.
	/// </summary>
	public class ReleaserClient : IReleaserClient
	{
		/// <summary>
		/// Returns the list of released builds for specified <paramref name="projectName"/>.
		/// </summary>
		public List<string> GetReleases(string projectName)
		{
			return new List<string> { "11.10.4.1" };
		}
	}
}
