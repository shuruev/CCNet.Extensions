using System;
using System.IO;
using CCNet.Common;
using CCNet.ProjectNotifier.Properties;

namespace CCNet.ProjectNotifier
{
	/// <summary>
	/// Notifies other projects about successful build.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// Main program.
		/// </summary>
		public static int Main(string[] args)
		{
			/*xxxargs = new[]
			{
				@"ProjectName=RSDN Editor",
				@"RootPath=\\rufrt-vxbuild\e$\CCNET"
			};*/

			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return 0;
			}

			try
			{
				Arguments.Default = ArgumentProperties.Parse(args);
				PerformNotifications();
			}
			catch (Exception e)
			{
				return ErrorHandler.Runtime(e);
			}

			return 0;
		}

		/// <summary>
		/// Displays usage text.
		/// </summary>
		private static void DisplayUsage()
		{
			Console.WriteLine();
			Console.WriteLine(Resources.UsageInfo);
			Console.WriteLine();
		}

		#region Performing notifications

		/// <summary>
		/// Performs all notifications needed.
		/// </summary>
		private static void PerformNotifications()
		{
			string fileName = ReferenceMark.GetReferenceMarkName(Arguments.ProjectName);
			foreach (string file in Directory.GetFiles(Arguments.RootPath, fileName, SearchOption.AllDirectories))
			{
				ReferenceMark.MarkUpdatedFile(file);

				string path = Path.GetDirectoryName(file);
				path = Path.GetDirectoryName(path);
				path = Path.GetDirectoryName(path);
				string projectName = Path.GetFileName(path);

				Console.WriteLine(
					Resources.LogReferencedBy,
					projectName);
			}
		}

		#endregion
	}
}
