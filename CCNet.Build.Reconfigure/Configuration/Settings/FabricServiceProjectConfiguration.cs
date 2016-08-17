using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class FabricServiceProjectConfiguration : ConsoleProjectConfiguration, IProjectSnapshot
	{
		public override ProjectType Type => ProjectType.FabricService;

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// remove I02 (ShouldHaveAppConfigDefault)
			checks.Remove("I02");

			return checks;
		}
	}
}
