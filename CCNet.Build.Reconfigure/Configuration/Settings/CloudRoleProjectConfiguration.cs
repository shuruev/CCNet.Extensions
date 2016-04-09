using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class CloudRoleProjectConfiguration : BasicProjectConfiguration, IProjectSnapshot
	{
		public override ProjectType Type
		{
			get { return ProjectType.CloudRole; }
		}

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// remove P04 (ProjectOutputTypeLibrary)
			checks.Remove("P04");

			return checks;
		}
	}
}
