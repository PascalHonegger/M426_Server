using NUnit.Framework;
using PrettySecureCloud;
using PrettySecureCloud.Security;

namespace PrettySecureCloudTest.Security
{
	[TestFixture]
	[Category("UnitTest")]
	public class BCryptHasherTest
	{
		private const string ExamplePassword = "Test P@ssw0rd";

		[SetUp]
		public void Setup()
		{
			_unitUnderTest = new BCryptHasher();
		}

		[TearDown]
		public void Teardown()
		{
			_unitUnderTest = null;
		}

		private BCryptHasher _unitUnderTest;

		[Test]
		public void CalculatedHashCanBeVerified()
		{
			//Arrange
			var hash = _unitUnderTest.CalculateHash(ExamplePassword);

			//Act
			var result = _unitUnderTest.Verify(ExamplePassword, hash);

			//Assert
			Assert.That(result, Is.True);
		}

		[Test]
		public void HashIsNotEqualToInput()
		{
			//Act
			var result = _unitUnderTest.CalculateHash(ExamplePassword);

			//Assert
			Assert.That(result, Is.Not.EqualTo(ExamplePassword));
		}

		[Test]
		public void VerifyOnWrongPasswordReturnsFalse()
		{
			//Arrange
			var hash = _unitUnderTest.CalculateHash(ExamplePassword);

			//Act
			var result = _unitUnderTest.Verify(ExamplePassword.Remove(0), hash);

			//Assert
			Assert.That(result, Is.False);
		}
	}
}