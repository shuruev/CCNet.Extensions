using System;
using System.Collections.Generic;
using System.IO;
using CCNet.Common.Helpers;
using CCNet.Common.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCNet.Common.Tests
{
	/// <summary>
	/// Tests ServiceTransaction class.
	/// </summary>
	[TestClass]
	public class ServiceTransactionTest
	{
		public TestContext TestContext { get; set; }

		/// <summary>
		/// Initializes test environment.
		/// </summary>
		[TestInitialize]
		public void TestInitialize()
		{
			Cleanup();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			Cleanup();
		}

		[TestMethod]
		public void Success_Begin()
		{
			List<ServiceItem> services = new List<ServiceItem>
			{
				new ServiceItem
				{
					ServiceName = "Test",
					DisplayName = "Test Service",
					BinaryPathName = @"G:\test.exe",
					TargetFramework = TargetFramework.Net35
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

		[TestMethod]
		public void Failed_Begin()
		{
			File.WriteAllText(
				ServiceTransaction.FilePathName,
				Resources.BeginInstalledServicesXmlFile);

			List<ServiceItem> services = new List<ServiceItem>
			{
				new ServiceItem
				{
					ServiceName = "Test",
					DisplayName = "Test Service",
					BinaryPathName = @"G:\test.exe",
					TargetFramework = TargetFramework.Net35
				}
			};

			TestHelper.Throws(
				() => ServiceTransaction.Begin(services),
				typeof(InvalidOperationException));

			File.Delete(ServiceTransaction.FilePathName);
		}

		[TestMethod]
		public void Success_Commit()
		{
			File.WriteAllText(
				ServiceTransaction.FilePathName,
				Resources.BeginInstalledServicesXmlFile);

			ServiceTransaction.Commit();

			bool actual = File.Exists(ServiceTransaction.FilePathName);

			Assert.AreEqual(
				false,
				actual);
		}

		[TestMethod]
		public void Failed_Commit()
		{
			TestHelper.Throws(
				ServiceTransaction.Commit,
				typeof(InvalidOperationException));
		}

		/// <summary>
		/// Tests GetUncommited method.
		/// </summary>
		[TestMethod]
		public void Getting_Uncommited_Services()
		{
			File.WriteAllText(
				ServiceTransaction.FilePathName,
				Resources.BeginInstalledServicesXmlFile);

			var service = ServiceTransaction.GetUncommited();

			File.Delete(ServiceTransaction.FilePathName);

			Assert.AreEqual(
				service.Count,
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
				service[0].TargetFramework);
		}

		private static void Cleanup()
		{
			if (File.Exists(ServiceTransaction.FilePathName))
			{
				File.Delete(ServiceTransaction.FilePathName);
			}
		}
	}
}
