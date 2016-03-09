using Lean.Configuration;

namespace CCNet.Build.SetupProject
{
	public static class Config
	{
		static Config()
		{
			var config = new AppConfigReader();
			NuGetUrl = config.Get<string>("NuGet.Url");
			TfsUrl = config.Get<string>("Tfs.Url");
		}

		public static string NuGetUrl { get; private set; }
		public static string TfsUrl { get; private set; }
	}
}
