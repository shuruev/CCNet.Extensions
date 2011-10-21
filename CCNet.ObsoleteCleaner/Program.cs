using System;
using CCNet.Common;
using CCNet.ObsoleteCleaner.Properties;

namespace CCNet.ObsoleteCleaner
{
	public class Program
	{
		/// <summary>
		/// Main program.
		/// </summary>
		public static int Main(string[] args)
		{
			args = new[]
			{
				@"InternalReferencesPath=D:\InternalReferences",
				@"PublicationStoragePath=D:\PublicationStorage",
				@"DaysToLive=30",
			};

			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return 0;
			}

			try
			{
				Arguments.Default = ArgumentProperties.Parse(args);

				var internalReferences = new CleaningFolder(
					Arguments.InternalReferencesPath,
					new InternalReferencesObsoleteDetector());
				internalReferences.Clean();

				var publicationStorage = new CleaningFolder(
					Arguments.PublicationStoragePath,
					new PublicationStorageObsoleteDetector());
				publicationStorage.Clean();
			}
			catch (Exception e)
			{
				return ErrorHandler.Runtime(e);
			}

			return 0;
		}

		/// <summary>
		/// Displays usage text.
		/// </summary>
		private static void DisplayUsage()
		{
			Console.WriteLine();
			Console.WriteLine(Resources.UsageInfo);
			Console.WriteLine();
		}
	}
}
