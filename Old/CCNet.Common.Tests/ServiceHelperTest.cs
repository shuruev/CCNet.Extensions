using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CCNet.Common.Helpers;
using CCNet.Common.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCNet.Common.Tests
{
	/// <summary>
	/// Tests ServiceHelper class.
	/// </summary>
	[TestClass]
	public class ServiceHelperTest
	{
		private static readonly string s_basePath =
			Path.GetDirectoryName(
				Assembly.GetExecutingAssembly().Location);

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

		public TestContext TestContext { get; set; }

		[TestMethod]
		public void Parsing_Of_Sc_Query_Output()
		{
			string output = Resources.ScQueryOutput;
			HashSet<ServiceItem> expected = new HashSet<ServiceItem>
			{
				new ServiceItem
				{
					ServiceName = "TermService",
					DisplayName = "Terminal Services"
				},
				new ServiceItem
				{
					ServiceName = "Themes",
					DisplayName = "Themes"
				},
				new ServiceItem
				{
					ServiceName = "UNS",
					DisplayName = "Intel(R) Management and Security Application User Notification Service"
				}
			};

			HashSet<ServiceItem> actual = ServiceHelper_Accessor.ParseServicesOutput(output);

			Assert.IsTrue(expected.IsSubsetOf(actual));
			Assert.IsTrue(expected.IsSupersetOf(actual));
		}

		[TestMethod]
		public void Parsing_Of_Sc_Qc_Output()
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

		[TestMethod]
		public void Custom_Process_Creation()
		{
			Process p =
				ServiceHelper_Accessor.CreateCustomProcess(
					"cmd.exe",
					null);

			p.Start();
			p.StandardInput.WriteLine("exit");
			string output = p.StandardOutput.ReadToEnd();

			Assert.AreEqual(0, p.ExitCode);
			Assert.IsTrue(output.StartsWith("Microsoft Windows"));
		}

		[TestMethod]
		public void Getting_Installed_Services()
		{
			var services = ServiceHelper_Accessor.GetInstalledServices();
			Assert.IsNotNull(services);
			Assert.IsTrue(services.Count > 0);
		}

		[TestMethod]
		public void Getting_Binary_File_Name_Of_Installed_Service()
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

		[TestMethod]
		public void Installing_And_Uninstalling_Single_Service_NET_4()
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

			ServiceHelper_Accessor.InstallService(
				targetFramework,
				binaryPathName);

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

			ServiceHelper_Accessor.UninstallService(
				targetFramework,
				binaryPathName);

			File.Delete(binaryPathName);
		}

		[TestMethod]
		public void Installing_And_Uninstalling_Single_Service_NET_2()
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

			ServiceHelper_Accessor.InstallService(
				targetFramework,
				binaryPathName);

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

			ServiceHelper_Accessor.UninstallService(
				targetFramework,
				binaryPathName);

			File.Delete(binaryPathName);
		}

		[TestMethod]
		public void Installing_And_Uninstalling_Multiple_Service_NET_4()
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

			ServiceHelper_Accessor.InstallService(
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

			ServiceHelper_Accessor.UninstallService(
				targetFramework,
				binaryPathName);

			File.Delete(binaryPathName);
		}

		[TestMethod]
		public void Installing_And_Uninstalling_Multiple_Service_NET_2()
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

			ServiceHelper_Accessor.InstallService(
				targetFramework,
				binaryPathName);

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

			ServiceHelper_Accessor.UninstallService(
				targetFramework,
				binaryPathName);

			File.Delete(binaryPathName);
		}

		/// <summary>
		/// Tests GetServiceItemList method.
		/// </summary>
		[TestMethod]
		public void Getting_Services_Info_From_Binary_File()
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
				serviceItemA.TargetFramework);

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
				serviceItemB.TargetFramework);

			File.Delete(binaryPathName);
		}

		/// <summary>
		/// Tests DeletePreviouslyInstalledServices method.
		/// </summary>
		[TestMethod]
		public void Deleting_Service_Using_Uncommited_Transaction_Info()
		{
			string binaryPathName = Path.Combine(
				s_basePath,
				"TestWindowsService.exe");

			// create log file
			string log = ServiceTransaction.FilePathName;
			File.WriteAllText(
				log,
				string.Format(
					Resources.UncommitedInstalledServicesXmlFile,
					binaryPathName));

			// install service
			const TargetFramework targetFramework = TargetFramework.Net20;

			bool ok = CompileDynamic(
				Resources.WindowsService1,
				binaryPathName,
				"v2.0.50727");

			Assert.IsTrue(ok);

			ServiceHelper_Accessor.InstallService(
				targetFramework,
				binaryPathName);

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

		/// <summary>
		/// Compiles C# code in run time.
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

			Process p =
				ServiceHelper_Accessor.CreateCustomProcess(
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

		/// <summary>
		/// Performs test cleanup.
		/// </summary>
		private static void Cleanup()
		{
			if (File.Exists(ServiceTransaction.FilePathName))
			{
				File.Delete(ServiceTransaction.FilePathName);
			}

			var sourceCodes = new Dictionary<string, string>
			{
				{ "WindowsService1", Resources.WindowsService1 },
				{ "WindowsService2a", Resources.WindowsService2 },
				{ "WindowsService2b", Resources.WindowsService2 }
			};

			var services =
				ServiceHelper_Accessor.GetInstalledServices()
					.Where(p => sourceCodes.Keys.Contains(p.ServiceName));

			foreach (var service in services)
			{
				service.TargetFramework =
					TargetFramework.Net40;
				service.BinaryPathName =
					ServiceHelper_Accessor.GetInstalledServiceBinaryPathName(service.ServiceName);

				CleanupServiceInstallation(
					service,
					sourceCodes[service.ServiceName]);
			}
		}

		/// <summary>
		/// Uninstalls service.
		/// </summary>
		private static void CleanupServiceInstallation(
			ServiceItem service,
			string sourceCode)
		{
			if (File.Exists(service.BinaryPathName))
			{
				File.Delete(service.BinaryPathName);
			}

			CompileDynamic(
				sourceCode,
				service.BinaryPathName,
				"v4.0.30319");

			try
			{
				ServiceHelper_Accessor.UninstallService(
					service.TargetFramework,
					service.BinaryPathName);
			}
			catch (InvalidOperationException)
			{
			}

			if (File.Exists(service.BinaryPathName))
			{
				File.Delete(service.BinaryPathName);
			}
		}
	}
}
