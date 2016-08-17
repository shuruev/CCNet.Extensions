namespace CCNet.Build.Common
{
	/// <summary>
	/// Specified build project type.
	/// </summary>
	public enum ProjectType
	{
		/// <summary>
		/// Class library.
		/// </summary>
		Library,

		/// <summary>
		/// Web site.
		/// </summary>
		Website,

		/// <summary>
		/// Web service.
		/// </summary>
		Webservice,

		/// <summary>
		/// Windows service.
		/// </summary>
		Service,

		/// <summary>
		/// Console application.
		/// </summary>
		Console,

		/// <summary>
		/// Windows application.
		/// </summary>
		Windows,

		/// <summary>
		/// Azure cloud role.
		/// </summary>
		CloudRole,

		/// <summary>
		/// Azure cloud service.
		/// </summary>
		CloudService,

		/// <summary>
		/// Service Fabric service.
		/// </summary>
		FabricService,

		/// <summary>
		/// Service Fabric application.
		/// </summary>
		FabricApplication
	}
}
