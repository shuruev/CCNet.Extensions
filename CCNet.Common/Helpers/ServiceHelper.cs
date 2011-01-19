using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using CCNet.Common.Properties;

namespace CCNet.Common.Helpers
{
	/// <summary>
	/// Common methods for working with properties collections.
	/// </summary>
	public static class ServiceHelper
	{
		private static readonly Regex s_serviceItemRegex =
			new Regex(@"SERVICE_NAME: (?<serviceName>.+)\r\nDISPLAY_NAME: (?<displayName>.+)\r\n");

		private static readonly Regex s_binaryPathNameRegex =
			new Regex(@"BINARY_PATH_NAME\s+: (?<binaryPathName>.+)\r\n");

		private static readonly string s_systemRoot = Environment.GetEnvironmentVariable("SystemRoot");

		private static readonly string s_installUtil20Path =
			string.Format(
				@"{0}\Microsoft.NET\Framework\v2.0.50727\installutil.exe",
				s_systemRoot);

		private static readonly string s_installUtil40Path =
			string.Format(
				@"{0}\Microsoft.NET\Framework\v4.0.30319\installutil.exe",
				s_systemRoot);

		/// <summary>
		/// Deletes previously installed services.
		/// </summary>
		public static void DeletePreviouslyInstalledServices()
		{
			var services = ServiceTransaction.GetUncommited();

			foreach (var service in services)
			{
				UninstallService(
					service.TargetFramework,
					service.BinaryPathName);
			}

			if (services.Count > 0)
			{
				ServiceTransaction.Commit();
			}
		}

		/// <summary>
		/// Gets services properties.
		/// </summary>
		public static List<ServiceItem> GetServiceItemList(
			TargetFramework targetFramework,
			string binaryPathName)
		{
			HashSet<ServiceItem> oldServices = GetInstalledServices();

			InstallService(
				targetFramework,
				binaryPathName);

			HashSet<ServiceItem> newServices = GetInstalledServices();

			newServices.ExceptWith(oldServices);

			foreach (var serviceItem in newServices)
			{
				serviceItem.TargetFramework = targetFramework;
				serviceItem.BinaryPathName = GetInstalledServiceBinaryPathName(serviceItem.ServiceName);
			}

			ServiceTransaction.Begin(newServices.ToList());

			UninstallService(
				targetFramework,
				binaryPathName);

			ServiceTransaction.Commit();

			return newServices.ToList();
		}

		/// <summary>
		/// Gets binary path name for service with specified service name.
		/// </summary>
		private static string GetInstalledServiceBinaryPathName(string serviceName)
		{
			Process p = CreateCustomProcess(
				"sc",
				string.Format(
					"qc {0}",
					serviceName));

			p.Start();

			string output = p.StandardOutput.ReadToEnd();

			if (p.ExitCode != 0)
			{
				return null;
			}

			var binaryPathName = ParseServiceBinaryPathName(output);

			return binaryPathName;
		}

		/// <summary>
		/// Gets properties of installed services.
		/// </summary>
		private static HashSet<ServiceItem> GetInstalledServices()
		{
			Process p = CreateCustomProcess(
				"sc",
				"query state= all");

			p.Start();

			string output = p.StandardOutput.ReadToEnd();

			var serviceItemSet = ParseServicesOutput(output);
			return serviceItemSet;
		}

		/// <summary>
		/// Parses output of "sc query" command to extract currently installed services.
		/// </summary>
		private static HashSet<ServiceItem> ParseServicesOutput(string output)
		{
			HashSet<ServiceItem> serviceItemSet = new HashSet<ServiceItem>();

			foreach (Match match in s_serviceItemRegex.Matches(output))
			{
				string serviceName = match.Groups["serviceName"].Value;
				string displayName = match.Groups["displayName"].Value;

				ServiceItem sp = new ServiceItem { ServiceName = serviceName, DisplayName = displayName };
				serviceItemSet.Add(sp);
			}

			return serviceItemSet;
		}

		/// <summary>
		/// Parses output of "sc qc SERVICE_NAME" command to extract binary path name.
		/// </summary>
		private static string ParseServiceBinaryPathName(string output)
		{
			foreach (Match match in s_binaryPathNameRegex.Matches(output))
			{
				string binaryPathName = match.Groups["binaryPathName"].Value;
				if (binaryPathName.StartsWith("\""))
				{
					var array = binaryPathName.Split(new[] { '"' }, StringSplitOptions.RemoveEmptyEntries);
					binaryPathName = array[0];
				}
				else if (binaryPathName.Contains(" "))
				{
					var array = binaryPathName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					binaryPathName = array[0];
				}

				return binaryPathName;
			}

			return null;
		}

		/// <summary>
		/// Installs services.
		/// </summary>
		private static void InstallService(
			TargetFramework targetFramework,
			string binaryPathName)
		{
			string installUtilPath = null;
			switch (targetFramework)
			{
				case TargetFramework.Net20:
				case TargetFramework.Net35:
					installUtilPath = s_installUtil20Path;
					break;
				case TargetFramework.Net40:
					installUtilPath = s_installUtil40Path;
					break;
			}

			Process p = CreateCustomProcess(
				installUtilPath,
				string.Format(
					"\"{0}\"",
					binaryPathName),
				true);

			p.Start();
			p.WaitForExit();

			if (p.ExitCode == 0)
			{
				return;
			}

			string message = string.Format(
					Resources.ServiceInstallError,
					binaryPathName);

			throw new InvalidOperationException(message);
		}

		/// <summary>
		/// Uninstalls services.
		/// </summary>
		private static void UninstallService(
			TargetFramework targetFramework,
			string binaryPathName)
		{
			string installUtilPath = null;
			switch (targetFramework)
			{
				case TargetFramework.Net20:
				case TargetFramework.Net35:
					installUtilPath = s_installUtil20Path;
					break;
				case TargetFramework.Net40:
					installUtilPath = s_installUtil40Path;
					break;
			}

			Process p = CreateCustomProcess(
				installUtilPath,
				string.Format(
					"/u \"{0}\"",
					binaryPathName),
				true);

			p.Start();
			p.WaitForExit();

			if (p.ExitCode == 0)
			{
				return;
			}

			string message = string.Format(
					Resources.ServiceUninstallError,
					binaryPathName);

			throw new InvalidOperationException(message);
		}

		/// <summary>
		/// Creates process with custom settings.
		/// </summary>
		private static Process CreateCustomProcess(
			string command,
			string arguments,
			bool runAsAdministrator = false)
		{
			Process p = new Process();

			p.StartInfo.FileName = command;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = runAsAdministrator;
			p.StartInfo.RedirectStandardOutput = !runAsAdministrator;
			p.StartInfo.RedirectStandardInput = !runAsAdministrator;
			p.StartInfo.RedirectStandardError = !runAsAdministrator;
			if (runAsAdministrator)
			{
				p.StartInfo.Verb = "runas";
			}

			return p;
		}
	}
}
