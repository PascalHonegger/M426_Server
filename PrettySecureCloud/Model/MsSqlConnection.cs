using System.Data;
using System.Data.SqlClient;
using PrettySecureCloud.Properties;

namespace PrettySecureCloud.Model
{
	public class MsSqlConnection : IDatabaseConnection
	{
		/// <inheritdoc />
		public IDbConnection OpenConnection()
		{
			var connection = new SqlConnection(Settings.Default.ConnectionString);

			connection.Open();

			return connection;
		}

		public IDbCommand CreateEmpyCommand()
		{
			return new SqlCommand();
		}
	}
}