using System.Collections.Generic;

namespace CCNet.ObsoleteCleaner
{
	/// <summary>
	/// Describes detection algorithm of obsolete subfolders.
	/// </summary>
	public interface IObsoleteDetector
	{
		/// <summary>
		/// Returns the list of obsolete subfolders.
		/// </summary>
		IEnumerable<string> GetObsoleteSubfolders(string projectPath);
	}
}
