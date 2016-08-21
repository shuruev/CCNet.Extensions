using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atom.Toolbox;

namespace NetBuild.CheckProject.Standard
{
	public class Item1 : IContextProvider
	{
		public string Data { get; set; }

		public void Load(CheckContext context, CheckArgs args)
		{
			Data = args.Get<string>("myarg");
		}
	}
}
