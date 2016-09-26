using System.Data.SqlClient;
using PrettySecureCloud.Properties;

namespace PrettySecureCloud
{
	public static class DbConn
	{
		private static SqlConnection _connection;

		private static SqlConnection Connection
			=> _connection ?? (_connection = new SqlConnection(Settings.Default.ConnectionString));

		public static SqlCommand Command => Connection.CreateCommand();
	}
}