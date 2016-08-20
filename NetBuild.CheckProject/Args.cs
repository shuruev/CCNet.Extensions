using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Toolbox;
using NetBuild.ConsoleApp;

namespace NetBuild.CheckProject
{
	public class Args : ConsoleArgs
	{
		public Args(string[] args)
			: base(args, "path", "check")
		{
		}

		[Required("path", "Target project path to work with.")]
		public string ProjectPath => this.Get<string>("path");

		[Required("check", "A list of issues to check separated by '|'.")]
		public string[] CheckIssues => this.Get<string>("check").Split('|');

		[Optional("project", "Specifies project name.")]
		public string[] ProjectName => this.Get<string>("project").Split('|');

		public bool Hey { get; set; }
	}
}
