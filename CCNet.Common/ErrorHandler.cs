using System;
using System.Reflection;
using CCNet.Common.Properties;

namespace CCNet.Common
{
	/// <summary>
	/// Handles general errors.
	/// </summary>
	public static class ErrorHandler
	{
		/// <summary>
		/// Handles run-time exception.
		/// </summary>
		public static int Runtime(Exception e)
		{
			Console.Error.WriteLine(
				Resources.ExceptionHtml,
				Assembly.GetEntryAssembly().GetName().Name,
				e.ToString().ToHtml());

			return -1;
		}
	}
}
