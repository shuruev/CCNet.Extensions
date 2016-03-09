using System;

namespace CCNet.Build.Tfs
{
	/// <summary>
	/// Changeset summary DTO.
	/// </summary>
	public class ChangesetSummary
	{
		/// <summary>
		/// Gets or sets changeset ID.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets user name who commited this changeset.
		/// </summary>
		public string User { get; set; }

		/// <summary>
		/// Gets or sets display name for a user who commited this changeset.
		/// </summary>
		public string UserDisplay { get; set; }

		/// <summary>
		/// Gets or sets date and time when this changeset was commited.
		/// </summary>
		public DateTime Date { get; set; }
	}
}
