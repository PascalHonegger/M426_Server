using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using PrettySecureCloud;
using PrettySecureCloud.Exceptions;
using PrettySecureCloud.Model;

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

			foreach (var tempUser in _temporaryUsers)
			{
				foreach (var service in tempUser.Services.ToList())
				{
					_deleteServicesFromUserCommand.CommandText = $"DELETE FROM tbl_User_Service WHERE fk_User={service.Id}";
					_deleteServicesFromUserCommand.ExecuteNonQuery();
				}
				_deleteUserCommand.CommandText = $"DELETE FROM tbl_User WHERE id_User={tempUser.Id}";
				_deleteUserCommand.ExecuteNonQuery();
			}
		}

		private readonly IDbCommand _deleteUserCommand;
		private readonly IDbCommand _deleteServicesFromUserCommand;

		public LoginServiceTest()
		{
			var connection = new MsSqlConnection();

			_deleteUserCommand = connection.Command;
			_deleteServicesFromUserCommand = connection.Command;
		}

		private LoginService _unitUnderTest;

		private readonly IList<User> _temporaryUsers = new List<User>();

		private User TemporaryUser => RegisterTemporaryUser(RandomString, RandomString);

		private User RegisterTemporaryUser(string username, string password)
		{
			_unitUnderTest.Register(username, RandomString, password);
			var randomUser = _unitUnderTest.Login(username, password);

			_temporaryUsers.Add(randomUser);
			return randomUser;
		}

		private static string RandomString => Guid.NewGuid().ToString();

		[Test]
		public void ChangePasswordThrowsAnExceptionIfPasswordIsWrong()
		{
			//Arrange
			var user = TemporaryUser;

			//Act & Assert
			Assert.Throws<WrongCredentialsException>(() => _unitUnderTest.ChangePassword(user.Id, "Wrong password", RandomString));
		}

		[Test]
		public void LoginThrowsAnExceptionWithWrongCredentials()
		{
			//Act & Assert
			Assert.Throws<WrongCredentialsException>(() => _unitUnderTest.Login(RandomString, RandomString));
		}

		[Test]
		public void LoginWorksAfterPasswordChange()
		{
			//Arrange
			var username = RandomString;
			var oldPassword = RandomString;
			var newPassword = RandomString;
			var createdUser = RegisterTemporaryUser(username, oldPassword);
			_unitUnderTest.ChangePassword(createdUser.Id, oldPassword, newPassword);

			//Act & Assert
			Assert.Throws<WrongCredentialsException>(() => _unitUnderTest.Login(username, oldPassword));
			var loadedUser = _unitUnderTest.Login(username, newPassword);

			Assert.That(loadedUser.EncryptionKey, Is.EqualTo(createdUser.EncryptionKey));
		}

		[Test]
		public void RegisterThrowsAnExceptionIfUserAlreadyExists()
		{
			//Arrange
			var existingUser = TemporaryUser;

			//Act & Assert
			Assert.Throws<UserAlreadyExistsException>(
				() => _unitUnderTest.Register(existingUser.Username, RandomString, RandomString));
			Assert.Throws<UserAlreadyExistsException>(
				() => _unitUnderTest.Register(RandomString, existingUser.Mail, RandomString));
		}

		[Test]
		public void TestEmailUnique()
		{
			//Arrange
			var randomUser = TemporaryUser;

			//Act
			var unique = _unitUnderTest.EmailUnique(randomUser.Mail);

			//Assert
			Assert.That(unique, Is.False);
		}

		[Test]
		public void TestUsernameUnique()
		{
			//Arrange
			var randomUser = TemporaryUser;

			//Act
			var unique = _unitUnderTest.UsernameUnique(randomUser.Username);

			//Assert
			Assert.That(unique, Is.False);
		}
	}
}