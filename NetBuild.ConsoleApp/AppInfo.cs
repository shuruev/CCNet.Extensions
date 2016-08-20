using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NetBuild.ConsoleApp
{
	public class AppInfo
	{
		public string Executable { get; }
		public Version Version { get; }

		public string Title { get; }
		public string Description { get; }
		public string Copyright { get; }

		public AppInfo()
		{
			var assembly = Assembly.GetEntryAssembly();

			Executable = Path.GetFileName(assembly.Location);
			Version = assembly.GetName().Version;

			Title = ExtractFromAttribute<AssemblyTitleAttribute>(assembly, attr => attr.Title);
			Description = ExtractFromAttribute<AssemblyDescriptionAttribute>(assembly, attr => attr.Description);
			Copyright = ExtractFromAttribute<AssemblyCopyrightAttribute>(assembly, attr => attr.Copyright);
		}

		private string ExtractFromAttribute<T>(Assembly assembly, Func<T, string> extract) where T : Attribute
		{
			string result = null;

			var attr = (T)assembly.GetCustomAttributes(typeof(T)).FirstOrDefault();
			if (attr != null)
				result = extract.Invoke(attr);

			if (String.IsNullOrWhiteSpace(result))
				throw new InvalidOperationException($"{typeof(T).Name} should be specified for the entry assembly.");

			return result;
		}
	}
}
