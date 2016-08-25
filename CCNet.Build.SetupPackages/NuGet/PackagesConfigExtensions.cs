using System;
using System.Xml.Linq;

namespace CCNet.Build.SetupPackages
{
	public static class PackagesConfigExtensions
	{
		public static NuGetReference AsPackage(this XElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			var id = element.Attribute("id");
			if (id == null)
				throw new InvalidOperationException("Cannot find attribute 'id' in <package> element.");

			var version = element.Attribute("version");
			if (version == null)
				throw new InvalidOperationException("Cannot find attribute 'version' in <package> element.");

			return new NuGetReference(id.Value, version.Value);
		}
	}
}
