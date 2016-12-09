using System;

namespace CCNet.Build.Reconfigure
{
	public partial class ProjectExtensions
	{
		public static string WorkingDirectoryRelease(this IProjectRelease project)
		{
			return String.Format(@"{0}\release", project.WorkingDirectory);
		}

		public static string ReleaseFileName(this IProjectRelease project, bool isClickOnce = false)
		{
			return isClickOnce 
                ? String.Format(@"{0}.publish.zip", project.Name) 
                : String.Format(@"{0}.zip", project.Name);
		}

		public static string ReleaseFileLocal(this IProjectRelease project, bool isClickOnce = false)
		{
            return String.Format(@"{0}\{1}", project.WorkingDirectoryRelease(), project.ReleaseFileName(isClickOnce));
		}
    }
}
