using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBuild.CheckProject.Legacy
{
	public class Item2 : IContextProvider
	{
		public string Data { get; set; }

		public void Load(CheckContext context, CheckArgs args)
		{
			Data = "Hey " + context.Of<Item1>().Data + " " + context.Of<Item1>().Data;
		}
	}
}
