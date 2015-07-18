using System;
using System.Text;
using CCNet.Common;
using CCNet.HttpStatusChecker.Properties;

namespace CCNet.HttpStatusChecker
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
				Resources.ErrorDescription,
				message);
		}

		#endregion

		#region Status errors

		/// <summary>
		/// Raises "StatusCodeError" error.
		/// </summary>
		public static void StatusCodeError(int statusCode, string content)
		{
			var sb = new StringBuilder(
				Resources.ErrorWrongStatusCode.Display(statusCode));

			if (!string.IsNullOrWhiteSpace(content))
			{
				sb
					.AppendLine()
					.Append(
						Resources.ErrorResponseContent.Display(content));
			}

			RaiseInternal(sb.ToString());
		}

		#endregion
	}
}
