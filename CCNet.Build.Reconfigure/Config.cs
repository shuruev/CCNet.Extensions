using CCNet.Build.Common;
using Lean.Configuration;

namespace CCNet.Build.Reconfigure
{
	public static class Config
	{
		static Config()
		{
			var config = new AppConfigReader();

			var thumbprint = config.Get<string>("Security.Thumbprint");
			SecureConfig.Initialize(thumbprint);

			ConfluenceUsername = config.Get<string>("Confluence.Username");
			ConfluencePassword = SecureConfig.Decrypt(config.Get<string>("Confluence.Password"));

			CCNetUrl = config.Get<string>("CCNet.Url");
			NuGetUrl = config.Get<string>("NuGet.Url");
			TfsUrl = config.Get<string>("Tfs.Url");
		}

		public static string ConfluenceUsername { get; private set; }
		public static string ConfluencePassword { get; private set; }

		public static string CCNetUrl { get; private set; }
		public static string NuGetUrl { get; private set; }
		public static string TfsUrl { get; private set; }
	}
}
