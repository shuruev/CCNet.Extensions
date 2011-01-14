using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

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
		public static void Begin(ServiceItem[] services)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ServiceItem[]));

			List<ServiceItem> list = new List<ServiceItem>();

			if (File.Exists(FilePathName))
			{
				using (TextReader tr = new StreamReader(FilePathName))
				{
					list.AddRange(
						(ServiceItem[])serializer.Deserialize(tr));
				}
			}

			list.AddRange(services);

			using (TextWriter tw = new StreamWriter(FilePathName))
			{
				serializer.Serialize(tw, list.ToArray());
			}
		}

		/// <summary>
		/// Commit service transaction.
		/// </summary>
		public static void Commit(string serviceExecutablePath)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ServiceItem[]));

			var list = new ServiceItem[0];

			if (File.Exists(FilePathName))
			{
				using (TextReader tr = new StreamReader(FilePathName))
				{
					list = (ServiceItem[])serializer.Deserialize(tr);
				}
			}

			using (TextWriter tw = new StreamWriter(FilePathName))
			{
				serializer.Serialize(
					tw,
					list
						.Where(service => service.BinaryPathName != serviceExecutablePath)
						.ToArray());
			}
		}

		/// <summary>
		/// Gets uncommited services.
		/// </summary>
		public static ServiceItem[] GetUncommited()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ServiceItem[]));

			if (File.Exists(FilePathName))
			{
				using (TextReader tr = new StreamReader(FilePathName))
				{
					return (ServiceItem[])serializer.Deserialize(tr);
				}
			}

			return new ServiceItem[0];
		}
	}
}
