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
		}

		public static string ConfluenceUsername { get; private set; }
		public static string ConfluencePassword { get; private set; }
	}
}
