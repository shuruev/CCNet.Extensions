using System;
using System.IO;

namespace NetBuild.CheckProject.SourceControl
{
	/// <summary>
	/// Provides solution file structure based on a source control remote path.
	/// </summary>
	public class RemoteSolution : IContextProvider
	{
		/// <summary>
		/// Gets solution path, e.g. "$/Main/Product".
		/// </summary>
		public string SolutionPath { get; private set; }

		/// <summary>
		/// Gets solution name, e.g. "Product.sln".
		/// </summary>
		public string SolutionName => $"{Path.GetFileName(SolutionPath)}.sln";

		/// <summary>
		/// Loads required data.
		/// </summary>
		public void Load(CheckContext context)
		{
			var remotePath = context.Of<RemotePath>().Value;
			SolutionPath = Path.GetDirectoryName(remotePath).Replace('\\', '/');

			if (context.Args.DebugMode)
				Console.WriteLine($"{nameof(SolutionPath)} = {SolutionPath}");
		}
	}
}
