using System.Data;
using System.Data.SqlClient;
using PrettySecureCloud.Properties;

namespace PrettySecureCloud.Model
{
	public class MsSqlConnection : IDatabaseConnection
	{
		public MsSqlConnection()
		{
			Connection = new SqlConnection(Settings.Default.ConnectionString);

			Connection.Open();
		}

		private SqlConnection Connection { get; }

		public IDbCommand Command => Connection.CreateCommand();
	}
}