using System;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace NetBuild.Tfs
{
	/// <summary>
	/// Item header DTO.
	/// </summary>
	public class ItemHeader
	{
		/// <summary>
		/// Gets or sets server path.
		/// </summary>
		public string ServerPath { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether specified item is a folder.
		/// </summary>
		public bool IsFolder { get; set; }

		/// <summary>
		/// Gets or sets item encoding.
		/// </summary>
		public int Encoding { get; set; }

		/// <summary>
		/// Gets or sets date and time when this item was checked-in.
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ItemHeader()
		{
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ItemHeader(Item item)
		{
			ServerPath = item.ServerItem;
			IsFolder = item.ItemType == ItemType.Folder;
			Encoding = item.Encoding;
			Date = item.CheckinDate.ToUniversalTime();
		}
	}
}
