namespace NetBuild.CheckProject.Standard
{
	/// <summary>
	/// Gets company name.
	/// E.g. "ACME Corporation".
	/// </summary>
	public class CompanyName : ConfigurationValue<string>
	{
		public CompanyName()
			: base("CompanyName")
		{
		}
	}
}
