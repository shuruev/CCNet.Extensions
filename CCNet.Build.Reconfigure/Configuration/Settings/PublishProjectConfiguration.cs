using System.Collections.Generic;

namespace CCNet.Build.Reconfigure
{
	public abstract class PublishProjectConfiguration : BasicProjectConfiguration
	{
		public string Title { get; set; }

		protected override List<string> GetIssuesToCheck()
		{
			var checks = base.GetIssuesToCheck();

			// project items
			checks.Add("I01"); // ShouldHaveAppConfig
			checks.Add("I02"); // ShouldHaveAppConfigDefault

			return checks;
		}
	}
}
