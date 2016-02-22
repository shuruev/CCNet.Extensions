using System;
using System.Collections.Generic;

namespace CCNet.Build.SetupPackages
{
	public class LogPackages : Dictionary<string, LogPackage>
	{
		public LogPackages()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}
	}
}
