using System;

namespace CCNet.Build.SetupPackages
{
	public class NuGetReference
	{
		public string Id { get; private set; }
		public Version Version { get; private set; }
		public string Suffix { get; private set; }

		public NuGetReference(string id, string version)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException(nameof(id));

			Id = id;

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
	}
}
