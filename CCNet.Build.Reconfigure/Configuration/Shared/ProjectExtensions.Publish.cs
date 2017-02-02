using System;

namespace CCNet.Build.Reconfigure
{
	public partial class ProjectExtensions
	{
		public static string WorkingDirectoryPublish(this IProjectPublish project)
		{
			return String.Format(@"{0}\publish", project.WorkingDirectory);
		}

		public static string WorkingDirectoryPublishExtensions(this IProjectPublish project)
		{
			return String.Format(@"{0}\publish\Extensions", project.WorkingDirectory);
		}

		public static string PublishFileName(this IProjectPublish project)
		{
			return String.Format(@"{0}.publish.zip", project.Name);
		}

		public static string PublishFileLocal(this IProjectPublish project)
		{
			return String.Format(@"{0}\{1}", project.WorkingDirectoryPublish(), project.PublishFileName());
		}
	}
}
