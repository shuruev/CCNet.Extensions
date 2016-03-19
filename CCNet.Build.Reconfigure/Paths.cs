using System.IO;

namespace CCNet.Build.Reconfigure
{
	public static class Paths
	{
		public static string LibraryConfig
		{
			get { return Path.Combine(Args.OutputDirectory, "CCNetLibrary.config"); }
		}

		public static string WebsiteConfig
		{
			get { return Path.Combine(Args.OutputDirectory, "CCNetWebsite.config"); }
		}

		public static string ServiceConfig
		{
			get { return Path.Combine(Args.OutputDirectory, "CCNetService.config"); }
		}
	}
}
