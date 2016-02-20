using System;

namespace CCNet.Build.SetupPackages
{
	public class NuGetPackage
	{
		public string Id { get; private set; }
		public string Name { get; private set; }
		public string Area { get; private set; }
		public Version Version { get; private set; }

		public NuGetPackage(string id)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");

			Id = id;

			if (!id.Contains("__"))
			{
				Name = id;
				return;
			}

			var parts = id.Split(new[] { "__" }, 2, StringSplitOptions.None);
			Area = parts[0];
			Name = parts[1];
		}

		public NuGetPackage(string id, string version)
			: this(id)
		{
			if (String.IsNullOrEmpty(version))
				throw new ArgumentNullException("version");

			Version = new Version(version);
		}
	}
}
