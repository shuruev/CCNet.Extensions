namespace CCNet.Build.CheckProject
{
	public class FabricProjectFileShouldExist : ProjectFileShouldExist
	{
		public override void Check(CheckContext context)
		{
			Check(context.LocalFiles.Result, ".sfproj");
		}
	}
}
