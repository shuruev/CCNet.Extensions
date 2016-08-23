namespace CCNet.Build.Reconfigure
{
	public partial class ConfigurationBuilder
	{
		private static string GetCategory(IProjectConfiguration config)
		{
			return config.Area;
		}

		private static string GetQueue(IProjectConfiguration config)
		{
			return config.Area;
		}
	}
}
