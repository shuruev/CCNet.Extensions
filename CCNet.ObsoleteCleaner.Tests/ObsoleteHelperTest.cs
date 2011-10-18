using System;
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
	}
}
