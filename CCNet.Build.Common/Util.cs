using System.Collections.Generic;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Some utility methods which are hard to be categorized yet.
	/// </summary>
	public static partial class Util
	{
		/// <summary>
		/// Gets local name used for project files and folders.
		/// It is a project name where we omit some prefixes, like "CnetConent." for example.
		/// </summary>
		public static string ProjectNameToLocalName(string projectName)
		{
			const string prefix = "CnetContent.";

			if (!projectName.StartsWith(prefix))
				return projectName;

			return projectName.Substring(prefix.Length);
		}

		/// <summary>
		/// Gets potenial project names which may correspond to the specified local name.
		/// E.g. for the local name "MyProject" it could be just "MyProject" or "CnetContent.MyProject".
		/// </summary>
		public static List<string> LocalNameToProjectNames(string localName)
		{
			const string prefix = "CnetContent.";

			return new List<string>
			{
				localName,
				prefix + localName
			};
		}
	}
}
