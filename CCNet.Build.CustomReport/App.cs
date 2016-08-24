using System;
using CCNet.Build.Common;
using NetBuild.ConsoleApp;

namespace CCNet.Build.CustomReport
{
	public class App : ConsoleApp<Args>
	{
		protected override void Run()
		{
			ReportNuget();
			ReportConfluence();
		}

		private void ReportNuget()
		{
			if (String.IsNullOrEmpty(m_args.NugetPackage))
				return;

			const string nugetUrl = "http://rufc-devbuild.cneu.cnwk/nuget";

			var packagesUrl = $"{nugetUrl}/packages/{m_args.NugetPackage}/";
			if (m_args.NugetPackage.Contains("-"))
			{
				var parts = m_args.NugetPackage.Split(new[] { '-' }, 2);
				var name = parts[0];
				var branch = parts[1];
				packagesUrl = $"{nugetUrl}/private/{branch}/packages/{name}/";
			}

			Execute.ReportLink(packagesUrl, $"{nugetUrl}/Content/Logos/nugetlogo.png");
		}

		private void ReportConfluence()
		{
			if (String.IsNullOrEmpty(m_args.ConfluencePage))
				return;

			var pageUrl = $"https://owl.cbsi.com/confluence/display/CCSSEDRU/{m_args.ConfluencePage}";

			Execute.ReportLink(pageUrl, "https://owl.cbsi.com/images/confluence_logo_landing.png");
		}
	}
}
