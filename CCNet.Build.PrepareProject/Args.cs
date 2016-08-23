using System;
using Atom.Toolbox;
using CCNet.Build.Common;
using NetBuild.ConsoleApp;

namespace CCNet.Build.PrepareProject
{
	public class Args : ConsoleArgs
	{
		public Args(string[] args)
			: base(args, "path", "version")
		{
		}

		[Required("path", "Target project folder.")]
		public string ProjectPath => this.Get<string>("path");

		[Required("version", "Current version.")]
		public Version CurrentVersion => new Version(this.Get<string>("version")).Normalize();

		[Required("output", "Specifies the output folder.")]
		public string OutputPath => this.Get<string>("output");

		[Required("tfs", "Specifies the TFS path.")]
		public string TfsPath => this.Get<string>("tfs");

		[Optional("updateAssemblyInfo", "Update assembly information file.")]
		public bool UpdateAssemblyInfo => this.Get("updateAssemblyInfo", false);
	}
}
