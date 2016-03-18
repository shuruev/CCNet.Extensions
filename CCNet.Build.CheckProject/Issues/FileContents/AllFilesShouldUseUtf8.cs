using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCNet.Build.CheckProject
{
	public class AllFilesShouldUseUtf8 : IChecker
	{
		private static readonly byte[] s_utf8Bom = Encoding.UTF8.GetPreamble();

		public void Check(CheckContext context)
		{
			var encodings = GetTfsEncodings(context);
			var signatures = GetLocalUtf8BomSignatures(encodings.Keys);

			// skip files which are marked as UTF-8 in TFS and using BOM
			var exclude = encodings
				.Where(i => i.Value == 65001)
				.Select(i => i.Key)
				.Where(signatures.ContainsKey)
				.Where(i => signatures[i])
				.ToList();

			foreach (var file in exclude)
			{
				encodings.Remove(file);
				signatures.Remove(file);
			}

			var required = new List<string>();
			var mismatched = new List<string>();
			var exceptional = new List<string>();

			var ansi = GetLocalOnlyAnsiCharacters(encodings);
			foreach (var file in encodings.Keys)
			{
				if (ShouldAlwaysUseUtf8WithBom(file))
				{
					// some files (like *.cs or *.config) should always be saved with BOM and marked as UTF-8 in TFS
					required.Add(file);
				}
				else if (signatures[file])
				{
					// file saved with BOM but not marked as UTF-8 in TFS
					mismatched.Add(file);
				}
				else if (!ansi[file])
				{
					// all other files are forced to be UTF-8 + BOM only if they are using non-ANSI characters
					exceptional.Add(file);
				}
			}

			if (required.Count == 0 && exceptional.Count == 0)
				return;

			var sb = new StringBuilder();

			Func<string, string> format = file =>
			{
				return String.Format(
					"- {0} ({1} BOM, marked as {2})",
					file,
					signatures[file] ? "uses" : "no",
					encodings[file]);
			};

			if (required.Count > 0)
			{
				sb.AppendLine("The following files should always be saved with BOM and marked as UTF-8 (65001) in TFS:");
				foreach (var file in required)
				{
					sb.AppendLine(format(file));
				}
			}

			if (mismatched.Count > 0)
			{
				if (sb.Length > 0)
					sb.AppendLine("                               ");

				sb.AppendLine("The following files are saved with BOM, so they should rather be marked as UTF-8 (65001) in TFS:");
				foreach (var file in mismatched)
				{
					sb.AppendLine(format(file));
				}
			}

			if (exceptional.Count > 0)
			{
				if (sb.Length > 0)
					sb.AppendLine("                               ");

				sb.AppendLine("The following files use non-ANSI characters, so they should rather be saved with BOM and marked as UTF-8 (65001) in TFS:");
				foreach (var file in exceptional)
				{
					sb.AppendLine(format(file));
				}
			}

			throw new FailedCheckException(
				@"{0}
                               
Please update TFS encodings using 'Advanced/Properties...' dialog, and/or save required files using 'Advanced Save Options...' command in Visual Studio.
This should help other team memebers to avoid possible conflicts while working with such files in Visual Studio and other tools.",
				sb.ToString());
		}

		private bool ShouldAlwaysUseUtf8WithBom(string textFile)
		{
			var name = Path.GetFileName(textFile).ToLowerInvariant();
			if (name.EndsWith(".cs")
				|| name.EndsWith(".asax")
				|| name.EndsWith(".ascx")
				|| name.EndsWith(".aspx")
				|| name.EndsWith(".config")
				|| name.EndsWith(".config.default")
				|| name.EndsWith(".csproj")
				|| name.EndsWith(".csproj.vspscc"))
				return true;

			return false;
		}

		private Dictionary<string, int> GetTfsEncodings(CheckContext context)
		{
			Console.WriteLine("Getting file encodings from TFS...");
			var files = context.Tfs.GetAllFileEncodings(Args.TfsPath)
				.Where(i => i.Value > 0)
				.ToDictionary(
					item => item.Key.Replace(Args.TfsPath + '/', String.Empty),
					item => item.Value);

			Console.WriteLine("Found {0} text files.", files.Count);
			return files;
		}

		private Dictionary<string, bool> GetLocalUtf8BomSignatures(IEnumerable<string> textFiles)
		{
			var bag = new ConcurrentDictionary<string, bool>();

			Console.WriteLine("Checking if local files are using UTF-8 signatures...");
			Parallel.ForEach(
				textFiles,
				file =>
				{
					var filePath = Path.Combine(Args.ProjectPath, file);
					bag[file] = IsUsingUtf8BomSignature(filePath);

				});

			Console.WriteLine("Checked {0} local files, {1} are using UTF-8 signatures.", bag.Count, bag.Count(i => i.Value));
			return bag.ToDictionary(i => i.Key, i => i.Value);
		}

		private Dictionary<string, bool> GetLocalOnlyAnsiCharacters(Dictionary<string, int> tfsEncodings)
		{
			var bag = new ConcurrentDictionary<string, bool>();

			Console.WriteLine("Checking if local files are using non-ANSI characters...");
			Parallel.ForEach(
				tfsEncodings,
				item =>
				{
					var filePath = Path.Combine(Args.ProjectPath, item.Key);
					bag[item.Key] = IsUsingOnlyAnsiCharacters(filePath, item.Value);

				});

			Console.WriteLine("Checked {0} local files, {1} are using non-ANSI characters.", bag.Count, bag.Count(i => !i.Value));
			return bag.ToDictionary(i => i.Key, i => i.Value);
		}

		private bool IsUsingUtf8BomSignature(string filePath)
		{
			using (var fs = File.Open(filePath, FileMode.Open))
			{
				var bom = new byte[3];
				fs.Read(bom, 0, 3);

				for (int i = 0; i < s_utf8Bom.Length; i++)
				{
					if (s_utf8Bom[i] != bom[i])
						return false;
				}

				return true;
			}
		}

		private bool IsUsingOnlyAnsiCharacters(string filePath, int tfsEncoding)
		{
			var utf8 = File.ReadAllText(filePath, Encoding.UTF8);
			var cp1252 = File.ReadAllText(filePath, Encoding.GetEncoding(1252));
			var tfs = File.ReadAllText(filePath, Encoding.GetEncoding(tfsEncoding));
			if (utf8 == cp1252 && utf8 == tfs)
				return true;

			return false;
		}
	}
}
