using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBuild.CheckProject.Standard
{
	public class Test1 : ICheckIssue
	{
		public string Issue => "T01";

		public void Check(CheckContext context)
		{
			Console.WriteLine("The project file name should be {0}", context.Value<ProjectFile>());
		}
	}
}
