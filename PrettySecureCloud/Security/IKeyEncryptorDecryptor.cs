namespace PrettySecureCloud.Security
{
	public interface IKeyEncryptorDecryptor
	{
		string Encrypt(string key, string password);

		string Decrypt(string encryptedKey, string password);
	}
}
