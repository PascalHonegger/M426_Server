using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using PrettySecureCloud.Exceptions;
using PrettySecureCloud.Model;
using PrettySecureCloud.Security;

namespace PrettySecureCloud
{
	public class LoginService : ILoginService
	{
		private readonly IPasswordHasher _passwordHasher;
		private readonly IKeyEncryptorDecryptor _keyEncryptorDecryptor;

		private readonly IDbCommand _insertUser;
		private readonly IDbCommand _loadUserFromName;
		private readonly IDbCommand _loadUserFromEmail;
		//TODO Use in update!
		private readonly IDbCommand _updateUser;
		private readonly IDbCommand _getServiceTypes;
		private readonly IDbCommand _getServiceByUser;

		public LoginService()
		{
			_passwordHasher = new BCryptHasher();
			_keyEncryptorDecryptor = new RijndaelKeyEncryptorDecryptor();
			IDatabaseConnection databaseConnection = new MsSqlConnection();

			_insertUser = databaseConnection.Command;
			_loadUserFromName = databaseConnection.Command;
			_loadUserFromEmail = databaseConnection.Command;
			_updateUser = databaseConnection.Command;
			_getServiceTypes = databaseConnection.Command;
			_getServiceByUser = databaseConnection.Command;

			//Insert user
			_insertUser.CommandText =
				"insert into tbl_User(username, email, password, encryptionkey) values(@username, @email, @password, @encryptionkey)";

			var insertUserParam = _insertUser.CreateParameter();
			insertUserParam.ParameterName = "@username";
			insertUserParam.DbType = DbType.String;
			_insertUser.Parameters.Add(insertUserParam);

			insertUserParam = _insertUser.CreateParameter();
			insertUserParam.ParameterName = "@email";
			insertUserParam.DbType = DbType.String;
			_insertUser.Parameters.Add(insertUserParam);

			insertUserParam = _insertUser.CreateParameter();
			insertUserParam.ParameterName = "@password";
			insertUserParam.DbType = DbType.String;
			_insertUser.Parameters.Add(insertUserParam);

			insertUserParam = _insertUser.CreateParameter();
			insertUserParam.ParameterName = "@encryptionkey";
			insertUserParam.DbType = DbType.String;
			_insertUser.Parameters.Add(insertUserParam);

			//Load user from name
			_loadUserFromName.CommandText = "select * from tbl_User where username = @username";

			var loadUserFromNameParam = _loadUserFromName.CreateParameter();
			loadUserFromNameParam.ParameterName = "@username";
			loadUserFromNameParam.DbType = DbType.String;
			_loadUserFromName.Parameters.Add(loadUserFromNameParam);

			//Load user from email
			_loadUserFromEmail.CommandText = "select * from tbl_User where email = @email";

			var loadUserFromEmailParam = _loadUserFromEmail.CreateParameter();
			loadUserFromEmailParam.ParameterName = "@email";
			loadUserFromEmailParam.DbType = DbType.String;
			_loadUserFromEmail.Parameters.Add(loadUserFromEmailParam);

			_updateUser.CommandText =
				"update tbl_User set username=@username, email=@email, encryptionkey=@encryptionkey where id_User = @iduser";
			_getServiceTypes.CommandText = "select * from tbl_Service";

			var updateUserParam = _updateUser.CreateParameter();
			updateUserParam.ParameterName = "@username";
			updateUserParam.DbType = DbType.String;
			_updateUser.Parameters.Add(updateUserParam);

			updateUserParam = _updateUser.CreateParameter();
			updateUserParam.ParameterName = "@email";
			updateUserParam.DbType = DbType.String;
			_updateUser.Parameters.Add(updateUserParam);

			updateUserParam = _updateUser.CreateParameter();
			updateUserParam.ParameterName = "@iduser";
			updateUserParam.DbType = DbType.Int32;
			_updateUser.Parameters.Add(updateUserParam);

			updateUserParam = _updateUser.CreateParameter();
			updateUserParam.ParameterName = "@encryptionkey";
			updateUserParam.DbType = DbType.String;
			_updateUser.Parameters.Add(updateUserParam);

			_getServiceByUser.CommandText = "select * from tbl_User_Service where fk_User=@iduser";
			var getServiceByUserParam = _getServiceByUser.CreateParameter();
			getServiceByUserParam.ParameterName = "@iduser";
			getServiceByUserParam.DbType = DbType.Int32;
			_getServiceByUser.Parameters.Add(getServiceByUserParam);
		}

		/// <summary>
		///     Verifies that a username is unique
		/// </summary>
		/// <param name="username">Username to be verified</param>
		/// <returns>True if the username is unique</returns>
		public bool UsernameUnique(string username)
		{
			((IDbDataParameter)_loadUserFromName.Parameters["@username"]).Value = username;

			var reader = _loadUserFromName.ExecuteReader();
			var unique = !reader.Read();
			reader.Close();

			return unique;
		}

