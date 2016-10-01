namespace PrettySecureCloud.Security
{
	public interface IPasswordHasher
	{
		/// <summary>
		/// Calculate the hash of the provided string
		/// </summary>
		/// <param name="password">String to be hashed</param>
		/// <returns>Hash of the provided <param name="password" /></returns>
		string CalculateHash(string password);

		/// <summary>
		/// Validates the given password to the hash
		/// </summary>
		/// <param name="hash">Hash (saved in database)</param>
		/// <param name="password">Password (provided by user)</param>
		/// <returns>True, if the password entered matches the hash</returns>
		bool Verify(string password, string hash);
	}
}
