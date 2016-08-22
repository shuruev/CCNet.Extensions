using System;

namespace NetBuild.CheckProject.SourceControl
{
	/// <summary>
	/// Gets an instance of a source control provider.
	/// </summary>
	public class SourceControlProvider : ValueProvider<ISourceControl>
	{
		public override ISourceControl Get(CheckContext context)
		{
			// should be overridden as specific provider (e.g. TFS, Git, etc.)
			throw new NotImplementedException();
		}
	}
}
