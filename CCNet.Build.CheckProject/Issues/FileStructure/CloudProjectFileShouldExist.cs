namespace CCNet.Build.CheckProject
{
	public class CloudProjectFileShouldExist : ProjectFileShouldExist
	{
		public override void Check(CheckContext context)
		{
			Check(context.LocalFiles.Result, ".ccproj");
		}
	}
}
