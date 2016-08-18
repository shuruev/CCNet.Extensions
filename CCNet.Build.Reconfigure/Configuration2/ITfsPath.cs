using System;

namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project use TFS location.
	/// </summary>
	public interface ITfsPath
	{
		/// <summary>
		/// Gets or sets source control path.
		/// </summary>
		string TfsPath { get; set; }

		/// <summary>
		/// Gets or sets how often source control changes should be checked.
		/// </summary>
		TimeSpan CheckEvery { get; set; }
	}
}
