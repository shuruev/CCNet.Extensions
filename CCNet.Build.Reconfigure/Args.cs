using CCNet.Build.Common;
using Lean.Configuration;

namespace CCNet.Build.Reconfigure
{
	public static class Args
	{
		public static ArgumentProperties Current { get; set; }

		public static string OutputDirectory
		{
			get { return Current.Get<string>("OutputDirectory"); }
		}
	}
}
