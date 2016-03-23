using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class ServiceProjectConfiguration : ConsoleProjectConfiguration
	{
		public override ProjectType Type
		{
			get { return ProjectType.Service; }
		}
	}
}