		/// <summary>
		///     Verifies that a Email is unique
		/// </summary>
		/// <param name="mail">Email to be verified</param>
		/// <returns>True if the Email is unique</returns>
		public bool EmailUnique(string mail)
		{
			((IDbDataParameter)_loadUserFromEmail.Parameters["@email"]).Value = mail;

			var reader = _loadUserFromEmail.ExecuteReader();
			var unique = !reader.Read();
			reader.Close();

			return unique;
		}

		/// <summary>
		///     Registers a new user
		/// </summary>
		/// <param name="username">Name of the user</param>
		/// <param name="mail">Mail of the user</param>
		/// <param name="password">
		///     Cleartext password of the user, which will be user to encrypt the key and is only saved in a
		///     hashed form
		/// </param>
		public void Register(string username, string mail, string password)
		{
			if (!EmailUnique(mail)) throw new UserAlreadyExistsException("E-Mail");
			if (!UsernameUnique(username)) throw new UserAlreadyExistsException("Benutzername");


			((IDbDataParameter)_insertUser.Parameters["@username"]).Value = username;
			((IDbDataParameter)_insertUser.Parameters["@email"]).Value = mail;
			((IDbDataParameter)_insertUser.Parameters["@password"]).Value = _passwordHasher.CalculateHash(password);
			//TODO Generate key
			((IDbDataParameter)_insertUser.Parameters["@encryptionkey"]).Value = _keyEncryptorDecryptor.Encrypt("TestKey", password);

			_insertUser.ExecuteNonQuery();
		}

		/// <summary>
		///     Logs a user in
		/// </summary>
		/// <param name="username">Name of the user</param>
		/// <param name="password">Cleartext password used to decrypt the key</param>
		/// <returns></returns>
		public User Login(string username, string password)
		{
			((IDbDataParameter)_loadUserFromName.Parameters["@username"]).Value = username;

			var reader = _loadUserFromName.ExecuteReader();

			//Reader needs to be closed in case there is an exception
			Action wrongLogin = () =>
			{
				reader.Close();
				throw new WrongCredentialsException();
			};

			//Validate user found
			if (!reader.Read()) wrongLogin();

			var hash = (string) reader["password"];
			
			//Verify Password
			if (!_passwordHasher.Verify(password, hash)) wrongLogin();

			var user = new User
			{
				Id = (int) reader["id_User"],
				Username = (string) reader["username"],
				Mail = (string) reader["email"],
				EncryptionKey = Encoding.Default.GetBytes(_keyEncryptorDecryptor.Decrypt((string)reader["encryptionkey"], password))
			};
			user.Services = LoadServices(user.Id);

			reader.Close();

			return user;
		}

		/// <summary>
		///     Update the stored password
		/// </summary>
		/// <param name="currentPassword">Current (old) cleartext password</param>
		/// <param name="newPassword">New password</param>
		public void ChangePassword(string currentPassword, string newPassword)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Add a service to a specified user
		/// </summary>
		/// <param name="userId">User</param>
		/// <param name="type">Service type (dropbox...)</param>
		/// <param name="name">Custom name</param>
		/// <param name="loginToken">Login token used by the client</param>
		/// <returns></returns>
		public CloudService AddService(int userId, ServiceType type, string name, string loginToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Change the properties of an existing service
		/// </summary>
		/// <param name="userId">User</param>
		/// <param name="serviceId">Service</param>
		/// <param name="newName">Updated name (or current name)</param>
		/// <param name="newLoginToken">Updated login toke (or current token)</param>
		public void UpdateService(int userId, int serviceId, string newName, string newLoginToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Remove a specified service from a user
		/// </summary>
		/// <param name="userId">User</param>
		/// <param name="serviceId">Service</param>
		public void RemoveService(int userId, int serviceId)
		{
			throw new NotImplementedException();
		}

		private IEnumerable<CloudService> LoadServices(int userId)
		{
			((IDbDataParameter)_getServiceByUser.Parameters["@iduser"]).Value = userId;
			var reader = _getServiceByUser.ExecuteReader();

			IList<ServiceType> allServices = LoadAllServices().ToList();

			while (reader.Read())
			{
				yield return new CloudService
				{
					Id = (int) reader["id_User_Service"],
					Name = (string) reader["name"],
					LoginToken = (string) reader["token"],
					Type = allServices.First(s => Equals(s.Id, (int) reader["fk_Service"]))
				};
			}
		}

		/// <summary>
		///     Load all services to the client can decide, which servicetypes he supports
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ServiceType> LoadAllServices()
		{
			var reader = _getServiceTypes.ExecuteReader();

			var serviceList = new List<ServiceType>();

			while (reader.Read())
			{
				serviceList.Add(new ServiceType
				{
					Id = (int)reader["id_Service"],
					Key = (string)reader["appkey"],
					Name = (string)reader["name"],
					Secret = (string)reader["appsecret"]
				});
			}

			reader.Close();

			return serviceList;
		}
	}
}