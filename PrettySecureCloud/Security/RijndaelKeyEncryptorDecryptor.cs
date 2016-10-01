using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PrettySecureCloud.Security
{
	public class RijndaelKeyEncryptorDecryptor : IKeyEncryptorDecryptor
	{
		private readonly SymmetricAlgorithm _cryptoService;
		private const int CustomKeySize = 16;

		public RijndaelKeyEncryptorDecryptor()
		{
			_cryptoService = new RijndaelManaged();
		}

		private static byte[] FillUp(IReadOnlyList<byte> bytes)
		{
			var result = new byte[CustomKeySize];
			for (var i = 0; i < 16 - 1; i++)
			{
				if (bytes.Count > i)
				{
					result[i] = bytes[i];
				}
			}
			return result;
		}

		// vector and key have to match between encryption and decryption
		private string Encrypt(string text, byte[] key, byte[] vector)
		{
			return Transform(text, _cryptoService.CreateEncryptor(key, vector));
		}

		// vector and key have to match between encryption and decryption
		private string Decrypt(string text, byte[] key, byte[] vector)
		{
			return Transform(text, _cryptoService.CreateDecryptor(key, vector));
		}

		private static string Transform(string text, ICryptoTransform cryptoTransform)
		{
			using (var stream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write))
				{
					var input = Encoding.Default.GetBytes(text);

					cryptoStream.Write(input, 0, input.Length);
					cryptoStream.FlushFinalBlock();

					return Encoding.Default.GetString(stream.ToArray());
				}
			}
		}




		public string Encrypt(string key, string password)
		{
			var byteArray = FillUp(Encoding.Default.GetBytes(password));
			return Encrypt(key, byteArray, byteArray);
		}

		public string Decrypt(string encryptedKey, string password)
		{
			var byteArray = FillUp(Encoding.Default.GetBytes(password));
			return Decrypt(encryptedKey, byteArray, byteArray);
		}
	}
}