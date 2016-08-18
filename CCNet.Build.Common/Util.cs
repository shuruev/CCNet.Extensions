namespace CCNet.Build.Common
{
	/// <summary>
	/// Some utility methods which are hard to be categorized yet.
	/// </summary>
	public static class Util
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
	}
}
