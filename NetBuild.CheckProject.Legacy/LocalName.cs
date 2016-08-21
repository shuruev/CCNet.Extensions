using NetBuild.CheckProject.Standard;

namespace NetBuild.CheckProject.Legacy
{
	public class CnetLocalName : LocalName
	{
		public override string Get(CheckContext context)
		{
			const string prefix = "CnetContent.";

			var name = context.Value<ProjectName>();
			if (name.StartsWith(prefix))
				name = name.Substring(prefix.Length);

			return name;
		}
	}
}
