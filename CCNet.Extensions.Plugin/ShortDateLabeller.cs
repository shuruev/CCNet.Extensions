using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace CCNet.Extensions.Plugin
{
	/// <summary>
	/// The ShortDateLabeller builds label using
	/// current date written in short format.
	/// </summary>
	[ReflectorType("shortDateLabeller")]
	public class ShortDateLabeller : ILabeller
	{
		#region ILabeller Members

		public string Generate(IIntegrationResult integrationResult)
		{
			Version oldVersion;

			// try getting old version
			try
			{
				Log.Debug(string.Concat("Old build label is: ", integrationResult.LastIntegration.Label));
				oldVersion = new Version(integrationResult.LastIntegration.Label);
			}
			catch (Exception)
			{
				oldVersion = new Version(0, 0, 0, 0);
			}

			Log.Debug(string.Concat("Old version is: ", oldVersion.ToString()));

			// get current year, month and day
			DateTime now = DateTime.Now;
			int currentYear = now.Year - 2000;
			int currentMonth = now.Month;
			int currentDay = now.Day;

			// get current build number
			int currentBuild = 1;
			if (currentYear == oldVersion.Major
				&& currentMonth == oldVersion.Minor
				&& currentDay == oldVersion.Build)
				currentBuild = oldVersion.Revision + 1;

			Version newVersion = new Version(
				currentYear,
				currentMonth,
				currentDay,
				currentBuild);
			Log.Debug(string.Concat("New version is: ", newVersion.ToString()));

			// return new version string
			return newVersion.ToString();
		}

		#endregion

		#region ITask Members

		public void Run(IIntegrationResult result)
		{
			result.Label = Generate(result);
		}

		#endregion
	}
}
