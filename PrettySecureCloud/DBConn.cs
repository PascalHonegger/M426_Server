using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using PrettySecureCloud.Properties;

namespace PrettySecureCloud
{
	public static class DBConn
{
		private static SqlConnection _conn;

		private static SqlConnection Connection => _conn ?? (_conn = new SqlConnection(Settings.Default.ConnectionString) );

		public static SqlCommand Command => Connection.CreateCommand();

}
}