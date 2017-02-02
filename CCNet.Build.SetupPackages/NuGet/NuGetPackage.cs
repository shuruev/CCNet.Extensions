using System;
using CCNet.Build.Common;

namespace CCNet.Build.SetupPackages
{
	public class NuGetPackage : NuGetReference
	{
		public TargetFramework Framework { get; private set; }
		public string Title { get; private set; }
		public string Tags { get; private set; }

		public NuGetPackage(string id, string version, string framework, string title, string tags)
			: base(id, version)
		{
			if (String.IsNullOrEmpty(framework))
				throw new ArgumentNullException(nameof(framework));

			Framework = ParseFramework(framework);
			Title = title;
			Tags = tags;
		}

		protected static TargetFramework ParseFramework(string framework)
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

				case "net462":
					return TargetFramework.Net462;

				default:
					throw new InvalidOperationException($"Unknown target framework '{framework}'.");
			}
		}
	}
}
