namespace NetBuild.CheckProject.Standard
{
	public class ProjectFile : ValueProvider<string>
	{
		public override string Get(CheckContext context)
		{
			var name = context.Value<LocalName>();
			var ext = context.Value<ProjectExtension>();

			return $"{name}.{ext}";
		}
	}
}
