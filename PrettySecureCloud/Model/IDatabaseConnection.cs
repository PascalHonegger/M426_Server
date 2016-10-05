using System.Data;

namespace PrettySecureCloud.Model
{
	public interface IDatabaseConnection
	{
		IDbCommand Command { get; }
	}
}