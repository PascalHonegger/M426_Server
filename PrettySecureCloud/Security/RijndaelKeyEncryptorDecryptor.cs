using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PrettySecureCloud.Security
{
	public class RijndaelKeyEncryptorDecryptor : IKeyEncryptorDecryptor
	{
		private const int CustomKeySize = 16;
		private readonly SymmetricAlgorithm _cryptoService;

		private readonly Random _random = new Random();

		public RijndaelKeyEncryptorDecryptor()
		{
			_cryptoService = new RijndaelManaged();
		}


		public byte[] Encrypt(byte[] key, string password)
		{
			var byteArray = FillUp(Encoding.Default.GetBytes(password));
			return Transform(key, _cryptoService.CreateEncryptor(byteArray, byteArray));
		}

		public byte[] Decrypt(byte[] encryptedKey, string password)
		{
			var byteArray = FillUp(Encoding.Default.GetBytes(password));
			return Transform(encryptedKey, _cryptoService.CreateDecryptor(byteArray, byteArray));
		}

		public byte[] GenerateRandomKey()
		{
			var key = new byte[256];
			_random.NextBytes(key);
			return key;
		}

		private static byte[] FillUp(IReadOnlyList<byte> bytes)
		{
			var result = new byte[CustomKeySize];
			for (var i = 0; i < 16 - 1; i++)
				if (bytes.Count > i)
					result[i] = bytes[i];
			return result;
		}

		private static byte[] Transform(byte[] input, ICryptoTransform cryptoTransform)
		{
			using (var stream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write))
				{
					cryptoStream.Write(input, 0, input.Length);
					cryptoStream.FlushFinalBlock();

					return stream.ToArray();
				}
			}
		}
	}
}