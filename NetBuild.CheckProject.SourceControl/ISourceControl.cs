using System.Collections.Generic;

namespace NetBuild.CheckProject.SourceControl
{
	/// <summary>
	/// Can work with a remote source control location.
	/// </summary>
	public interface ISourceControl
	{
		/// <summary>
		/// Gets child items for a specified path (not recursive).
		/// </summary>
		List<string> GetChildItems(string path);
	}
}
