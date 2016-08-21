using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBuild.CheckProject.Legacy
{
	public class Checker1 : ICheckIssue
	{
		public string Issue => "X01";

		public void Check(CheckContext context, CheckArgs args)
		{
			Console.WriteLine(context.Of<Item2>().Data);
			Console.WriteLine(context.Of<Item1>().Data);
			Console.WriteLine(context.Of<Item2>().Data);
		}
	}
}
