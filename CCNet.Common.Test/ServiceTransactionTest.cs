using System.IO;
using System.Reflection;
using CCNet.Common.Helpers;
using CCNet.Common.Test.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCNet.Common.Test
{
	/// <summary>
	/// This is a test class for ServiceTransactionTest and is intended
	/// to contain all ServiceTransactionTest Unit Tests.
	/// </summary>
	[TestClass]
	public class ServiceTransactionTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		/// <summary>
		/// Tests Begin method.
		/// </summary>
		[TestMethod]
		public void BeginTest()
		{
			ServiceItem[] services = new[]
			{
				new ServiceItem
				{
					ServiceName = "Test",
					DisplayName = "Test Service",
					BinaryPathName = @"G:\test.exe",
					TargetFrameWork = TargetFramework.Net35
				}
			};

			ServiceTransaction.Begin(services);

			var serviceTransactionFileName = ServiceTransaction.FilePathName;

			string actual = File.ReadAllText(serviceTransactionFileName);

			File.Delete(serviceTransactionFileName);

			string expected = Resources.BeginInstalledServicesXmlFile;

			Assert.AreEqual(
				expected,
				actual);
		}

		/// <summary>
		/// Tests Commit method.
		/// </summary>
		[TestMethod]
		public void CommitTest()
		{
			const string binaryPathName = @"G:\test.exe";

			var serviceTransactionFileName = ServiceTransaction.FilePathName;

			File.WriteAllText(
				serviceTransactionFileName,
				Resources.BeginInstalledServicesXmlFile);

			ServiceTransaction.Commit(binaryPathName);

			string actual = File.ReadAllText(serviceTransactionFileName);

			File.Delete(serviceTransactionFileName);

			string expected = Resources.CommitInstalledServicesXmlFile;

			Assert.AreEqual(
				expected,
				actual);
		}

		/// <summary>
		/// Tests GetUncommited method.
		/// </summary>
		[TestMethod]
		public void GetUncommitedTest()
		{
			var serviceTransactionFileName = ServiceTransaction.FilePathName;

			File.WriteAllText(
				serviceTransactionFileName,
				Resources.BeginInstalledServicesXmlFile);

			var service = ServiceTransaction.GetUncommited();

			File.Delete(serviceTransactionFileName);

			Assert.AreEqual(
				service.Length,
				1);

			Assert.AreEqual(
				"Test",
				service[0].ServiceName);

			Assert.AreEqual(
				"Test Service",
				service[0].DisplayName);

			Assert.AreEqual(
				@"G:\test.exe",
				service[0].BinaryPathName);

			Assert.AreEqual(
				TargetFramework.Net35,
				service[0].TargetFrameWork);
		}
	}
}
