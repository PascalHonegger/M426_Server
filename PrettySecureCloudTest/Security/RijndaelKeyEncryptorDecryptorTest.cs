using System.Text;
using NUnit.Framework;
using PrettySecureCloud.Security;

namespace PrettySecureCloudTest.Security
{
	[TestFixture]
	[Category("UnitTest")]
	public class RijndaelKeyEncryptorDecryptorTest
	{
		[SetUp]
		public void Setup()
		{
			_unitUnderTest = new RijndaelKeyEncryptorDecryptor();
		}

		[TearDown]
		public void Teardown()
		{
			_unitUnderTest = null;
		}

		private const string ExamplePassword = "Test P@ssw0rd";
		private const string ExampleKey = "S4F9AKJSKER8W9AKFADSJK";

		private RijndaelKeyEncryptorDecryptor _unitUnderTest;

		[Test]
		public void EncryptedStringCanBeDecrypted()
		{
			//Arrange
			var encryptedKey = _unitUnderTest.Encrypt(Encoding.Default.GetBytes(ExampleKey), ExamplePassword);

			//Act
			var result = _unitUnderTest.Decrypt(encryptedKey, ExamplePassword);

			//Assert
			Assert.That(result, Is.EqualTo(ExampleKey));
		}
	}
}