using System;
using System.Collections.Generic;
using CCNet.Common;

namespace CCNet.HttpStatusChecker
{
	/// <summary>
	/// Command line properties for current project.
	/// </summary>
	public static class Arguments
	{
		/// <summary>
		/// Gets or sets a default instance.
		/// </summary>
		public static ArgumentProperties Default { get; set; }

		/// <summary>
		/// Gets the HTTP request method. This can be any valid HTTP method, e.g. GET, POST, etc.
		/// </summary>
		public static string Method
		{
			get { return Default.GetValue("Method"); }
		}

		/// <summary>
		/// Gets the URL of HTTP request.
		/// </summary>
		public static string Url
		{
			get { return Default.GetValue("Url"); }
		}

		/// <summary>
		/// Gets the dictionary of HTTP headers.
		/// </summary>
		public static Dictionary<string, string> Headers
		{
			get { return Default.GetObjectFromJson<Dictionary<string, string>>("Headers"); }
		}

		/// <summary>
		/// Gets the content of HTTP request to send.
		/// </summary>
		public static string Content
		{
			get { return Default.GetValue("Content"); }
		}

		/// <summary>
		/// Gets the timeout period before cancelling the request.
		/// </summary>
		public static TimeSpan Timeout
		{
			get { return Default.GetTimeSpanValue("Timeout"); }
		}

		/// <summary>
		/// Gets the list of exit codes that indicate success.
		/// </summary>
		public static List<int> SuccessStatusCodes
		{
			get { return Default.GetObjectFromJson<List<int>>("SuccessStatusCodes"); }
		}
	}
}
