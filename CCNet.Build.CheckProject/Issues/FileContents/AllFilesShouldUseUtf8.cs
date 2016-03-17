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
			var textFiles = CheckTfs(context);
			CheckLocal(textFiles);
		}

		private List<string> CheckTfs(CheckContext context)
		{
			Console.WriteLine("Getting file encodings from TFS...");
			var files = context.Tfs.GetAllFileEncodings(Args.TfsPath)
				.Where(i => i.Value > 0)
				.ToDictionary(
					item => item.Key.Replace(Args.TfsPath + '/', String.Empty),
					item => item.Value);

			Console.WriteLine("Found {0} text files.", files.Count);

			var nonUtf8 = files.Where(i => i.Value != 65001).ToList();
			if (nonUtf8.Count == 0)
				return files.Keys.ToList();

			throw new FailedCheckException(
				@"There are encoding issues with the following source control files:
{0}
                               
Please make sure all files under source control are using UTF-8 (65001) encoding. Other encoding could happen if files were created outside Visual Studio.
Update source control encoding using Properties menu, and change file contents if needed (in case it uses non-ANSI characters).",
				String.Join(Environment.NewLine, nonUtf8.Select(i => String.Format("- {0} ({1})", i.Key, i.Value))));
		}

		private void CheckLocal(IEnumerable<string> textFiles)
		{
			var bag = new ConcurrentBag<string>();

			Console.WriteLine("Checking local file encodings...");
			Parallel.ForEach(
				textFiles,
				file =>
				{
					var local = Path.Combine(Args.ProjectPath, file);
					if (!CheckLocalFile(local))
						bag.Add(file);
				});

			throw new FailedCheckException(
				@"The following files are marked as UTF-8 in source control, but seems using non-ANSI characters:
{0}
                               
Please double check file contents and properly re-save them using UTF-8 (65001) encoding with BOM signature. This will help avoiding issues for others working with these files in Visual Studio.",
				String.Join(Environment.NewLine, bag.Select(i => String.Format("- {0}", i))));
		}

		private bool CheckLocalFile(string filePath)
		{
			if (IsUsingUtf8Bom(filePath))
				return true;

			var utf8 = File.ReadAllText(filePath, Encoding.UTF8);
			var cp1252 = File.ReadAllText(filePath, Encoding.GetEncoding(1252));
			if (utf8 == cp1252)
				return true;

			return false;
		}

		private bool IsUsingUtf8Bom(string filePath)
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
	}
}
