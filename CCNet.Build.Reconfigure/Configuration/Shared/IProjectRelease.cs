namespace CCNet.Build.Reconfigure
{
	public interface IProjectRelease
	{
		string Name { get; }
		string UniqueName { get; }
		string WorkingDirectory { get; }
	}
}
