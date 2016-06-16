using System;
using System.IO;
using CCNet.Build.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace CCNet.Build.AzureUpload
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Execute.DisplayUsage("Uploads local files to Azure blob storage.", typeof(Args));
				return 0;
			}

			try
			{
				Args.Current = new ArgumentProperties(args);
				Execute.DisplayCurrent(typeof(Args));

				AzureUpload();
			}
			catch (Exception e)
			{
				return Execute.RuntimeError(e);
			}

			return 0;
		}

		private static void AzureUpload()
		{
			var accountName = Config.AccountName(Args.Storage);
			var accountKey = Config.AccountKey(Args.Storage);

			var credentials = new StorageCredentials(accountName, accountKey);
			var account = new CloudStorageAccount(credentials, false);
			var client = account.CreateCloudBlobClient();
			var container = client.GetContainerReference(Args.Container);

			var localFile = Args.LocalFile;
			var extension = Path.GetExtension(localFile).ToLowerInvariant();
			var name = Args.BlobFile
				.Replace("[datetime]", DateTime.UtcNow.ToString("yyyyMMdd-HHmm"));

			var blob = container.GetBlockBlobReference(name);

			switch (extension)
			{
				case ".txt":
					blob.Properties.ContentType = "text/plain";
					break;

				case ".xml":
				case ".cscfg":
					blob.Properties.ContentType = "text/xml";
					break;

				case ".zip":
					blob.Properties.ContentType = "application/zip";
					break;
			}

			Console.WriteLine("Local name: {0}", localFile);
			Console.WriteLine("Blob name: {0}", blob.Name);

			Console.Write("Uploading file... ");
			blob.UploadFromFile(localFile, FileMode.Open);
			Console.WriteLine("OK");
		}
	}
}
