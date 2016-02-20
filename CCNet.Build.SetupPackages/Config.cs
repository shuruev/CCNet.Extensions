using Lean.Configuration;

namespace CCNet.Build.SetupPackages
{
	public static class Config
	{
		static Config()
		{
			var config = new AppConfigReader();
			NuGetDbConnection = config.Get<string>("NuGetDb.Connection");
		}

		public static string NuGetDbConnection { get; private set; }
	}
}
