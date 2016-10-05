namespace PrettySecureCloud.Security
{
	public interface IKeyEncryptorDecryptor
	{
		byte[] Encrypt(byte[] key, string password);

		byte[] Decrypt(byte[] encryptedKey, string password);

		byte[] GenerateRandomKey();
	}
}