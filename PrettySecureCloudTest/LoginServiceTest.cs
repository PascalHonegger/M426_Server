using NUnit.Framework;
using PrettySecureCloud;

namespace PrettySecureCloudTest
{
	[TestFixture]
	[Category("IntegrationTest")]
	public class LoginServiceTest
	{
		[SetUp]
		public void Setup()
		{
			_unitUnderTest = new LoginService();
		}

		[TearDown]
		public void Teardown()
		{
			_unitUnderTest = null;
		}

		private LoginService _unitUnderTest;

		[Test]
		public void TestUsernameUnique()
		{
			//Arrange
			//TODO Random user creation and deletion!
			var randomUser = new User();

			//Act
			var unique = _unitUnderTest.UsernameUnique(randomUser.Username);

			//Assert
			Assert.That(unique, Is.False);
		}
	}
}