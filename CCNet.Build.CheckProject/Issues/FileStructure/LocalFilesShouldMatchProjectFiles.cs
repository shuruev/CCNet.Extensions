using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCNet.Build.CheckProject
{
	public class LocalFilesShouldMatchProjectFiles : IChecker
	{
		public void Check(CheckContext context)
		{
			var items = context.LocalFiles.Result
				.Where(item => item != Paths.ProjectFileName + ".vspscc")
				.Where(item => !item.StartsWith(@"$tf\"))
				.Where(item => !item.EndsWith(".DotSettings"))
				.ToList();

			var required = context.ProjectFiles.Result
				.Select(file => file.FullName)
				.Union(new[] { Paths.ProjectFileName })
				.ToList();

			string description;
			if (!CheckEntries(items, required, out description))
			{
				throw new FailedCheckException(
					@"All files under source control should match the files included into project:
{0}
                               
Please include necessary files into project or remove them from source control.",
					description);
			}
		}

		private static bool CheckEntries(IEnumerable<string> items, IEnumerable<string> required, out string description)
		{
			var map = new HashSet<string>(items);
			var input = new List<string>(required);

			description = null;
			var sb = new StringBuilder();

			foreach (var item in input)
			{
				if (!map.Contains(item))
				{
					sb.AppendFormat("- missing required entry '{0}'", item);
					sb.AppendLine();
				}
			}

			foreach (var item in input)
			{
				map.Remove(item);
			}

			foreach (var item in map)
			{
				sb.AppendFormat("- found unexpected entry '{0}'", item);
				sb.AppendLine();
			}

			if (sb.Length == 0)
				return true;

			description = sb.ToString();
			return false;
		}
	}
}
