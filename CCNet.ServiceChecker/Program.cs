using System;
using System.Collections;
using System.Text;
using CCNet.Common;
using CCNet.Common.Helpers;
using CCNet.ServiceChecker.Properties;

namespace CCNet.ServiceChecker
{
	class Program
	{
		public static int Main(string[] args)
		{
			/*xxxargs = new[]F
			{
				@"ServiceName=RSDN Editor",
				@"DisplayName=\\rufrt-vxbuild\e$\CCNET",
				@"TargetFramework=",
				@BinaryPathName
			};*/

			if (args == null || args.Length == 0)
			{
				DisplayUsage();
				return 0;
			}

			try
			{
				Arguments.Default = ArgumentProperties.Parse(args);
				PerformChecks();
			}
			catch (Exception e)
			{
				return ErrorHandler.Runtime(e);
			}

			return RaiseError.ExitCode;
		}

		/// <summary>
		/// Performs all checks.
		/// </summary>
		private static void PerformChecks()
		{
			var serviceItemList =
				ServiceHelper.GetServiceItemList(
					Arguments.TargetFramework,
					Arguments.BinaryPathName);

			CheckServiceProjectStructure(serviceItemList);

			if (RaiseError.ExitCode > 0)
				return;

			CheckServiceItem(serviceItemList[0]);
		}

		/// <summary>
		/// Checks "WrongNumberOfServices" condition.
		/// </summary>
		private static void CheckServiceProjectStructure(ICollection serviceItemList)
		{
			int count = 0;

			if (serviceItemList != null)
			{
				count = serviceItemList.Count;
			}

			if (count != 1)
			{
				RaiseError.WrongNumberOfServices(count);
			}
		}

		/// <summary>
		/// Checks "WrongServiceItem" condition.
		/// </summary>
		private static void CheckServiceItem(ServiceItem serviceItem)
		{
			string serviceName = serviceItem.ServiceName;
			string displayName = serviceItem.DisplayName;

			StringBuilder message = new StringBuilder();

			if (serviceName != Arguments.ServiceName)
			{
				message.AppendLine(
					Errors.InvalidServiceName
					.Display(
						Arguments.ServiceName,
						serviceName));
			}

			if (displayName != Arguments.DisplayName)
			{
				message.AppendLine(
					Errors.InvalidServiceName
					.Display(
						Arguments.DisplayName,
						displayName));
			}

			if (message.Length > 0)
			{
				RaiseError.WrongServiceItem(message.ToString());
			}
		}

		/// <summary>
		/// Displays usage text.
		/// </summary>
		private static void DisplayUsage()
		{
			Console.WriteLine();
			Console.WriteLine(Resources.UsageInfo);
			Console.WriteLine();
		}
	}
}
