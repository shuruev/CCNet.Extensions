using System;

namespace CCNet.Build.SetupPackages
{
	public class NuGetReference
	{
		public string Id { get; private set; }
		public Version Version { get; private set; }
		public string Suffix { get; private set; }
		public string Branch { get; private set; }

		public NuGetReference(string id, string version)
		{

			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException(nameof(id));

			string prefix = null;

			Id = ParseId(id, out prefix);

			Branch = prefix;

			if (String.IsNullOrEmpty(version))
				throw new ArgumentNullException(nameof(version));

			if (version.Contains("-"))
			{
				var parts = version.Split(new[] { '-' }, 2);
				Version = new Version(parts[0]);
				Suffix = parts[1];
			}
			else
			{
				Version = new Version(version);
			}
		}

		private static string ParseId(string id, out string prefix)
		{
			prefix = null;

			if (String.IsNullOrEmpty(id))
				return null;

			var pair = id.Split(new[] { "__" }, StringSplitOptions.None);
			if (pair.Length != 3
				|| pair[0] != string.Empty
				|| pair[1].Length == 0
				|| pair[2].Length == 0)
			{
				return id;
			}

			prefix = pair[1];

			return pair[2];
		}
	}
}
