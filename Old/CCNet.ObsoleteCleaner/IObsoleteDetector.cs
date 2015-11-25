using System.Collections.Generic;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Describes detection algorithm of obsolete subfolders.
	/// </summary>
	public interface IObsoleteDetector
	{
		/// <summary>
		/// Gets the list of obsolete subfolders.
		/// Returns false if project path is unknown.
		/// </summary>
		bool GetObsoleteSubfolders(string projectPath, out List<string> obsoleteSubfolders);
	}
}
