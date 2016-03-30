using System;
using System.Text;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class LogPackage
	{
		public string PackageName { get; set; }
		public string ProjectName { get; set; }
		public bool IsLocal { get; set; }
		public Version SourceVersion { get; set; }
		public Version BuildVersion { get; set; }
		public bool ProjectReference { get; set; }
		public bool PinnedToCurrent { get; set; }
		public Version PinnedToSpecific { get; set; }

		public string Location
		{
			get
			{
				return IsLocal ? "Local" : "Remote";
			}
		}

		public string Comment
		{
			get
			{
				var sb = new StringBuilder(Location);

				if (IsLocal)
				{
					if (PinnedToCurrent)
					{
						sb.Append(", pinned to current version");
					}
					else if (PinnedToSpecific != null)
					{
						sb.AppendFormat(", pinned to version {0}", PinnedToSpecific.Normalize());
					}
				}
				else
				{
					if (!PinnedToCurrent)
					{
						sb.Append(", free update");
					}
				}

				return sb.ToString();
			}
		}

		public void Report()
		{
			if (SourceVersion == null)
				throw new InvalidOperationException(
					String.Format("Source version is missing for package '{0}'.", PackageName));

			if (BuildVersion == null)
				throw new InvalidOperationException(
					String.Format("Build version is missing for package '{0}'.", PackageName));

			var source = SourceVersion.Normalize().ToString();
			var build = BuildVersion.Normalize().ToString();

			if (ProjectReference)
			{
				source = source + " (csproj)";
			}

			Execute.ReportPackage(ProjectName, source, build, Comment);
		}
	}
}
