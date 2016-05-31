using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCNet.Build.Tfs;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CCNet.Sandbox
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var tfsUrl = "http://rufc-devbuild.cneu.cnwk:8080/tfs/sed";
			//var tfs = new TfsClient(tfsUrl);

			var credentials = new TfsClientCredentials();
			var collection = new TfsTeamProjectCollection(new Uri(tfsUrl), credentials);

			var server = collection.GetService<VersionControlServer>();
			var test = GetChildItems(server);
		}

		public static List<string> GetChildItems(VersionControlServer server)
		{
			/*var path = "$/Main/PartnerAccess/PALibraries/PAUdb/.nuget";
			var items = server.GetItems(path, new ChangesetVersionSpec(2538), RecursionType.OneLevel);*/
			var path = "$/Main/PartnerAccess/PALibraries/PAUdb/udbcodegen";
			//var items = server.GetItems(path, new ChangesetVersionSpec(2538), RecursionType.OneLevel);
			var items = server.GetItems(path, RecursionType.OneLevel);
			return items
				.Items
				.Select(i => i.ServerItem)
				.Where(i => i != path)
				.ToList();
		}
	}
}
