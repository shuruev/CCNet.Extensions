using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public class WebserviceProjectConfiguration : WebsiteProjectConfiguration
	{
		public override ProjectType Type
		{
			get { return ProjectType.Webservice; }
		}
	}
}
