using System.Collections.Generic;
using System.Data.SqlClient;
using Lean.Database;

namespace CCNet.Build.SetupPackages
{
	public class NuGetDb
	{
		private readonly string m_connectionString;

		public NuGetDb(string connectionString)
		{
			m_connectionString = connectionString;
		}

		public List<NuGetPackage> GetLatestVersions()
		{
			var result = new List<NuGetPackage>();

			using (var conn = new SqlConnection(m_connectionString))
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText =
						@"
WITH T AS
(
	SELECT
		PR.Id,
		P.[Version],
		DENSE_RANK() OVER (PARTITION BY PR.[Key] ORDER BY P.[Key] DESC) AS [Rank]
	FROM PackageRegistrations PR
		INNER JOIN Packages P
		ON P.PackageRegistrationKey = PR.[Key]
)

SELECT Id, [Version]
FROM T
WHERE [Rank] = 1
						";

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var id = reader.ReadString("Id");
							var version = reader.ReadString("Version");

							result.Add(new NuGetPackage(id, version));
						}
					}
				}
			}

			return result;
		}
	}
}
