using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class CloudRoleProjectConfiguration : BasicProjectConfiguration, IProjectSnapshot
	{
		public override ProjectType Type
		{
			get { return ProjectType.CloudRole; }
		}
	}
}
