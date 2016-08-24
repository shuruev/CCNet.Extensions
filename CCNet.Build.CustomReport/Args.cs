using Atom.Toolbox;
using NetBuild.ConsoleApp;

namespace CCNet.Build.CustomReport
{
	public class Args : ConsoleArgs
	{
		public Args(string[] args)
			: base(args)
		{
		}

		[Optional("confluence", "Specifies page name for Confluence.")]
		public string ConfluencePage => this.Get<string>("confluence", null);

		[Optional("nuget", "Specifies package ID for NuGet gallery.")]
		public string NugetPackage => this.Get<string>("nuget", null);
	}
}
