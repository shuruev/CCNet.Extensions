using System;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Provides additional information about project types.
	/// </summary>
	public static class ProjectTypeExtensions
	{
		/// <summary>
		/// Gets build server name for building this type of projects.
		/// </summary>
		public static string ServerName(this ProjectType projectType)
		{
			switch (projectType)
			{
				case ProjectType.Library:
					return "Library";

				case ProjectType.Website:
				case ProjectType.Webservice:
					return "Website";

				case ProjectType.Service:
					return "Service";

				case ProjectType.Console:
				case ProjectType.Windows:
					return "Application";

				default:
					throw new InvalidOperationException(
						String.Format("Unknown where to build projects of type '{0}'.", projectType));
			}
		}
	}
}
