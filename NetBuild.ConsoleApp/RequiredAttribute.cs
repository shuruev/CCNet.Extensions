namespace NetBuild.ConsoleApp
{
	public class RequiredAttribute : ArgumentAttribute
	{
		public RequiredAttribute(string name, string description)
			: base(name, description, true)
		{
		}
	}
}
