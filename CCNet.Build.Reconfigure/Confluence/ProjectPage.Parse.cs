using System;
using System.Collections.Generic;
using System.Linq;
using CCNet.Build.Common;

namespace CCNet.Build.Reconfigure
{
	public partial class ProjectPage
	{
		protected static string FindByKey(Dictionary<string, string> properties, Func<string, bool> keyFilter)
		{
			return properties
				.Select(i => new Tuple<string, string>(i.Key.AsciiOnly().ToLowerInvariant(), i.Value))
				.Where(i => keyFilter(i.Item1))
				.Select(i => i.Item2)
				.FirstOrDefault();
		}

		protected static T ParseEnum<T>(
			Dictionary<string, string> properties,
			Func<string, bool> keyFilter,
			Func<string, string> convertValue,
			T byDefault)
			where T : struct
		{
			var value = FindByKey(properties, keyFilter);

			if (value == null)
				return byDefault;

			T result;
			if (Enum.TryParse(convertValue(value), true, out result))
				return result;

			return byDefault;
		}

		protected static T ParseEnum<T>(Dictionary<string, string> properties, string keyContains, T byDefault)
			where T : struct
		{
			return ParseEnum(
				properties,
				key => key.Contains(keyContains),
				value => value.AsciiOnly(),
				byDefault);
		}
	}
}
