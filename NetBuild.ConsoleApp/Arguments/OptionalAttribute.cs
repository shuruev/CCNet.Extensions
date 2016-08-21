namespace NetBuild.ConsoleApp
{
	public class OptionalAttribute : ArgumentAttribute
	{
		public OptionalAttribute(string name, string description)
			: base(name, description, false)
		{
		}
	}
}
