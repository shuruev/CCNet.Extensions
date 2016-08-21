using Atom.Toolbox;
using NetBuild.ConsoleApp;

namespace NetBuild.CheckProject
{
	public class Args : ConsoleArgs
	{
		public Args(string[] args)
			: base(args, "issues")
		{
		}

		[Required("issues", "A list of issues to check separated by '|'.")]
		public string[] CheckIssues => this.Get<string>("issues").Split('|');

		[Optional("asm", "Assembly name to debug.")]
		public string DebugAssembly => this.Get<string>("asm", null);
	}
}
