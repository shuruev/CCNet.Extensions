using System;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class LogPackage
	{
		public string Name { get; set; }
		public bool IsLocal { get; set; }
		public Version SourceVersion { get; set; }
		public Version BuildVersion { get; set; }
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
				string pinned = null;
				if (PinnedToCurrent)
				{
					pinned = "pinned to its current version";
				}
				else
				{
					if (PinnedToSpecific != null)
					{
						pinned = String.Format("pinned to version {0}", PinnedToSpecific.Normalize());
					}
				}

				if (pinned == null)
					return Location;

				return String.Join(", ", Location, pinned);
			}
		}

		public void Report()
		{
			if (SourceVersion == null)
				throw new InvalidOperationException(
					String.Format("Source version is missing for package '{0}'.", Name));

			if (BuildVersion == null)
				throw new InvalidOperationException(
					String.Format("Build version is missing for package '{0}'.", Name));

			Console.WriteLine(
				"[PACKAGE] {0} | {1} | {2} | {3}",
				Name,
				SourceVersion.Normalize(),
				BuildVersion.Normalize(),
				Comment);
		}
	}
}
