namespace CCNet.Build.Reconfigure
{
	public class ConsoleProjectConfiguration2 :
		IProjectConfiguration,
		IReferencesDirectory,
		ISourceDirectory,
		IPackagesDirectory
	{
		public string Area { get; set; }
		public string Name { get; set; }
		public string Branch { get; set; }
		public string Description { get; set; }
		public string OwnerEmail { get; set; }

		public string Server => "Application";
	}
}
