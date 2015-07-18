using System;
using System.Net;
using System.Text;
using CCNet.Common;
using CCNet.HttpStatusChecker.Properties;

namespace CCNet.HttpStatusChecker
{
	public class Program
	{
		public static int Main(string[] args)
		{
			/*xxxargs = new[]
			{
				@"Method=POST",
				@"Url=http://api.hipchat.com/v2/room/1337130/notification",
				@"Headers={""Authorization"":""Bearer l0dAqgEbQh0xJhP4DAj6wxmboziUdKt423HyvjoX"",""Content-Type"":""application/json""}",
				@"Content={""color"":""red"",""message_format"":""html"",""message"":""<b>CCS.Portal.Trunk</b> build failed."",""notify"":""true""}",
				@"Timeout=00:00:20",
				@"SuccessStatusCodes=[204]"
			};*/

			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return 0;
			}

			Arguments.Default = ArgumentProperties.Parse(args);
			SendRequest();

			return RaiseError.ExitCode;
		}

		/// <summary>
		/// Sends the HTTP request.
		/// </summary>
		private static void SendRequest()
		{
			var request = WebRequest.Create(Arguments.Url);
			request.Method = Arguments.Method;
			foreach (var header in Arguments.Headers)
			{
				if ("Content-Type".Equals(header.Key, StringComparison.InvariantCultureIgnoreCase))
				{
					request.ContentType = header.Value;
					continue;
				}

				request.Headers[header.Key] = header.Value;
			}

			if (!string.IsNullOrEmpty(Arguments.Content))
			{
				var data = Encoding.UTF8.GetBytes(Arguments.Content);
				using (var stream = request.GetRequestStream())
				{
					stream.Write(data, 0, data.Length);
				}
			}

			request.Timeout = (int)Arguments.Timeout.TotalMilliseconds;

			try
			{
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					AnalyzeResponse(response);
				}
			}
			catch (WebException e)
			{
				using (var response = (HttpWebResponse)e.Response)
				{
					AnalyzeResponse(response);
				}
			}
		}

		private static void AnalyzeResponse(HttpWebResponse response)
		{
			string responseContent = null;
			if (response.ContentLength > 0)
			{
				var resultBytes = new byte[response.ContentLength];
				using (var stream = response.GetResponseStream())
				{
					if (stream != null)
					{
						stream.Read(resultBytes, 0, resultBytes.Length);
					}
				}

				responseContent = Encoding.UTF8.GetString(resultBytes);
			}

			if (!Arguments.SuccessStatusCodes.Contains((int)response.StatusCode))
			{
				RaiseError.StatusCodeError((int)response.StatusCode, responseContent);
			}
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
	}
}
