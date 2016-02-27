using System;
using System.Text;

namespace CCNet.Build.Confluence
{
	/// <summary>
	/// Confluence page summary DTO.
	/// </summary>
	public class PageSummary
	{
		/// <summary>
		/// Gets or sets page ID.
		/// </summary>
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets parent page ID.
		/// </summary>
		public long ParentId { get; set; }

		/// <summary>
		/// Gets or sets page name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return new StringBuilder()
				.Append("\"")
				.Append(Name)
				.Append("\", #")
				.Append(Id)
				.ToString();
		}
	}
}
