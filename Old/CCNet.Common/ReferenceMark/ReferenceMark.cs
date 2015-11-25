using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace CCNet.Common
{
	/// <summary>
	/// Common methods for working with reference marks.
	/// </summary>
	public static class ReferenceMark
	{
		#region Service methods

		/// <summary>
		/// Gets appropriate path for reference mark files.
		/// </summary>
		private static string GetEndPath(ReferenceType referenceType, string referencesDirectory)
		{
			return Path.Combine(referencesDirectory, referenceType.ToString());
		}

		/// <summary>
		/// Recreates appropriate folder for reference mark files.
		/// </summary>
		private static void RecreateEndPath(ReferenceType referenceType, string referencesDirectory)
		{
			string path = GetEndPath(referenceType, referencesDirectory);
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
				Thread.Sleep(500);
			}

			Directory.CreateDirectory(path);
			Thread.Sleep(500);

			MarkUpdatedFolder(referencesDirectory);
		}

		#endregion

		#region Setting marks

		/// <summary>
		/// Gets name for file that marks reference file.
		/// </summary>
		public static string GetReferenceMarkName(string referenceName)
		{
			return "{0}.txt".Display(referenceName);
		}

		/// <summary>
		/// Gets name for file that marks reference folder.
		/// </summary>
		public static string GetFolderMarkName()
		{
			return "Update.txt";
		}

		/// <summary>
		/// Marks specified file as updated.
		/// </summary>
		public static void MarkUpdatedFile(string fileName)
		{
			File.WriteAllText(fileName, DateTime.Now.ToUniversalString());
		}

		/// <summary>
		/// Marks reference file as updated.
		/// </summary>
		public static void MarkUpdatedReference(ReferenceType referenceType, string referencesDirectory, string referenceName)
		{
			string fileName = GetReferenceMarkName(referenceName);
			string path = GetEndPath(referenceType, referencesDirectory);
			MarkUpdatedFile(Path.Combine(path, fileName));
		}

		/// <summary>
		/// Marks reference folder as updated.
		/// </summary>
		private static void MarkUpdatedFolder(string referencesDirectory)
		{
			string fileName = GetFolderMarkName();
			MarkUpdatedFile(Path.Combine(referencesDirectory, fileName));
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Gets current reference marks of specified type.
		/// </summary>
		public static List<string> GetCurrent(ReferenceType referenceType, string referencesDirectory)
		{
			List<string> references = new List<string>();

			string path = GetEndPath(referenceType, referencesDirectory);
			if (!Directory.Exists(path))
				return references;

			references.AddRange(
				Directory.GetFiles(path, "*.txt")
				.Select(Path.GetFileNameWithoutExtension));

			return references;
		}

		/// <summary>
		/// Setups actual project references of specified type.
		/// </summary>
		public static void SetupActual(ReferenceType referenceType, string referencesDirectory, IEnumerable<string> actualReferences)
		{
			List<string> currentReferences = GetCurrent(referenceType, referencesDirectory);
			if (actualReferences.OrderBy(i => i)
				.SequenceEqual(currentReferences.OrderBy(i => i)))
				return;

			RecreateEndPath(referenceType, referencesDirectory);

			foreach (string actualReference in actualReferences)
			{
				MarkUpdatedReference(referenceType, referencesDirectory, actualReference);
			}
		}

		#endregion
	}
}
