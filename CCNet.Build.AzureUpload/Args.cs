using CCNet.Build.Common;
using Lean.Configuration;

namespace CCNet.Build.AzureUpload
{
	public static class Args
	{
		public static ArgumentProperties Current { get; set; }

		public static string AccountName
		{
			get { return Current.Get<string>("AccountName"); }
		}

		public static string AccountKey
		{
			get { return Current.Get<string>("AccountKey"); }
		}

		public static string Container
		{
			get { return Current.Get<string>("Container"); }
		}

		public static string LocalFile
		{
			get { return Current.Get<string>("LocalFile"); }
		}

		public static string BlobFile
		{
			get { return Current.Get<string>("BlobFile"); }
		}
	}
}
