using Lean.Configuration;

namespace CCNet.Build.CheckProject
{
	public static class Config
	{
		static Config()
		{
			var config = new AppConfigReader();

			TfsUrl = config.Get<string>("Tfs.Url");
		}

		public static string TfsUrl { get; private set; }
	}
}
