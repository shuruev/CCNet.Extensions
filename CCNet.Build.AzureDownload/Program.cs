using System;
using System.IO;
using CCNet.Build.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace CCNet.Build.AzureDownload
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Execute.DisplayUsage("Downloads files from Azure blob storage.", typeof(Args));
				return 0;
			}

			try
			{
				Args.Current = new ArgumentProperties(args);
				Execute.DisplayCurrent(typeof(Args));

				AzureDownload();
			}
			catch (Exception e)
			{
				return Execute.RuntimeError(e);
			}

			return 0;
		}

		private static void AzureDownload()
		{
			var accountName = Config.AccountName(Args.Storage);
			var accountKey = Config.AccountKey(Args.Storage);

			var credentials = new StorageCredentials(accountName, accountKey);
			var account = new CloudStorageAccount(credentials, false);
			var client = account.CreateCloudBlobClient();
			var container = client.GetContainerReference(Args.Container);

			var blob = container.GetBlockBlobReference(Args.BlobFile);
			var localFile = Args.LocalFile;

			Console.WriteLine("Blob name: {0}", blob.Name);
			Console.WriteLine("Local name: {0}", localFile);

			Console.Write("Downloading file... ");
			blob.DownloadToFile(localFile, FileMode.Create);
			Console.WriteLine("OK");
		}
	}
}
