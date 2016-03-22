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
SELECT MAX([Key]) AS LatestKey
INTO #Latest
FROM Packages
GROUP BY PackageRegistrationKey

SELECT
	PR.Id,
	P.[Version],
	PF.TargetFramework
FROM #Latest T
	INNER JOIN Packages P
	ON P.[Key] = T.LatestKey
	INNER JOIN PackageRegistrations PR
	ON PR.[Key] = P.PackageRegistrationKey
	INNER JOIN PackageFrameworks PF
	ON PF.[Key] = P.[Key]

DROP TABLE #Latest
						";

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var id = reader.ReadString("Id");
							var version = reader.ReadString("Version");
							var framework = reader.ReadString("TargetFramework");

							result.Add(new NuGetPackage(id, version, framework));
						}
					}
				}
			}

			return result;
		}
	}
}
