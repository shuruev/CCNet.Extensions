namespace CCNet.Common
{
	/// <summary>
	/// Size of virtual machine in Azure role.
	/// </summary>
	public enum VmSize
	{
		/// <summary>
		/// Specifies that size should not be used..
		/// </summary>
		Unspecified,

		/// <summary>
		/// Specifies Extra Small insctance.
		/// </summary>
		ExtraSmall,

		/// <summary>
		/// Specifies Small instance.
		/// </summary>
		Small,

		/// <summary>
		/// Specifies Medium instance.
		/// </summary>
		Medium,

		/// <summary>
		/// Specifies Large instance.
		/// </summary>
		Large,

		/// <summary>
		/// Specifies ExtraLarge instance.
		/// </summary>
		ExtraLarge,
	}
}
