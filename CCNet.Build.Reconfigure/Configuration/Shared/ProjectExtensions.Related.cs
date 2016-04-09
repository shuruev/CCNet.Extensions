using System;

namespace CCNet.Build.Reconfigure
{
	public partial class ProjectExtensions
	{
		public static string WorkingDirectoryRelated(this IProjectRelated project)
		{
			return String.Format(@"{0}\related", project.WorkingDirectory);
		}
	}
}
