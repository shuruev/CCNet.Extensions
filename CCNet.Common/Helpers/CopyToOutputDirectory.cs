namespace CCNet.Common
{
	/// <summary>
	/// Types of copying to output directory.
	/// </summary>
	public enum CopyToOutputDirectory
	{
		/// <summary>
		/// Do not copy.
		/// </summary>
		None,

		/// <summary>
		/// Copy if newer.
		/// </summary>
		PreserveNewest,

		/// <summary>
		/// Copy always.
		/// </summary>
		Always
	}
}
