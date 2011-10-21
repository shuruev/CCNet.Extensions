using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCNet.ObsoleteCleaner.Tests
{
	/// <summary>
	/// Tests for methods of ObsoleteHelper class.
	/// </summary>
	[TestClass]
	public class ObsoleteHelperTest
	{
		[TestMethod]
		public void Converts_Version_String_To_DateTime_Success()
		{
			DateTime? expected = new DateTime(2010, 12, 9);

			DateTime? actual = ObsoleteHelper.ConvertVersionToDate("10.12.9.7");

			Assert.IsNotNull(actual);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Converts_Version_String_To_DateTime_Failed()
		{
			DateTime? actual = ObsoleteHelper.ConvertVersionToDate("0.0.0.0");

			Assert.IsNull(actual);
		}

		[TestMethod]
		public void Converts_Version_String_To_DateTime_Failed_2()
		{
			DateTime? actual = ObsoleteHelper.ConvertVersionToDate("not-a-version");

			Assert.IsNull(actual);
		}

		[TestMethod]
		public void Is_Obsolete_Returns_True()
		{
			var date = DateTime.Now - TimeSpan.FromDays(2);
			const int daysToLive = 1;

			var actual = ObsoleteHelper.IsObsolete(date, daysToLive);

			Assert.IsTrue(actual);
		}

		[TestMethod]
		public void Is_Obsolete_Returns_False()
		{
			var date = DateTime.Now;
			const int daysToLive = 1;

			var actual = ObsoleteHelper.IsObsolete(date, daysToLive);

			Assert.IsFalse(actual);
		}

		[TestMethod]
		public void Getting_Obsolete_Paths()
		{
			// prepare
			var now = DateTime.Now;
			var todaysVersion = GetFirstVersionByDay(now);
			var lastWeekVersion = GetFirstVersionByDay(now.AddDays(-7));
			var lastYearVersion = GetFirstVersionByDay(now.AddYears(-1));
			const string someVersion = "SomeFolder";

			const string projectFolder = @"d:\test";

			string todaysPath = Path.Combine(projectFolder, todaysVersion);
			string lastWeekPath = Path.Combine(projectFolder, lastWeekVersion);
			string lastYearPath = Path.Combine(projectFolder, lastYearVersion);
			string somePath = Path.Combine(projectFolder, someVersion);

			var sourcePaths = new[]
			{
				todaysPath,
				lastWeekPath,
				lastYearPath,
				somePath,
			};

			var excludeVersions = new List<string> { lastYearVersion, projectFolder };
			const int daysToLive = 4;

			// test
			var result = ObsoleteHelper.GetObsoletePaths(
				sourcePaths,
				excludeVersions,
				daysToLive);

			// check
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(lastWeekPath, result[0]);
		}

		/// <summary>
		/// Gets first version number at specified <paramref name="day"/>.
		/// </summary>
		private static string GetFirstVersionByDay(DateTime day)
		{
			return string.Format(
				"{0}.{1}.{2}.1",
				day.Year.ToString().Substring(2, 2),
				day.Month,
				day.Day);
		}
	}
}
