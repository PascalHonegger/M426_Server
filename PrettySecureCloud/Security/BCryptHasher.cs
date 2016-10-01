namespace PrettySecureCloud.Security
{
	// ReSharper disable once InconsistentNaming
	public class BCryptHasher : IPasswordHasher
	{
		/// <summary>
		/// Calculate the hash of the provided string
		/// </summary>
		/// <param name="password">String to be hashed</param>
		/// <returns>Hash of the provided <param name="password" /></returns>
		public string CalculateHash(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		/// <summary>
		/// Validates the given password to the hash
		/// </summary>
		/// <param name="hash">Hash (saved in database)</param>
		/// <param name="password">Password (provided by user)</param>
		/// <returns>True, if the password entered matches the hash</returns>
		public bool Validate(string password, string hash)
		{
			return BCrypt.Net.BCrypt.Verify(password, hash);
		}
	}
}