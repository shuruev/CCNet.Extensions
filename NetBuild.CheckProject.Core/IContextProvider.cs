namespace NetBuild.CheckProject
{
	/// <summary>
	/// Provides data to be used within consistency checks.
	/// </summary>
	public interface IContextProvider
	{
		/// <summary>
		/// Loads required data.
		/// </summary>
		void Load(CheckContext context, CheckArgs args);
	}
}
