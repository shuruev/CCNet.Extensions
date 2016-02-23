using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class LogPackages : Dictionary<string, LogPackage>
	{
		public LogPackages()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		public void Report()
		{
			foreach (var reference in Values.OrderBy(i => i.Location).ThenBy(i => i.Name))
			{
				reference.Report();
			}
		}

		public string Summary()
		{
			if (Count == 0)
				return "No referenced packages.";

			var sb = new StringBuilder();
			sb.AppendLine("Packages:");

			foreach (var reference in Values.OrderBy(i => i.Location).ThenBy(i => i.Name))
			{
				sb.AppendFormat(
					"- {0} {1} ({2})",
					reference.Name,
					reference.BuildVersion.Normalize(),
					reference.Comment.ToLowerInvariant());

				sb.AppendLine();
			}

			return sb.ToString();
		}
	}
}
