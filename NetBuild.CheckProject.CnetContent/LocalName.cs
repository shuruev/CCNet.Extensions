using NetBuild.CheckProject.Standard;

namespace NetBuild.CheckProject.CnetContent
{
	public class CnetLocalName : LocalName
	{
		public override string Get(CheckContext context)
		{
			const string prefix = "CnetContent.";

			var name = context.Of<ProjectName>().Value;
			if (name.StartsWith(prefix))
				name = name.Substring(prefix.Length);

			return name;
		}
	}
}
