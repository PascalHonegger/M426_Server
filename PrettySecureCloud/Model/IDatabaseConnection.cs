using System.Data;

namespace PrettySecureCloud.Model
{
	public interface IDatabaseConnection
	{
		/// <summary>
		/// Open a new connection. Always close the connection!
		/// </summary>
		/// <returns>Created and opened connection</returns>
		IDbConnection OpenConnection();

		/// <summary>
		/// Create an empy command which can be used with the connection
		/// </summary>
		/// <returns>Empty command</returns>
		IDbCommand CreateEmpyCommand();
	}
}