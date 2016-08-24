namespace CCNet.Build.Reconfigure
{
	/// <summary>
	/// Build project will be built as bundle by merging other libraries.
	/// </summary>
	public interface IMakeBundle
	{
		/// <summary>
		/// Gets other assemblies which should be bundled to the current assembly.
		/// </summary>
		string Bundles { get; }
	}
}
