namespace CCNet.Build.Reconfigure
{
	public interface IProjectSnapshot
	{
		string Name { get; }
		string WorkingDirectoryTemp { get; }
	}
}
