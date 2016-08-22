namespace NetBuild.CheckProject.SourceControl.Tfs
{
	/// <summary>
	/// Gets an instance of a source control provider.
	/// </summary>
	public class TfsSourceControlProvider : SourceControlProvider
	{
		public override ISourceControl Get(CheckContext context)
		{
			var tfsUrl = context.Of<TfsUrl>().Value;
			return new TfsSourceControl(tfsUrl);
		}
	}
}
