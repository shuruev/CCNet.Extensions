using System;
using System.Xml.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class NuGetPackage
	{
		public string Id { get; private set; }
		public string Name { get; private set; }
		public string Area { get; private set; }

		public Version Version { get; private set; }
		public TargetFramework Framework { get; private set; }

		public string Tags { get; set; }

		private NuGetPackage(string id)
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

		public NuGetPackage(string id, string version, string framework)
			: this(id)
		{
			if (String.IsNullOrEmpty(version))
				throw new ArgumentNullException("version");

			if (String.IsNullOrEmpty(framework))
				throw new ArgumentNullException("framework");

			Version = new Version(version);
			Framework = ParseFramework(framework);
		}

		public NuGetPackage(XElement config)
			: this(
				config.Attribute("id").Value,
				config.Attribute("version").Value,
				config.Attribute("targetFramework").Value)
		{
		}

		private static TargetFramework ParseFramework(string framework)
		{
			switch (framework)
			{
				case "net20":
					return TargetFramework.Net20;

				case "net35":
					return TargetFramework.Net35;

				case "net40":
					return TargetFramework.Net40;

				case "net45":
					return TargetFramework.Net45;

				default:
					throw new InvalidOperationException(
						String.Format("Unknown target framework '{0}'.", framework));
			}
		}
	}
}
