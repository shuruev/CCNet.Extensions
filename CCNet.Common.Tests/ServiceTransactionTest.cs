using System;
using System.Collections.Generic;
using System.IO;
using CCNet.Common.Helpers;
using CCNet.Common.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCNet.Common.Tests
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
		/// Initializes test environment.
		/// </summary>
		[TestInitialize]
		public void TestInitialize()
		{
			Cleanup();
		}

		/// <summary>
		/// Disposes test environment.
		/// </summary>
		[TestCleanup]
		public void TestCleanup()
		{
			Cleanup();
		}

		/// <summary>
		/// Tests Begin method (success).
		/// </summary>
		[TestMethod]
		public void BeginSuccessTest()
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

		/// <summary>
		/// Tests Begin method (failed).
		/// </summary>
		[TestMethod]
		public void BeginFailedTest()
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

			bool actual = false;
			try
			{
				ServiceTransaction.Begin(services);
			}
			catch (InvalidOperationException)
			{
				actual = true;
			}

			Assert.AreEqual(
				true,
				actual);

			File.Delete(ServiceTransaction.FilePathName);
		}

		/// <summary>
		/// Tests Commit method (success).
		/// </summary>
		[TestMethod]
		public void CommitSuccessTest()
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

		/// <summary>
		/// Tests Commit method (failure).
		/// </summary>
		[TestMethod]
		public void CommitFailureTest()
		{
			bool thrown = false;
			try
			{
				ServiceTransaction.Commit();
			}
			catch (InvalidOperationException)
			{
				thrown = true;
			}

			Assert.AreEqual(
				true,
				thrown);
		}

		/// <summary>
		/// Tests GetUncommited method.
		/// </summary>
		[TestMethod]
		public void GetUncommitedTest()
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
