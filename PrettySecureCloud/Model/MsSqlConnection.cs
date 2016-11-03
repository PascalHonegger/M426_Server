using System.Data;
using System.Data.SqlClient;
using PrettySecureCloud.Properties;

namespace PrettySecureCloud.Model
{
	public class MsSqlConnection : IDatabaseConnection
	{
		public MsSqlConnection()
		{
			_connection = new SqlConnection(Settings.Default.ConnectionString);

			_connection.Open();
		}

		private readonly SqlConnection _connection;

		public IDbCommand Command => _connection.CreateCommand();

		~MsSqlConnection()
		{
			_connection?.Close();
			_connection?.Dispose();
		}
	}
}