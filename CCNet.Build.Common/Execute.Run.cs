using System;
using System.Diagnostics;

namespace CCNet.Build.Common
{
	/// <summary>
	/// Running custom tools from command line process.
	/// </summary>
	public static partial class Execute
	{
		/// <summary>
		/// Runs custom tool and redirects its output to the main process.
		/// </summary>
		public static void Run(string fileName, string arguments)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = fileName,
					Arguments = arguments,
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				}
			};

			process.Start();
			process.WaitForExit();

			var output = process.StandardOutput.ReadToEnd();
			Console.WriteLine(output);

			var error = process.StandardError.ReadToEnd();
			if (!String.IsNullOrEmpty(error))
				throw new ApplicationException(error);
		}
	}
}
