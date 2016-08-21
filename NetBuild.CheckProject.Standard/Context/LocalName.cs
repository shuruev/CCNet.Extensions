namespace NetBuild.CheckProject.Standard
{
	public class LocalName : ValueProvider<string>
	{
		public override string Get(CheckContext context)
		{
			return context.Value<ProjectName>();
		}
	}
}
