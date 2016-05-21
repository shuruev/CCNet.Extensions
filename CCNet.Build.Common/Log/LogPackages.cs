using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Represents a collection of referenced packages.
	/// </summary>
	public class LogPackages : Dictionary<string, LogPackage>
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public LogPackages()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		/// <summary>
		/// Reports all referenced packages to a console.
		/// </summary>
		public void Report()
		{
			foreach (var reference in Values.OrderBy(i => i.Location).ThenBy(i => i.ProjectName))
			{
				reference.Report();
			}
		}

		/// <summary>
		/// Prepares textual summary for all referenced packages.
		/// </summary>
		public string Summary()
		{
			if (Count == 0)
				return "No referenced packages.";

			var sb = new StringBuilder();
			sb.AppendLine("Packages:");

			foreach (var reference in Values.OrderBy(i => i.Location).ThenBy(i => i.PackageId))
			{
				sb.AppendFormat(
					"- {0} {1} ({2})",
					reference.PackageId,
					reference.BuildVersion.Normalize(),
					reference.Comment.ToLowerInvariant());

				sb.AppendLine();
			}

			return sb.ToString();
		}
	}
}
