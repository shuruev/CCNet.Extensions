using System;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Reporting special output markers which are used by the other tools.
	/// </summary>
	public static partial class Execute
	{
		/// <summary>
		/// Reports package used by the current project.
		/// </summary>
		public static void ReportPackage(string name, string source, string build, string notes)
		{
			Console.WriteLine(
				"[PACKAGE] {0} | {1} | {2} | {3}",
				name,
				source,
				build,
				notes);
		}

		/// <summary>
		/// Reports which projects are using the current project.
		/// </summary>
		public static void ReportUsage(string name, string server)
		{
			Console.WriteLine(
				"[USAGE] {0} | {1}",
				name,
				server);
		}

		/// <summary>
		/// Reports URL to be displayed for the current project.
		/// </summary>
		public static void ReportLink(string url, string image)
		{
			Console.WriteLine(
				"[LINK] {0} | {1}",
				url,
				image);
		}
	}
}
