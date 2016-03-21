using System;
using System.Collections.Generic;
using System.Xml.Linq;
using CCNet.Build.Confluence;
using CCNet.Build.Tfs;

namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Holds project configuration and builds pages for Confluence.
	/// </summary>
	public interface IProjectPage
	{
		/// <summary>
		/// Gets key for sorting order when displaying.
		/// </summary>
		string OrderKey { get; }

		/// <summary>
		/// Checks whether project configuration seems valid.
		/// </summary>
		void CheckPage(TfsClient client);

		/// <summary>
		/// Renders new page based on project configuration.
		/// </summary>
		PageDocument RenderPage();

		/// <summary>
		/// Renders table row for a summary page.
		/// </summary>
		XElement RenderSummaryRow(bool forArea);

		/// <summary>
		/// Exports configurations for build server.
		/// </summary>
		List<ProjectConfiguration> ExportConfigurations();

		/// <summary>
		/// Exports unique ID for referencing this project.
		/// </summary>
		Tuple<string, Guid> ExportMap();
	}
}
