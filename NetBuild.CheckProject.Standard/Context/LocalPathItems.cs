using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetBuild.CheckProject.Standard
{
	/// <summary>
	/// Gets all files and folders from the local path (recursive).
	/// Returned paths are relative to the target, and folders are marked with a trailing backslash.
	/// E.g. "Product.Core.csproj", "App.config", "Properties\" "Properties\AssemblyInfo.cs", etc.
	/// </summary>
	public class LocalPathItems : ValueProvider<List<string>>
	{
		public override List<string> Get(CheckContext context)
		{
			var localPath = context.Of<LocalPath>().Value;

			Console.Write("Getting local file list... ");
			var files = Directory.GetFiles(localPath, "*", SearchOption.AllDirectories)
				.Union(
					Directory.GetDirectories(localPath, "*", SearchOption.AllDirectories)
					.Select(item => item + '\\'))
				.Select(item => item.Replace(localPath + '\\', String.Empty))
				.ToList();

			Console.WriteLine($"{files.Count} files found.");
			return files;
		}
	}
}
