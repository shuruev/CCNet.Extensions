using System.Text;

namespace CCNet.Build.Confluence
{
	/// <summary>
	/// Confluence page DTO.
	/// </summary>
	public class Page : PageSummary
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public Page()
		{
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public Page(PageSummary summary)
		{
			Id = summary.Id;
			ParentId = summary.ParentId;
			Name = summary.Name;
			Space = summary.Space;
			Version = summary.Version;
		}

		/// <summary>
		/// Gets or sets page content.
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			var sb = new StringBuilder()
				.Append("\"")
				.Append(Name)
				.Append("\", ");

			if (Content == null)
			{
				sb.Append("no data");
			}
			else
			{
				sb.Append(Content.Length);
				sb.Append(" characters");
			}

			return sb.ToString();
		}
	}
}
