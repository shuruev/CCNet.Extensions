using System;
using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.SetupProject
{
	public partial class Program
	{
		private static void RenderLinks()
		{
			List<dynamic> links;

			switch (Args.ProjectType)
			{
				case ProjectType.Library:
					links = BuildLinksLibrary();
					break;

				case ProjectType.Website:
					links = BuildLinksWebsite();
					break;

				case ProjectType.Webservice:
					links = BuildLinksWebservice();
					break;

				case ProjectType.Service:
					links = BuildLinksService();
					break;

				case ProjectType.Console:
					links = BuildLinksConsole();
					break;

				case ProjectType.Windows:
					links = BuildLinksWindows();
					break;

				case ProjectType.Cloud:
					links = BuildLinksCloud();
					break;

				default:
					throw new InvalidOperationException(
						String.Format("Unknown project type '{0}'.", Args.ProjectType));
			}

			foreach (var link in links)
			{
				Execute.ReportLink(link.Url, link.Image);
			}
		}

		private static List<dynamic> BuildLinksLibrary()
		{
			return new List<dynamic>
			{
				new
				{
					Url = String.Format("{0}/packages/{1}/", Config.NuGetUrl, Args.PackageId),
					Image = String.Format("{0}/Content/Logos/nugetlogo.png", Config.NuGetUrl)
				},
				new
				{
					Url = String.Format("https://owl.cbsi.com/confluence/display/CCSSEDRU/{0}+library", Args.ProjectName),
					Image = "https://owl.cbsi.com/images/confluence_logo_landing.png"
				}
			};
		}

		private static List<dynamic> BuildLinksWebsite()
		{
			return new List<dynamic>
			{
				new
				{
					Url = String.Format("https://owl.cbsi.com/confluence/display/CCSSEDRU/{0}+web+site", Args.ProjectName),
					Image = "https://owl.cbsi.com/images/confluence_logo_landing.png"
				}
			};
		}

		private static List<dynamic> BuildLinksWebservice()
		{
			return new List<dynamic>
			{
				new
				{
					Url = String.Format("https://owl.cbsi.com/confluence/display/CCSSEDRU/{0}+web+service", Args.ProjectName),
					Image = "https://owl.cbsi.com/images/confluence_logo_landing.png"
				}
			};
		}

		private static List<dynamic> BuildLinksService()
		{
			return new List<dynamic>
			{
				new
				{
					Url = String.Format("https://owl.cbsi.com/confluence/display/CCSSEDRU/{0}+service", Args.ProjectName),
					Image = "https://owl.cbsi.com/images/confluence_logo_landing.png"
				}
			};
		}

		private static List<dynamic> BuildLinksConsole()
		{
			return new List<dynamic>
			{
				new
				{
					Url = String.Format("https://owl.cbsi.com/confluence/display/CCSSEDRU/{0}+console", Args.ProjectName),
					Image = "https://owl.cbsi.com/images/confluence_logo_landing.png"
				}
			};
		}

		private static List<dynamic> BuildLinksWindows()
		{
			return new List<dynamic>
			{
				new
				{
					Url = String.Format("https://owl.cbsi.com/confluence/display/CCSSEDRU/{0}+application", Args.ProjectName),
					Image = "https://owl.cbsi.com/images/confluence_logo_landing.png"
				}
			};
		}

		private static List<dynamic> BuildLinksCloud()
		{
			return new List<dynamic>
			{
				new
				{
					Url = String.Format("https://owl.cbsi.com/confluence/display/CCSSEDRU/{0}+cloud+service", Args.ProjectName),
					Image = "https://owl.cbsi.com/images/confluence_logo_landing.png"
				}
			};
		}
	}
}
