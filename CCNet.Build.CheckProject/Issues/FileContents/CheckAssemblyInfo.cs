using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CCNet.Build.CheckProject
{
	public class CheckAssemblyInfo : IChecker
	{
		private static readonly Regex s_propertyRegex = new Regex(@"^\[assembly: Assembly(?<Name>\w+)\((?<Value>.*)\)]$");
		private static readonly Regex s_copyrightRegex = new Regex(@"^Copyright © CNET Content Solutions 20[0-2]\d$");

		public void Check(CheckContext context)
		{
			var lines = File.ReadAllLines(Paths.AssemblyInfoFile);
			var properties = ParseProperties(lines);

			properties.CheckRequired("Title", Args.ProjectTitle);
			properties.CheckRequired("Description", String.Empty);
			properties.CheckRequired("Configuration", String.Empty);
			properties.CheckRequired("Company", "CNET Content Solutions");
			properties.CheckRequired("Product", Args.ProjectTitle);

			properties.CheckRequired(
				"Copyright",
				value => s_copyrightRegex.IsMatch(value),
				String.Format("Usually it should end with '... CNET Content Solutions {0}', or another year.", DateTime.UtcNow.Year));

			properties.CheckRequired("Trademark", String.Empty);
			properties.CheckRequired("Culture", String.Empty);
			properties.CheckRequired("Version", "1.0.0.0");
			properties.CheckRequired("FileVersion", "1.0.0.0");
		}

		private Dictionary<string, string> ParseProperties(IEnumerable<string> lines)
		{
			var result = new Dictionary<string, string>();

			foreach (var line in lines)
			{
				if (!s_propertyRegex.IsMatch(line))
					continue;

				var match = s_propertyRegex.Match(line);
				var name = match.Groups["Name"].Value;
				var value = match.Groups["Value"].Value.Trim('"');

				result.Add(name, value);
			}

			return result;
		}
	}
}
