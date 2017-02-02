using System;
using System.IO;
using System.Linq;
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

			var local = Args.LocalFile;
			var remote = Args.BlobFile
				.Replace("[datetime]", DateTime.UtcNow.ToString("yyyyMMdd-HHmm"));

			var attr = File.GetAttributes(local);
			var isFolder = ((attr & FileAttributes.Directory) == FileAttributes.Directory);

			if (!isFolder)
			{
				container.UploadFile(
					file: local,
					blobName: remote);
			}
			else
			{
				var files = Directory.GetFiles(local, "*", SearchOption.AllDirectories);
				var filesToBlobs = files.Select(
					file =>
					{
						var bloblFileName = file
							.Replace(local, string.Empty)
							.Replace("\\", "/");

						var blobFilePath = remote + bloblFileName;
						return new Tuple<string, string>(file, blobFilePath);
					});

				foreach (var filesToBlob in filesToBlobs)
				{
					container.UploadFile(
						file: filesToBlob.Item1,
						blobName: filesToBlob.Item2);
				}
			}
		}
	}
}
