using System.Data.SqlClient;
using PrettySecureCloud.Properties;

namespace PrettySecureCloud
{
	public static class DbConn
	{
		static DbConn()
		{
			Connection = new SqlConnection(Settings.Default.ConnectionString);

			Connection.Open();
		}

		private static SqlConnection Connection	{ get; }

		public static SqlCommand Command => Connection.CreateCommand();
	}
}