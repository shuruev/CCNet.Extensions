namespace CCNet.Build.CheckProject
{
	public class CheckProjectAssemblyName : IChecker
	{
		public void Check(CheckContext context)
		{
			var properties = context.ProjectCommonProperties.Result;
			properties.CheckRequired("AssemblyName", Args.AssemblyName);
		}
	}
}
