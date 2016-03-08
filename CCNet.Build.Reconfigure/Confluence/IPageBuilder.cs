using CCNet.Build.Confluence;
using CCNet.Build.Tfs;

namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Rebuilds project configuration page in Confluence.
	/// </summary>
	public interface IPageBuilder
	{
		/// <summary>
		/// Checks whether all page properties seems valid.
		/// </summary>
		void CheckPage(string areaName, TfsClient client);

		/// <summary>
		/// Renders new page based on internal properties.
		/// </summary>
		PageDocument BuildPage();
	}
}
