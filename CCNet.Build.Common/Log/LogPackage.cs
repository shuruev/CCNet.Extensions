using System;
using System.Text;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Service class for logging referenced packages.
	/// </summary>
	public class LogPackage
	{
		/// <summary>
		/// Gets or sets package ID.
		/// </summary>
		public string PackageId { get; set; }

		/// <summary>
		/// Gets or sets project name (sometimes it can be different from package ID).
		/// </summary>
		public string ProjectName { get; set; }

		/// <summary>
		/// Gets or sets project URL to display.
		/// </summary>
		public string ProjectUrl { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether current package belongs to the internal company artifacts.
		/// </summary>
		public bool IsLocal { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether current package represents some static files
		/// which were manually published to the internal company artifacts.
		/// </summary>
		public bool IsStatic { get; set; }

		/// <summary>
		/// Gets or sets checked-in version of the package.
		/// </summary>
		public Version SourceVersion { get; set; }

		/// <summary>
		/// Gets or sets a version of the package which was actually used during the build process.
		/// </summary>
		public Version BuildVersion { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this package was referenced as a project.
		/// </summary>
		public bool ProjectReference { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this package should always be pinned to its current version.
		/// </summary>
		public bool PinnedToCurrent { get; set; }

		/// <summary>
		/// Gets or sets a specific version which always should be used when building this package.
		/// </summary>
		public Version PinnedToSpecific { get; set; }

		/// <summary>
		/// Gets textual description of package location.
		/// </summary>
		public string Location
		{
			get
			{
				if (IsLocal)
				{
					return IsStatic ? "Local (static)" : "Local";
				}

				return "Remote";
			}
		}

		/// <summary>
		/// Gets textual description of package build details.
		/// </summary>
		public string Comment
		{
			get
			{
				// special case for snapshots, better to refactor in future
				if (SourceVersion == null && ProjectReference)
					return "Snapshot";

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

		/// <summary>
		/// Reports referenced package to a console.
		/// </summary>
		public void Report()
		{
			if (String.IsNullOrEmpty(PackageId))
				throw new InvalidOperationException("Package ID is not specified.");

			if (String.IsNullOrEmpty(ProjectName))
				throw new InvalidOperationException(
					String.Format("Project name is missing for package '{0}'.", PackageId));

			if (String.IsNullOrEmpty(ProjectUrl))
				throw new InvalidOperationException(
					String.Format("Project URL is missing for package '{0}'.", PackageId));

			if (BuildVersion == null)
				throw new InvalidOperationException(
					String.Format("Build version is missing for package '{0}'.", PackageId));

			string source;
			if (SourceVersion == null)
			{
				source = "n/a";
			}
			else
			{
				source = SourceVersion.Normalize().ToString();
			}

			var build = BuildVersion.Normalize().ToString();

			if (ProjectReference)
			{
				source = source + " (csproj)";
			}

			Execute.ReportPackage(ProjectName, ProjectUrl, source, build, Comment);
		}
	}
}
