using System;
using System.Collections.Generic;
using System.Data;
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
		private readonly IDbCommand _deleteUserCommand;
		private readonly IDbCommand _deleteServicesFromUserCommand;

		public LoginServiceTest()
		{
			var connection = new MsSqlConnection();

			_deleteUserCommand = connection.Command;
			_deleteServicesFromUserCommand = connection.Command;
		}

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
				foreach (var service in tempUser.Services)
				{
					_deleteServicesFromUserCommand.CommandText = $"DELETE FROM tbl_User_Service WHERE fk_User={service.Id}";
					_deleteServicesFromUserCommand.ExecuteNonQuery();
				}
				_deleteUserCommand.CommandText = $"DELETE FROM tbl_User WHERE id_User={tempUser.Id}";
				_deleteUserCommand.ExecuteNonQuery();
			}
		}

		private LoginService _unitUnderTest;

		private readonly IList<User> _temporaryUsers = new List<User>();

		private User TemporaryUser
		{
			get
			{
				if (_unitUnderTest == null) return null;

				var name = RandomString;
				var pwd = RandomString;

				_unitUnderTest.Register(name, RandomString, pwd);
				var randomUser = _unitUnderTest.Login(name, pwd);

				_temporaryUsers.Add(randomUser);
				return randomUser;
			}
		}

		private static string RandomString => Guid.NewGuid().ToString();

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
		public void RegisterThrowsAnExceptionIfUserAlreadyExists()
		{
			//Arrange
			var existingUser = TemporaryUser;

			//Act & Assert
			Assert.Throws<UserAlreadyExistsException>(() => _unitUnderTest.Register(existingUser.Username, RandomString, RandomString));
			Assert.Throws<UserAlreadyExistsException>(() => _unitUnderTest.Register(RandomString, existingUser.Mail, RandomString));
		}

		[Test]
		public void LoginThrowsAnExceptionWithWrongCredentials()
		{
			//Act & Assert
			Assert.Throws<WrongCredentialsException>(() => _unitUnderTest.Login(RandomString, RandomString));
		}
	}
}