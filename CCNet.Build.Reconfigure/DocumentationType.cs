namespace CCNet.Build.Common
{
	/// <summary>
	/// Describes how well documentation for the project should be.
	/// </summary>
	public enum DocumentationType
	{
		/// <summary>
		/// Project does not generate XML documentation file.
		/// </summary>
		None,

		/// <summary>
		/// Project generates XML documentation file, but missing documentation warnings are suppressed.
		/// </summary>
		Partial,

		/// <summary>
		/// Project generates XML documentation file, and there will be warnings for missing documentation.
		/// </summary>
		Full,
	}
}
