using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using CCNet.Common.Properties;

namespace CCNet.Common.Helpers
{
	/// <summary>
	/// Service Transaction class.
	/// </summary>
	public static class ServiceTransaction
	{
		/// <summary>
		/// Gets transaction file path name.
		/// </summary>
		public static readonly string FilePathName =
			Path.Combine(
				Path.GetDirectoryName(
					Assembly.GetCallingAssembly().Location),
				"installed-services.xml");

		/// <summary>
		/// Opens service transaction.
		/// </summary>
		public static void Begin(List<ServiceItem> services)
		{
			if (File.Exists(FilePathName))
			{
				throw new InvalidOperationException(Resources.UnclosedTransactionFileFound);
			}

			XmlSerializer serializer = new XmlSerializer(typeof(List<ServiceItem>));
			using (TextWriter tw = new StreamWriter(FilePathName))
			{
				serializer.Serialize(tw, services);
			}
		}

		/// <summary>
		/// Commit service transaction.
		/// </summary>
		public static void Commit()
		{
			if (!File.Exists(FilePathName))
			{
				throw new InvalidOperationException(Resources.UnclosedTransactionFileFound);
			}

			File.Delete(FilePathName);
		}

		/// <summary>
		/// Gets uncommited services.
		/// </summary>
		public static List<ServiceItem> GetUncommited()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<ServiceItem>));

			if (File.Exists(FilePathName))
			{
				using (TextReader tr = new StreamReader(FilePathName))
				{
					return (List<ServiceItem>)serializer.Deserialize(tr);
				}
			}

			return new List<ServiceItem>();
		}
	}
}
