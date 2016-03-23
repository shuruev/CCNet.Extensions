using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class ConsoleProjectConfiguration : PublishProjectConfiguration
	{
		public override ProjectType Type
		{
			get { return ProjectType.Console; }
		}

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// replace P04 (ProjectOutputTypeLibrary) with P05 (ProjectOutputTypeExe)
			var p04 = checks.IndexOf("P04");
			checks.Insert(p04, "P05");
			checks.Remove("P04");

			return checks;
		}
	}
}
