using System;
using System.Collections.Generic;

namespace NetBuild.CheckProject.SourceControl
{
	/// <summary>
	/// Gets all files and folders from the remote solution path (not recursive).
	/// Returned paths are relative to the target, and folders are marked with a trailing slash.
	/// E.g. "packages/", ".nuget/", "Product.sln", etc.
	/// </summary>
	public class SolutionPathItems : ValueProvider<List<string>>
	{
		public override List<string> Get(CheckContext context)
		{
			var remoteSolution = context.Of<RemoteSolution>();
			var sourceControl = context.Of<SourceControlProvider>().Value;

			Console.Write("Getting source control items from the solution folder... ");
			var items = sourceControl.GetChildItems(remoteSolution.SolutionPath);

			Console.WriteLine($"{items.Count} found.");
			return items;
		}
	}
}
