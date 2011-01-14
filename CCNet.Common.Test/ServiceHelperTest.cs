using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CCNet.Common.Helpers;
using CCNet.Common.Test.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CCNet.Common.Test
{
	/// <summary>
	///This is a test class for ServiceHelperTest and is intended
	///to contain all ServiceHelperTest Unit Tests
	///</summary>
	[TestClass]
	public class ServiceHelperTest
	{
		private static readonly string s_basePath =
			Path.GetDirectoryName(
				Assembly.GetExecutingAssembly().Location);

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		/// <summary>
		/// Tests ParseServicesOutput method.
		///</summary>
		[TestMethod]
		public void ParseServicesOutputTest()
		{
			string output = Resources.ScQueryOutput;
			HashSet<ServiceItem> expected = new HashSet<ServiceItem>
			{
				new ServiceItem {ServiceName = "TermService", DisplayName = "Terminal Services"},
				new ServiceItem {ServiceName = "Themes", DisplayName = "Themes"},
				new ServiceItem {ServiceName = "UNS", DisplayName = "Intel(R) Management and Security Application User Notification Service"}
			};

			HashSet<ServiceItem> actual = ServiceHelper_Accessor.ParseServicesOutput(output);

			Assert.IsTrue(expected.IsSubsetOf(actual));
			Assert.IsTrue(expected.IsSupersetOf(actual));
		}

		/// <summary>
		/// Tests ParseServiceBinaryPathName method.
		/// </summary>
		[TestMethod]
		public void ParseServiceBinaryPathNameTest()
		{
			string expected = @"C:\Windows\system32\svchost.exe";
			string actual = ServiceHelper_Accessor.ParseServiceBinaryPathName(Resources.ScQcWudfsvcOutput);

			Assert.AreEqual(
				expected,
				actual);

			expected = @"D:\Mailbox\CCNet.Extensions\CCNet.Common.Test\bin\Debug\TestWindowsService.exe";
			actual = ServiceHelper_Accessor.ParseServiceBinaryPathName(Resources.ScQcWindowsService1Output);

			Assert.AreEqual(
				expected,
				actual);
		}

		/// <summary>
		/// Tests CreateConsoleCall method.
		/// </summary>
		[TestMethod]
		public void CreateConsoleCallTest()
		{
			Process p =
				ServiceHelper_Accessor.CreateConsoleCall(
					"cmd.exe",
					null);

			p.Start();
			p.StandardInput.WriteLine("exit");
			string output = p.StandardOutput.ReadToEnd();

			Assert.AreEqual(0, p.ExitCode);
			Assert.IsTrue(output.StartsWith("Microsoft Windows"));
		}

		/// <summary>
		/// Tests GetInstalledServices method.
		/// </summary>
		[TestMethod]
		public void GetInstalledServicesTest()
		{
			var services = ServiceHelper_Accessor.GetInstalledServices();
			Assert.IsNotNull(services);
			Assert.IsTrue(services.Count > 0);
		}

		/// <summary>
		/// Tests GetInstalledServiceBinaryPathName method.
		/// </summary>
		[TestMethod]
		public void GetInstalledServiceBinaryPathNameTest()
		{
			string systemRoot = Environment.GetEnvironmentVariable("SystemRoot");

			Assert.IsNotNull(systemRoot);

			string expected = Path.Combine(
				Path.Combine(
					systemRoot,
					"system32"),
				"svchost.exe");

			string actual =
				ServiceHelper_Accessor.GetInstalledServiceBinaryPathName(
					"LanmanServer");

			Assert.AreEqual(
				expected,
				actual);
		}

		/// <summary>
		/// Tests InstallService & UninstallService methods (single service instance, .NET Framework v4.0.30319).
		/// </summary>
		[TestMethod]
		public void InstallUninstallServiceSingle4Test()
		{
			const TargetFramework targetFramework = TargetFramework.Net40;

			string binaryPathName = Path.Combine(
				s_basePath,
				"TestWindowsService.exe");

			bool ok = CompileDynamic(
				Resources.WindowsService1,
				binaryPathName,
				"v4.0.30319");

			Assert.IsTrue(ok);

			var services1 = ServiceHelper_Accessor.GetInstalledServices();

			ok = ServiceHelper_Accessor.InstallService(
				targetFramework,
				binaryPathName);

			Assert.IsTrue(ok);

			var services2 = ServiceHelper_Accessor.GetInstalledServices();
			services2.ExceptWith(services1);

			Assert.AreEqual(
				1,
				services2.Count);

			ServiceItem serviceItem = services2.First();

			Assert.AreEqual(
				"WindowsService1",
				serviceItem.ServiceName);
			Assert.AreEqual(
				"[Test] Windows Service #1",
				serviceItem.DisplayName);

			string actualBinaryPathName =
				ServiceHelper_Accessor.GetInstalledServiceBinaryPathName("WindowsService1");

			Assert.AreEqual(binaryPathName, actualBinaryPathName);

			ok = ServiceHelper_Accessor.UninstallService(
				targetFramework,
				binaryPathName);

			Assert.IsTrue(ok);

			File.Delete(binaryPathName);
		}

		/// <summary>
		/// Tests InstallService & UninstallService methods (single service instance, .NET Framework v2.0.50727).
		/// </summary>
		[TestMethod]
		public void InstallUninstallServiceSingle2Test()
		{
			const TargetFramework targetFramework = TargetFramework.Net20;

			string binaryPathName = Path.Combine(
				s_basePath,
				"TestWindowsService.exe");

			bool ok = CompileDynamic(
				Resources.WindowsService1,
				binaryPathName,
				"v2.0.50727");

			Assert.IsTrue(ok);

			var services1 = ServiceHelper_Accessor.GetInstalledServices();

			ok = ServiceHelper_Accessor.InstallService(
				targetFramework,
				binaryPathName);

			Assert.IsTrue(ok);

			var services2 = ServiceHelper_Accessor.GetInstalledServices();
			services2.ExceptWith(services1);

			Assert.AreEqual(1, services2.Count);

			ServiceItem serviceItem = services2.First();

			Assert.AreEqual(
				"WindowsService1",
				serviceItem.ServiceName);
			Assert.AreEqual(
				"[Test] Windows Service #1",
				serviceItem.DisplayName);

			string actualBinaryPathName =
				ServiceHelper_Accessor.GetInstalledServiceBinaryPathName("WindowsService1");

			Assert.AreEqual(
				binaryPathName,
				actualBinaryPathName);

			ok = ServiceHelper_Accessor.UninstallService(
				targetFramework,
				binaryPathName);

			Assert.IsTrue(ok);

			File.Delete(binaryPathName);
		}

		/// <summary>
		/// Tests InstallService & UninstallService methods (double service instance, .NET Framework v4.0.30319).
		/// </summary>
		[TestMethod]
		public void InstallUninstallServiceDouble4Test()
		{
			const TargetFramework targetFramework = TargetFramework.Net40;

			string binaryPathName = Path.Combine(
				s_basePath,
				"TestWindowsService.exe");

			bool ok = CompileDynamic(
				Resources.WindowsService2,
				binaryPathName,
				"v4.0.30319");

			Assert.IsTrue(ok);

			var services1 = ServiceHelper_Accessor.GetInstalledServices();

			ok = ServiceHelper_Accessor.InstallService(
				targetFramework,
				binaryPathName);

			Assert.IsTrue(ok);

			var services2 = ServiceHelper_Accessor.GetInstalledServices();
			services2.ExceptWith(services1);

			Assert.AreEqual(2, services2.Count);

			ServiceItem serviceItemA = services2.First();

			Assert.AreEqual(
				"WindowsService2a",
				serviceItemA.ServiceName);
			Assert.AreEqual(
				"[Test] Windows Service #2-A",
				serviceItemA.DisplayName);

			string actualBinaryPathNameA =
				ServiceHelper_Accessor.GetInstalledServiceBinaryPathName("WindowsService2a");

			Assert.AreEqual(
				binaryPathName,
				actualBinaryPathNameA);

			ServiceItem serviceItemB = services2.Last();

			Assert.AreEqual(
				"WindowsService2b",
				serviceItemB.ServiceName);
			Assert.AreEqual(
				"[Test] Windows Service #2-B",
				serviceItemB.DisplayName);

			string actualBinaryPathNameB =
				ServiceHelper_Accessor.GetInstalledServiceBinaryPathName("WindowsService2a");

			Assert.AreEqual(
				binaryPathName,
				actualBinaryPathNameB);

			ok = ServiceHelper_Accessor.UninstallService(
				targetFramework,
				binaryPathName);

			Assert.IsTrue(ok);

			File.Delete(binaryPathName);
		}

		/// <summary>
		/// Tests InstallService & UninstallService methods (double service instance, .NET Framework v2.0.50727).
		/// </summary>
		[TestMethod]
		public void InstallUninstallServiceDouble2Test()
		{
			const TargetFramework targetFramework = TargetFramework.Net20;

			string binaryPathName = Path.Combine(
				s_basePath,
				"TestWindowsService.exe");

			bool ok = CompileDynamic(
				Resources.WindowsService2,
				binaryPathName,
				"v2.0.50727");

			Assert.IsTrue(ok);

			var services1 = ServiceHelper_Accessor.GetInstalledServices();

			ok = ServiceHelper_Accessor.InstallService(
				targetFramework,
				binaryPathName);

			Assert.IsTrue(ok);

			var services2 = ServiceHelper_Accessor.GetInstalledServices();
			services2.ExceptWith(services1);

			Assert.AreEqual(2, services2.Count);

			ServiceItem serviceItemA = services2.First();

			Assert.AreEqual("WindowsService2a", serviceItemA.ServiceName);
			Assert.AreEqual("[Test] Windows Service #2-A", serviceItemA.DisplayName);

			string actualBinaryPathNameA = ServiceHelper_Accessor.GetInstalledServiceBinaryPathName("WindowsService2a");

			Assert.AreEqual(binaryPathName, actualBinaryPathNameA);

			ServiceItem serviceItemB = services2.Last();

			Assert.AreEqual("WindowsService2b", serviceItemB.ServiceName);
			Assert.AreEqual("[Test] Windows Service #2-B", serviceItemB.DisplayName);

			string actualBinaryPathNameB = ServiceHelper_Accessor.GetInstalledServiceBinaryPathName("WindowsService2a");

			Assert.AreEqual(binaryPathName, actualBinaryPathNameB);

			ok = ServiceHelper_Accessor.UninstallService(
				targetFramework,
				binaryPathName);

			Assert.IsTrue(ok);

			File.Delete(binaryPathName);
		}

		/// <summary>
		/// Help method for tests.
		/// </summary>
		private static bool CompileDynamic(
			string codeData,
			string outputFilePathName,
			string dotNetVersion)
		{
			string systemRoot = Environment.GetEnvironmentVariable("SystemRoot");

			string cscExePathName = string.Format(
				@"{0}\Microsoft.NET\Framework\{1}\csc.exe",
				systemRoot,
				dotNetVersion);

			string tempCsFilePathName = Path.Combine(s_basePath, "code.cs");

			File.WriteAllText(
				tempCsFilePathName,
				codeData);

			Process p = ServiceHelper_Accessor.CreateConsoleCall(
				cscExePathName,
				string.Format(
					"/optimize \"/out:{0}\" \"{1}\"",
					outputFilePathName,
					tempCsFilePathName));

			p.Start();
			p.StandardOutput.ReadToEnd();

			File.Delete(tempCsFilePathName);

			return p.ExitCode == 0;
		}

		[TestMethod]
		public void GetServiceItemListTest()
		{
			const TargetFramework targetFramework = TargetFramework.Net40;

			string binaryPathName = Path.Combine(
				s_basePath,
				"TestWindowsService.exe");

			bool ok = CompileDynamic(
				Resources.WindowsService2,
				binaryPathName,
				"v4.0.30319");

			Assert.IsTrue(ok);

			List<ServiceItem> services =
				ServiceHelper.GetServiceItemList(
					targetFramework,
					binaryPathName);

			Assert.AreEqual(2, services.Count);

			ServiceItem serviceItemA = services[0];

			Assert.AreEqual(
				"WindowsService2a",
				serviceItemA.ServiceName);
			Assert.AreEqual(
				"[Test] Windows Service #2-A",
				serviceItemA.DisplayName);

			Assert.AreEqual(
				binaryPathName,
				serviceItemA.BinaryPathName);

			Assert.AreEqual(
				targetFramework,
				serviceItemA.TargetFrameWork);

			ServiceItem serviceItemB = services[1];

			Assert.AreEqual(
				"WindowsService2b",
				serviceItemB.ServiceName);
			Assert.AreEqual(
				"[Test] Windows Service #2-B",
				serviceItemB.DisplayName);

			Assert.AreEqual(
				binaryPathName,
				serviceItemB.BinaryPathName);

			Assert.AreEqual(
				targetFramework,
				serviceItemB.TargetFrameWork);

			File.Delete(binaryPathName);
		}

		/// <summary>
		/// Tests DeletePreviouslyInstalledServices method.
		/// </summary>
		[TestMethod]
		public void DeletePreviouslyInstalledServicesTest()
		{
			// create log file
			string log = ServiceTransaction.FilePathName;
			File.WriteAllText(
				log,
				Resources.UncommitedInstalledServicesXmlFile);

			// install service
			const TargetFramework targetFramework = TargetFramework.Net20;

			string binaryPathName = Path.Combine(
				s_basePath,
				"TestWindowsService.exe");

			bool ok = CompileDynamic(
				Resources.WindowsService1,
				binaryPathName,
				"v2.0.50727");

			Assert.IsTrue(ok);

			ok = ServiceHelper_Accessor.InstallService(
				targetFramework,
				binaryPathName);

			Assert.IsTrue(ok);

			// ensure in successfully installation

			string actualBinaryPath =
				ServiceHelper_Accessor.GetInstalledServiceBinaryPathName("WindowsService1");

			Assert.AreEqual(
				binaryPathName,
				actualBinaryPath);

			// call test method

			ServiceHelper.DeletePreviouslyInstalledServices();

			// ensure in successfully uninstallation

			actualBinaryPath =
				ServiceHelper_Accessor.GetInstalledServiceBinaryPathName("WindowsService1");

			Assert.IsNull(actualBinaryPath);
		}
	}
}
