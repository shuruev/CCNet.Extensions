using System;
using System.Collections.Generic;
using CCNet.Build.Common;

namespace CCNet.Build.SetupProject
{
	public partial class Program
	{
		private static readonly string m_nugetLogo = $"{Config.NuGetUrl}/Content/Logos/nugetlogo.png";

		private static readonly string m_confluenceLogo = "https://owl.cbsi.com/images/confluence_logo_landing.png";
		private static readonly string m_confluencePageTemplate = "https://owl.cbsi.com/confluence/display/CCSSEDRU/{0}+{1}";

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
					Url = GetNugetUrl(),
					Image = m_nugetLogo
				},
				new
				{
					Url = GetConfluenceUrl("library"),
					Image = m_confluenceLogo
				}
			};
		}

		private static List<dynamic> BuildLinksWebsite()
		{
			return new List<dynamic>
			{
				new
				{
					Url = GetConfluenceUrl("web+site"),
					Image = m_confluenceLogo
				}
			};
		}

		private static List<dynamic> BuildLinksWebservice()
		{
			return new List<dynamic>
			{
				new
				{
					Url = GetConfluenceUrl("web+service"),
					Image = m_confluenceLogo
				}
			};
		}

		private static List<dynamic> BuildLinksService()
		{
			return new List<dynamic>
			{
				new
				{
					Url = GetConfluenceUrl("service"),
					Image = m_confluenceLogo
				}
			};
		}

		private static List<dynamic> BuildLinksConsole()
		{
			return new List<dynamic>
			{
				new
				{
					Url = GetConfluenceUrl("console"),
					Image = m_confluenceLogo
				}
			};
		}

		private static List<dynamic> BuildLinksWindows()
		{
			return new List<dynamic>
			{
				new
				{
					Url = GetConfluenceUrl("application"),
					Image = m_confluenceLogo
				}
			};
		}

		private static List<dynamic> BuildLinksCloudRole()
		{
			return new List<dynamic>
			{
				new
				{
					Url = GetConfluenceUrl("cloud+role"),
					Image = m_confluenceLogo
				}
			};
		}

		private static List<dynamic> BuildLinksCloudService()
		{
			return new List<dynamic>
			{
				new
				{
					Url = GetConfluenceUrl("cloud+service"),
					Image = m_confluenceLogo
				}
			};
		}

		private static object GetConfluenceUrl(string suffix)
		{
			string projectConfluenceName = Args.ProjectName;
			if (Args.BranchName != null)
			{
				projectConfluenceName = $"~+{Args.BranchName}+~+{Args.ProjectName}";
			}

			return string.Format(m_confluencePageTemplate, projectConfluenceName, suffix);
		}

		private static object GetNugetUrl()
		{
			string url = $"{Config.NuGetUrl}/packages/{Args.PackageId}/";
			if (Args.BranchName != null)
			{
				url = $"{Config.NuGetUrl}/private/{Args.BranchName}/packages/__{Args.BranchName.ToLower()}__{Args.PackageId}/";
			}

			return url;
		}
	}
}
