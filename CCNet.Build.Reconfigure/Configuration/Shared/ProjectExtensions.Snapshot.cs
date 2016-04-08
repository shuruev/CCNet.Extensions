using System;

namespace CCNet.Build.Reconfigure
{
	public partial class ProjectExtensions
	{
		public static string TempFileExclude(this IProjectSnapshot project)
		{
			return String.Format(@"{0}\exclude.txt", project.WorkingDirectoryTemp);
		}

		public static string SnapshotFileName(this IProjectSnapshot project)
		{
			return String.Format(@"{0}.snapshot.zip", project.Name);
		}

		public static string TempFileSnapshot(this IProjectSnapshot project)
		{
			return String.Format(@"{0}\{1}", project.WorkingDirectoryTemp, project.SnapshotFileName());
		}
	}
}
