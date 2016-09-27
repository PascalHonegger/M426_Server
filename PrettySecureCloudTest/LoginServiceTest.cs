using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using PrettySecureCloud;

namespace PrettySecureCloudTest
{
	[TestFixture]
	[Category("IntegrationTest")]
	public class LoginServiceTest
	{
		private readonly SqlCommand _deleteUserCommand = DbConn.Command;
		private readonly SqlCommand _deleteServicesFromUserCommand = DbConn.Command;

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
				}
				_deleteUserCommand.CommandText = $"DELETE FROM tbl_User WHERE id_User={tempUser.Id}";
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

		private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		private string RandomString => new string(Enumerable.Repeat(Chars, 20).Select(s => s[_random.Next(s.Length)]).ToArray());

		private readonly Random _random = new Random();

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
	}
}