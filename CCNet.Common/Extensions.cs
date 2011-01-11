using System;
using System.Globalization;
using System.Text;

namespace CCNet.Common
{
	/// <summary>
	/// Contains extension methods.
	/// </summary>
	public static class Extensions
	{
		#region String extensions

		/// <summary>
		/// Applies formatting for invariant culture.
		/// </summary>
		public static string Display(this string format, params object[] args)
		{
			return String.Format(CultureInfo.InvariantCulture, format, args);
		}

		/// <summary>
		/// Prepares text string to HTML output.
		/// </summary>
		public static string ToHtml(this string text)
		{
			if (String.IsNullOrEmpty(text))
				return String.Empty;

			StringBuilder sb = new StringBuilder();
			foreach (char t in text)
			{
				switch (t)
				{
					case '&':
						sb.Append("&amp;");
						break;
					case '<':
						sb.Append("&lt;");
						break;
					case '>':
						sb.Append("&gt;");
						break;
					case '"':
						sb.Append("&quot;");
						break;
					default:
						sb.Append(t);
						break;
				}
			}

			text = sb.ToString();

			if (text.StartsWith(" ", StringComparison.Ordinal))
			{
				text = text.TrimStart(' ');
				text = text.Insert(0, "&nbsp;");
			}

			if (text.EndsWith(" ", StringComparison.Ordinal))
			{
				text = text.TrimEnd(' ');
				text = text.Insert(text.Length, "&nbsp;");
			}

			text = text.Replace("  ", "&nbsp;&nbsp;");

			text = text.Replace("\r\n", "\n");
			text = text.Replace("\n", "<br />");
			text = text.Replace("\v", "<br />");

			return text;
		}

		#endregion

		#region DateTime extensions

		/// <summary>
		/// Get universal and sortable date/time string.
		/// </summary>
		public static string ToUniversalString(this DateTime dateTime)
		{
			return dateTime.ToString("yyyy-MM-dd HH.mm.ss", CultureInfo.InvariantCulture);
		}

		#endregion
	}
}
