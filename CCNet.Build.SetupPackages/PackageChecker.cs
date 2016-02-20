using System;
using System.Collections.Generic;
using System.Linq;

namespace CCNet.Build.SetupPackages
{
	public class PackageChecker
	{
		private readonly NuGetDb m_db;

		private Dictionary<string, NuGetPackage> m_localPackages;
		private Dictionary<string, NuGetPackage> m_customVersions;

		public PackageChecker(string nugetDbConnection, string customVersions)
		{
			m_db = new NuGetDb(nugetDbConnection);

			m_localPackages = new Dictionary<string, NuGetPackage>();
			m_customVersions = new Dictionary<string, NuGetPackage>();

			if (!String.IsNullOrEmpty(customVersions))
			{
				ParseCustomVersions(customVersions);
			}
		}

		public int PackageCount
		{
			get { return m_localPackages.Count; }
		}

		private void ParseCustomVersions(string customVersions)
		{
			var versions = new List<NuGetPackage>();

			foreach (string item in customVersions.Split('|'))
			{
				if (!item.Contains('+'))
				{
					// empty version means package should be pinned to its current version
					versions.Add(new NuGetPackage(item));
					continue;
				}

				var parts = item.Split('+');
				var name = parts[0];
				var version = parts[1];
				versions.Add(new NuGetPackage(name, version));
			}

			m_customVersions = versions.ToDictionary(p => p.Id, StringComparer.OrdinalIgnoreCase);
		}

		public void Load()
		{
			m_localPackages = m_db.GetLatestVersions().ToDictionary(p => p.Id, StringComparer.OrdinalIgnoreCase);
		}

		public bool IsLocal(string name)
		{
			return m_localPackages.ContainsKey(name);
		}

		public bool IsPinnedToCurrentVersion(string name)
		{
			if (!m_customVersions.ContainsKey(name))
				return false;

			var version = m_customVersions[name].Version;
			if (version == null)
				return true;

			if (!IsLocal(name))
				throw new InvalidOperationException("Only local packages can be pinned to specific version.");

			return false;
		}

		public Version VersionToUse(string name)
		{
			if (!IsLocal(name))
				throw new InvalidOperationException("Only local packages can be updated to a specifc version.");

			if (IsPinnedToCurrentVersion(name))
				throw new InvalidOperationException("This package is pinned to its current version.");

			if (m_customVersions.ContainsKey(name))
			{
				var custom = m_customVersions[name];
				return custom.Version;
			}

			return m_localPackages[name].Version;
		}
	}
}
