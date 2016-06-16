using System;
using System.Xml.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class NuGetPackage
	{
		public string Id { get; private set; }
		public Version Version { get; private set; }
		public TargetFramework Framework { get; private set; }

		public string Title { get; set; }
		public string Tags { get; set; }

		public NuGetPackage(string id, string version, string framework)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");

			if (String.IsNullOrEmpty(version))
				throw new ArgumentNullException("version");

			if (String.IsNullOrEmpty(framework))
				throw new ArgumentNullException("framework");

			// so far we just ignore additional marks, like aplha, beta, preview, prerelase, etc.
			version = version.Split('-')[0];

			Id = id;
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

				case "net452":
					return TargetFramework.Net452;

				case "net461":
					return TargetFramework.Net461;

				default:
					throw new InvalidOperationException(
						String.Format("Unknown target framework '{0}'.", framework));
			}
		}
	}
}
