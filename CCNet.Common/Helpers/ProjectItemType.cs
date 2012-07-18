namespace CCNet.Common
{
	/// <summary>
	/// Project item type.
	/// </summary>
	public enum ProjectItemType
	{
		/// <summary>
		/// None.
		/// </summary>
		None,

		/// <summary>
		/// Compile.
		/// </summary>
		Compile,

		/// <summary>
		/// Content.
		/// </summary>
		Content,

		/// <summary>
		/// Embedded resource.
		/// </summary>
		EmbeddedResource,

		/// <summary>
		/// Entity when using Entity Framework.
		/// </summary>
		EntityDeploy,

		/// <summary>
		/// Resource.
		/// </summary>
		Resource,

		/// <summary>
		/// Shadow.
		/// </summary>
		Shadow,

		/// <summary>
		/// WPF application definition.
		/// </summary>
		ApplicationDefinition,

		/// <summary>
		/// WPF page.
		/// </summary>
		Page,

		/// <summary>
		/// Azure service definition.
		/// </summary>
		ServiceDefinition,

		/// <summary>
		/// Azure service configuration.
		/// </summary>
		ServiceConfiguration,

		/// <summary>
		/// Azure publish profile.
		/// </summary>
		PublishProfile
	}
}
