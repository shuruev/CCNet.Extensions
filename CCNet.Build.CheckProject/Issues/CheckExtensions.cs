using System;
using System.Collections.Generic;
using System.Text;

namespace CCNet.Build.CheckProject
{
	public static class CheckExtensions
	{
		private static void RaiseError(string details, string format, params object[] args)
		{
			var sb = new StringBuilder();
			sb.AppendFormat(format, args);

			if (!String.IsNullOrEmpty(details))
			{
				sb.AppendLine();
				sb.Append(details);
			}

			throw new FailedCheckException(sb.ToString());
		}

		private static void CheckValue(string details, string property, string expected, string current)
		{
			if (current == expected)
				return;

			RaiseError(details, "Property '{0}' is expected to have value '{1}', but now it is '{2}'.", property, expected, current);
		}

		private static void CheckValue(string details, string property, Func<string, bool> expected, string current)
		{
			if (expected(current))
				return;

			RaiseError(details, "Property '{0}' seems having improper value '{1}'.", property, current);
		}

		public static void CheckRequired(this IDictionary<string, string> properties, string key, string value, string details = null)
		{
			if (!properties.ContainsKey(key))
			{
				RaiseError(details, "Missing required property '{0}': '{1}'.", key, value);
			}

			CheckValue(details, key, value, properties[key]);
		}

		public static void CheckRequired(this IDictionary<string, string> properties, string key, Func<string, bool> shouldBe, string details = null)
		{
			if (!properties.ContainsKey(key))
			{
				RaiseError(details, "Missing required property '{0}'.", key);
			}

			CheckValue(details, key, shouldBe, properties[key]);
		}

		public static void CheckOptional(this IDictionary<string, string> properties, string key, string value, string details = null)
		{
			if (!properties.ContainsKey(key))
				return;

			CheckValue(details, key, value, properties[key]);
		}

		public static void CheckOptional(this IDictionary<string, string> properties, string key, Func<string, bool> shouldBe, string details = null)
		{
			if (!properties.ContainsKey(key))
				return;

			CheckValue(details, key, shouldBe, properties[key]);
		}
	}
}
