using System;
using System.Collections.Generic;
using System.Text;

namespace CCNet.Build.CheckProject
{
	public static class CheckExtensions
	{
		private static void CheckValue(string property, string expected, string current)
		{
			if (current != expected)
				throw new FailedCheckException("Property '{0}' is expected to have value '{1}', but now it is '{2}'.", property, expected, current);
		}

		public static void CheckRequired(this IDictionary<string, string> properties, string key, string value)
		{
			if (!properties.ContainsKey(key))
				throw new FailedCheckException("Missing required property '{0}': '{1}'.", key, value);

			CheckValue(key, value, properties[key]);
		}

		public static void CheckRequired(this IDictionary<string, string> properties, string key, Func<string, bool> valueShouldBe, string valueExplanation = null)
		{
			if (!properties.ContainsKey(key))
				throw new FailedCheckException("Missing required property '{0}'.");

			var current = properties[key];
			if (!valueShouldBe(current))
			{
				var sb = new StringBuilder();
				sb.AppendFormat("Property '{0}' seems having improper value '{1}'.", key, current);

				if (!String.IsNullOrEmpty(valueExplanation))
				{
					sb.AppendLine();
					sb.Append(valueExplanation);
				}

				throw new FailedCheckException(sb.ToString());
			}
		}

		public static void CheckOptional(this IDictionary<string, string> properties, string key, string value)
		{
			if (!properties.ContainsKey(key))
				return;

			CheckValue(key, value, properties[key]);
		}
	}
}
