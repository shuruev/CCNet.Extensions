using System;
using Lean.Configuration;

namespace CCNet.Build.AzureDownload
{
	public static class Config
	{
		private static readonly IConfigReader s_config;

		static Config()
		{
			s_config = new AppConfigReader();
		}

		public static string AccountName(string storage)
		{
			return s_config.Get<string>(String.Format("Storage.{0}.AccountName", storage));
		}

		public static string AccountKey(string storage)
		{
			return s_config.Get<string>(String.Format("Storage.{0}.AccountKey", storage));
		}
	}
}
