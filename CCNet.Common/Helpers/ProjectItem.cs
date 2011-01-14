namespace CCNet.Common
{
	/// <summary>
	/// Project item.
	/// </summary>
	public class ProjectItem
	{
		/// <summary>
		/// Gets or sets full item name.
		/// </summary>
		public string FullName { get; set; }

		/// <summary>
		/// Gets or sets project item type.
		/// </summary>
		public ProjectItemType Type { get; set; }

		/// <summary>
		/// Gets or sets copying to output directory option.
		/// </summary>
		public CopyToOutputDirectory CopyToOutput { get; set; }
	}
}
