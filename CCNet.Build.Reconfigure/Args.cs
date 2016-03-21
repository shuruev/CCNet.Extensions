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

		public static string ConfluenceCache
		{
			get { return Current.Get<string>("ConfluenceCache"); }
		}

		public static string ProjectMap
		{
			get { return Current.Get<string>("ProjectMap"); }
		}
	}
}
