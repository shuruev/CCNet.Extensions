using Atom.Toolbox;

namespace CCNet.Build.PrepareProject
{
	public class Config : AppConfigReader
	{
		public string TfsUrl { get; private set; }

		public Config()
		{
			TfsUrl = this.Get<string>("Tfs.Url");
		}
	}
}
