using System;
using CCNet.Common;
using CCNet.ServiceChecker.Properties;

namespace CCNet.ServiceChecker
{
	/// <summary>
	/// Raises all errors.
	/// </summary>
	public static class RaiseError
	{
		/// <summary>
		/// Gets an appropriate exit code.
		/// </summary>
		public static int ExitCode { get; private set; }

		#region Internal methods

		/// <summary>
		/// Raises an error.
		/// </summary>
		private static void RaiseInternal(string message)
		{
			ExitCode = 1;

			Console.Error.WriteLine(
				Resources.DescriptionHtml,
				message.ToHtml());
		}

		#endregion

		#region Service project structure errors

		/// <summary>
		/// Raises "WrongNumberOfServices" error.
		/// </summary>
		public static void WrongNumberOfServices(int numberOfServices)
		{
			RaiseInternal(
				Errors.WrongNumberOfServices
				.Display(numberOfServices));
		}

		#endregion

		#region Service properties errors

		/// <summary>
		/// Raises "WrongServiceItem" error.
		/// </summary>
		public static void WrongServiceItem(string description)
		{
			RaiseInternal(
				Errors.WrongServiceItem
				.Display(description));
		}

		#endregion
	}
}
