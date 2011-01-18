namespace CCNet.Common
{
	/// <summary>
	/// Reference item.
	/// </summary>
	public class Reference
	{
		/// <summary>
		/// Gets or sets assembly name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets assembly version.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Gets or sets assembly culture.
		/// </summary>
		public string Culture { get; set; }

		/// <summary>
		/// Gets or sets public key token.
		/// </summary>
		public string PublicKeyToken { get; set; }

		/// <summary>
		/// Gets or sets processor architecture.
		/// </summary>
		public string ProcessorArchitecture { get; set; }

		/// <summary>
		/// Gets or sets specific version.
		/// </summary>
		public string SpecificVersion { get; set; }

		/// <summary>
		/// Gets or sets hint path.
		/// </summary>
		public string HintPath { get; set; }

		/// <summary>
		/// Gets or sets private property.
		/// </summary>
		public string Private { get; set; }

		/// <summary>
		/// Gets or sets aliases property.
		/// </summary>
		public string Aliases { get; set; }

		/// <summary>
		/// Gets or sets embedding options.
		/// </summary>
		public string EmbedInteropTypes { get; set; }

		/// <summary>
		/// Gets or sets required target framework.
		/// </summary>
		public string RequiredTargetFramework { get; set; }

		/// <summary>
		/// Gets or sets executable extension.
		/// </summary>
		public string ExecutableExtension { get; set; }

		/// <summary>
		/// Gets a value indicating whether reference uses specific version.
		/// </summary>
		public bool IsSpecificVersion
		{
			get
			{
				return Version != null && SpecificVersion != "False";
			}
		}
	}
}
