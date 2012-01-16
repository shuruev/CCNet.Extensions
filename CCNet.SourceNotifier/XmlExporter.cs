using System;
using System.Linq;
using System.Xml.Linq;
using CCNet.SourceNotifier.UserInfo;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CCNet.SourceNotifier
{
	/// <summary>
	/// Incapsulates XML generation.
	/// </summary>
	internal static class XmlExporter
	{
		/// <summary>
		/// Converts TimeSpan to string.
		/// </summary>
		public static string ExportTimeSpanDays(TimeSpan timeSpan)
		{
			return ((int)timeSpan.TotalDays).ToString();
		}

		/// <summary>
		/// Converts DateTime to string.
		/// </summary>
		public static string ExportDateTime(DateTime dateTime)
		{
			return dateTime.ToString();
		}

		/// <summary>
		/// Creates the standart XML document for the specified content.
		/// </summary>
		public static XDocument CreateDocument(object content)
		{
			return new XDocument(new XElement("root", content));
		}

		/// <summary>
		/// Creates XML element containing the information on the specified user.
		/// </summary>
		public static XElement ExportUserInfo(IUserInfo userInfo)
		{
			var result = new XElement("userInfo");
			result.Add(new XElement("displayName", userInfo.DisplayName));
			result.Add(new XElement("isRegistered", userInfo.IsRegistered));
			if (userInfo.IsRegistered)
			{
				var registeredInfo = (RegisteredUserInfo)userInfo;
				result.Add(new XElement("firstName", registeredInfo.FirstName));
				result.Add(new XElement("description", registeredInfo.Description));
				result.Add(new XElement("email", registeredInfo.EmailAddress));
				result.Add(new XElement("isLockedOut", registeredInfo.IsLockedOut));
				if (registeredInfo.LastLogon.HasValue)
				{
					result.Add(new XElement("lastLogon", ExportDateTime(registeredInfo.LastLogon.Value)));
					result.Add(new XElement("daysSinceLastLogon", ExportTimeSpanDays(DateTime.Now - registeredInfo.LastLogon.Value)));
				}
			}

			return result;
		}

		/// <summary>
		/// Creates XML element containing the information on the specified pending change.
		/// </summary>
		public static XElement ExportPendingChange(PendingChange change)
		{
			return new XElement(
				"change",
				new XElement("path", change.ServerItem),
				new XElement("checkoutDate", ExportDateTime(change.CreationDate)),
				new XElement("daysSinceCheckout", ExportTimeSpanDays(DateTime.Now - change.CreationDate)));
		}

		/// <summary>
		/// Creates XML element containing the information on the specified pending changes group.
		/// </summary>
		public static XElement ExportPendingChangesGroup(IGrouping<IUserInfo, PendingChange> changesGroup)
		{
			return new XElement(
				"group",
				ExportUserInfo(changesGroup.Key),
				from change in changesGroup select ExportPendingChange(change));
		}
	}
}
