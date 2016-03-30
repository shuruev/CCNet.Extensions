using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CCNet.Build.CheckProject
{
	public class CheckAssemblyInfo : IChecker
	{
		public void Check(CheckContext context)
		{
			var lines = File.ReadAllLines(Paths.AssemblyInfoFile);
			var properties = ParseProperties(lines);

			properties.CheckRequired("Title", Args.AssemblyName);
			properties.CheckRequired("Description", String.Empty);
			properties.CheckRequired("Configuration", String.Empty);
			properties.CheckRequired("Company", Args.CompanyName);
			properties.CheckRequired("Product", Args.AssemblyName);

			var copyrightRegex = new Regex(String.Format(@"^Copyright © {0} 20[0-2]\d$", Args.CompanyName));
			properties.CheckRequired(
				"Copyright",
				copyrightRegex.IsMatch,
				String.Format("Usually it should end with '... {0} {1}', or another year.", Args.CompanyName, DateTime.UtcNow.Year));

			properties.CheckRequired("Trademark", String.Empty);
			properties.CheckRequired("Culture", String.Empty);
			properties.CheckRequired("Version", "1.0.0.0");
			properties.CheckOptional("FileVersion", "1.0.0.0");
		}

		private Dictionary<string, string> ParseProperties(IEnumerable<string> lines)
		{
			var propertyRegex = new Regex(@"^\[assembly: Assembly(?<Name>\w+)\((?<Value>.*)\)]$");

			var result = new Dictionary<string, string>();
			foreach (var line in lines)
			{
				if (!propertyRegex.IsMatch(line))
					continue;

				var match = propertyRegex.Match(line);
				var name = match.Groups["Name"].Value;
				var value = match.Groups["Value"].Value.Trim('"');

				result.Add(name, value);
			}

			return result;
		}
	}
}
