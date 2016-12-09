using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
    public class WindowsProjectConfiguration : ConsoleProjectConfiguration, IProjectPublish
    {
        public bool ClickOnce { get; set; }

        public override ProjectType Type
        {
            get { return ProjectType.Windows; }
        }

        protected override List<string> GetIssuesToCheck()
        {
            var checks = base.GetIssuesToCheck();

            // replace P05 (ProjectOutputTypeExe) with P06 (ProjectOutputTypeWinExe)
            var p05 = checks.IndexOf("P05");
            checks.Insert(p05, "P06");
            checks.Remove("P05");

            // remove I02 (ShouldHaveAppConfigDefault)
            if (ClickOnce)
                checks.Remove("I02");

            return checks;
        }
    }
}
