using System;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CCNet.Build.AzureUpload
{
	internal static class CloudStorageExtensions
	{
		public static void UploadFile(this CloudBlobContainer container, string file, string blobName)
		{
			var blob = container.GetBlockBlobReference(blobName);
			var extension = Path.GetExtension(file).ToLowerInvariant();

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

			Console.WriteLine("Local name: {0}", file);
			Console.WriteLine("Blob name: {0}", blob.Name);

			Console.Write("Uploading file... ");
			blob.UploadFromFile(file, FileMode.Open);
			Console.WriteLine("OK");
		}
	}
}
