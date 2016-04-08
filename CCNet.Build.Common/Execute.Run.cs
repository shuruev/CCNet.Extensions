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
			using (var process = CreateProcess(fileName, arguments))
			{
				process.Start();

				var output = process.StandardOutput.ReadToEnd();
				var error = process.StandardError.ReadToEnd();

				process.WaitForExit();

				Console.WriteLine(output);

				if (!String.IsNullOrEmpty(error))
					throw new ApplicationException(error);
			}
		}

		/// <summary>
		/// Creates process instance to run.
		/// </summary>
		private static Process CreateProcess(string fileName, string arguments)
		{
			return new Process
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
		}
	}
}
