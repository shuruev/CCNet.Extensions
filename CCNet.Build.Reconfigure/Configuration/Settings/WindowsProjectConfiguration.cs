using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class WindowsProjectConfiguration : PublishProjectConfiguration
	{
		public bool ClickOnce { get; set; }

		public override ProjectType Type
		{
			get { return ProjectType.Windows; }
		}

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// replace P04 (ProjectOutputTypeLibrary) with P06 (ProjectOutputTypeWinExe)
			var p04 = checks.IndexOf("P04");
			checks.Insert(p04, "P06");
			checks.Remove("P04");

			return checks;
		}
	}
}
