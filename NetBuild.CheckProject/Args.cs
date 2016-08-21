using Atom.Toolbox;
using NetBuild.ConsoleApp;

namespace NetBuild.CheckProject
{
	public class Args : CheckArgs
	{
		public Args(string[] args)
			: base(args)
		{
		}

		[Optional("asm", "Assembly name to debug.")]
		public string DebugAssembly => this.Get<string>("asm", null);
	}
}
