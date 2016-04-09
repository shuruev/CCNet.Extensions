namespace CCNet.Build.Reconfigure
{
	public interface IProjectSnapshot
	{
		string Name { get; }
		string UniqueName { get; }
		string WorkingDirectoryTemp { get; }
	}
}
