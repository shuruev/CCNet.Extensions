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

				case ProjectType.CloudRole:
					links = BuildLinksCloudRole();
					break;

				case ProjectType.CloudService:
					links = BuildLinksCloudService();
					break;

				default:
					throw new InvalidOperationException($"Unknown project type '{Args.ProjectType}'.");
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
					Url = $"{Config.NuGetUrl}/packages/{Args.PackageId}/",
					Image = $"{Config.NuGetUrl}/Content/Logos/nugetlogo.png"
				},
				new
				{
					Url = $"https://owl.cbsi.com/confluence/display/CCSSEDRU/{Args.ProjectName}+library",
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
					Url = $"https://owl.cbsi.com/confluence/display/CCSSEDRU/{Args.ProjectName}+web+site",
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
					Url = $"https://owl.cbsi.com/confluence/display/CCSSEDRU/{Args.ProjectName}+web+service",
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
					Url = $"https://owl.cbsi.com/confluence/display/CCSSEDRU/{Args.ProjectName}+service",
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
					Url = $"https://owl.cbsi.com/confluence/display/CCSSEDRU/{Args.ProjectName}+console",
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
					Url = $"https://owl.cbsi.com/confluence/display/CCSSEDRU/{Args.ProjectName}+application",
					Image = "https://owl.cbsi.com/images/confluence_logo_landing.png"
				}
			};
		}

		private static List<dynamic> BuildLinksCloudRole()
		{
			return new List<dynamic>
			{
				new
				{
					Url = $"https://owl.cbsi.com/confluence/display/CCSSEDRU/{Args.ProjectName}+cloud+role",
					Image = "https://owl.cbsi.com/images/confluence_logo_landing.png"
				}
			};
		}

		private static List<dynamic> BuildLinksCloudService()
		{
			return new List<dynamic>
			{
				new
				{
					Url = $"https://owl.cbsi.com/confluence/display/CCSSEDRU/{Args.ProjectName}+cloud+service",
					Image = "https://owl.cbsi.com/images/confluence_logo_landing.png"
				}
			};
		}
	}
}
