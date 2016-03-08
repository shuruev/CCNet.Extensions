using Lean.Configuration;

namespace CCNet.Build.SetupProject
{
	public static class Config
	{
		static Config()
		{
			var config = new AppConfigReader();
			NuGetUrl = config.Get<string>("NuGetDb.Url");
		}

		public static string NuGetUrl { get; private set; }
	}
}
