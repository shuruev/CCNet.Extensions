using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class LibraryProjectProperties : AssemblyProjectProperties
	{
		public override ProjectType Type
		{
			get { return ProjectType.Library; }
		}
	}
}
