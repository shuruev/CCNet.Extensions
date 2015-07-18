using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CCNet.Common.Tests
{
	/// <summary>
	/// Tests ArgumentProperties class.
	/// </summary>
	[TestClass]
	public class ArgumentPropertiesTest
	{
		[TestMethod]
		public void Parsing_Of_TimeSpan_Is_Ok()
		{
			var args = ArgumentProperties.Parse(string.Format("Time=2.10:12:14"));
			var actualTime = args.GetTimeSpanValue("Time");

			Assert.AreEqual(TimeSpan.Parse("2.10:12:14"), actualTime);
		}

		[TestMethod]
		public void Parsing_Of_Value_With_Equals_Sign_Is_Ok()
		{
			var args = ArgumentProperties.Parse("Test=2+2=4");
			var actualValue = args.GetValue("Test");

			Assert.AreEqual("2+2=4", actualValue);
		}

		[TestMethod]
		public void Parsing_Of_Json_Value_Is_Ok()
		{
			int[] expectedValue = { 10, 12 };

			var args = ArgumentProperties.Parse("Test=[10,12]");
			var actualValue = args.GetObjectFromJson<int[]>("Test");

			Assert.IsNotNull(actualValue);
			Assert.AreEqual(2, actualValue.Length);
			Assert.AreEqual(expectedValue[0], actualValue[0]);
			Assert.AreEqual(expectedValue[1], actualValue[1]);
		}
	}
}
