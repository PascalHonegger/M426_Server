using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PrettySecureCloud.Exceptions;
using PrettySecureCloud.Model;
using PrettySecureCloud.Security;

namespace PrettySecureCloud
{
	public class LoginService : ILoginService
	{
		private readonly IDbCommand _changePassword;
		private readonly IDbCommand _getServiceByUser;
		private readonly IDbCommand _getServiceTypes;

		private readonly IDbCommand _insertUser;
		private readonly IKeyEncryptorDecryptor _keyEncryptorDecryptor;
		private readonly IDbCommand _loadUserFromEmail;
		private readonly IDbCommand _loadUserFromId;
		private readonly IDbCommand _loadUserFromName;
		private readonly IDbCommand _addService;
		private readonly IDbCommand _removeService;
		private readonly IDbCommand _updateService;
		private readonly IPasswordHasher _passwordHasher;

		public LoginService()
		{
			_passwordHasher = new BCryptHasher();
			_keyEncryptorDecryptor = new RijndaelKeyEncryptorDecryptor();
			IDatabaseConnection databaseConnection = new MsSqlConnection();

			_insertUser = databaseConnection.Command;
			_loadUserFromName = databaseConnection.Command;
			_loadUserFromEmail = databaseConnection.Command;
			_loadUserFromId = databaseConnection.Command;
			_changePassword = databaseConnection.Command;
			_getServiceTypes = databaseConnection.Command;
			_getServiceByUser = databaseConnection.Command;
			_addService = databaseConnection.Command;
			_removeService = databaseConnection.Command;
			_updateService = databaseConnection.Command;

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
			insertUserParam.DbType = DbType.Binary;
			_insertUser.Parameters.Add(insertUserParam);

			//Load user from name
			_loadUserFromName.CommandText = "select * from tbl_User where username = @username COLLATE Latin1_General_CS_AS";

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

			//Load user from ID
			_loadUserFromId.CommandText = "select * from tbl_User where id_User = @iduser";

			var loadUserFromIdParam = _loadUserFromId.CreateParameter();
			loadUserFromIdParam.ParameterName = "@iduser";
			loadUserFromIdParam.DbType = DbType.Int32;
			_loadUserFromId.Parameters.Add(loadUserFromIdParam);

			//Change password
			_changePassword.CommandText =
				"update tbl_User set password=@password, encryptionkey=@encryptionkey where id_User = @iduser";

			var changePasswordParam = _changePassword.CreateParameter();
			changePasswordParam.ParameterName = "@password";
			changePasswordParam.DbType = DbType.String;
			_changePassword.Parameters.Add(changePasswordParam);

			changePasswordParam = _changePassword.CreateParameter();
			changePasswordParam.ParameterName = "@iduser";
			changePasswordParam.DbType = DbType.Int32;
			_changePassword.Parameters.Add(changePasswordParam);

			changePasswordParam = _changePassword.CreateParameter();
			changePasswordParam.ParameterName = "@encryptionkey";
			changePasswordParam.DbType = DbType.Binary;
			_changePassword.Parameters.Add(changePasswordParam);

			//Load services from a specified user
			_getServiceByUser.CommandText = "select id_User_Service, token, tbl_User_Service.name as customname, id_Service, appkey, appsecret, tbl_Service.name from tbl_User_Service join tbl_Service on id_Service=fk_Service where fk_User=@iduser";
			var getServiceByUserParam = _getServiceByUser.CreateParameter();
			getServiceByUserParam.ParameterName = "@iduser";
			getServiceByUserParam.DbType = DbType.Int32;
			_getServiceByUser.Parameters.Add(getServiceByUserParam);

			//Load all servicetypes
			_getServiceTypes.CommandText = "select * from tbl_Service";

			//Add service to a user
			_addService.CommandText = "insert into tbl_User_Service(name, token, fk_User, fk_Service) values (@name, @token, @iduser, @idservice); select cast(scope_identity() as int)";

			var addServiceParam = _addService.CreateParameter();
			addServiceParam.ParameterName = "@name";
			addServiceParam.DbType = DbType.String;
			_addService.Parameters.Add(addServiceParam);

			addServiceParam = _addService.CreateParameter();
			addServiceParam.ParameterName = "@token";
			addServiceParam.DbType = DbType.String;
			_addService.Parameters.Add(addServiceParam);

			addServiceParam = _addService.CreateParameter();
			addServiceParam.ParameterName = "@iduser";
			addServiceParam.DbType = DbType.Int32;
			_addService.Parameters.Add(addServiceParam);

			addServiceParam = _addService.CreateParameter();
			addServiceParam.ParameterName = "@idservice";
			addServiceParam.DbType = DbType.Int32;
			_addService.Parameters.Add(addServiceParam);

			//Remove service from a user
			_removeService.CommandText = "delete from tbl_User_Service where id_User_Service=@iduserservice";

			var removeServiceParam = _removeService.CreateParameter();
			removeServiceParam.ParameterName = "@iduserservice";
			removeServiceParam.DbType = DbType.Int32;
			_removeService.Parameters.Add(removeServiceParam);

			//Update service from a user
			_updateService.CommandText = "update tbl_User_Service set name=@name where id_User_Service=@iduserservice";

			var updateServiceParam = _updateService.CreateParameter();
			updateServiceParam.ParameterName = "@name";
			updateServiceParam.DbType = DbType.String;
			_updateService.Parameters.Add(updateServiceParam);

			updateServiceParam = _updateService.CreateParameter();
			updateServiceParam.ParameterName = "@iduserservice";
			updateServiceParam.DbType = DbType.Int32;
			_updateService.Parameters.Add(updateServiceParam);
		}

		/// <summary>
		///     Verifies that a username is unique
		/// </summary>
		/// <param name="username">Username to be verified</param>
		/// <returns>True if the username is unique</returns>
		public bool UsernameUnique(string username)
		{
			((IDbDataParameter) _loadUserFromName.Parameters["@username"]).Value = username;

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
			((IDbDataParameter) _loadUserFromEmail.Parameters["@email"]).Value = mail;

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


			((IDbDataParameter) _insertUser.Parameters["@username"]).Value = username;
			((IDbDataParameter) _insertUser.Parameters["@email"]).Value = mail;
			((IDbDataParameter) _insertUser.Parameters["@password"]).Value = _passwordHasher.CalculateHash(password);
			((IDbDataParameter) _insertUser.Parameters["@encryptionkey"]).Value =
				_keyEncryptorDecryptor.Encrypt(_keyEncryptorDecryptor.GenerateRandomKey(), password);

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
			((IDbDataParameter) _loadUserFromName.Parameters["@username"]).Value = username;

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
				EncryptionKey = _keyEncryptorDecryptor.Decrypt((byte[]) reader["encryptionkey"], password)
			};

			reader.Close();

			user.Services = LoadServices(user.Id);

			return user;
		}

		/// <summary>
		///     Update the stored password
		/// </summary>
		/// <param name="userId">User</param>
		/// <param name="currentPassword">Current (old) cleartext password</param>
		/// <param name="newPassword">New password</param>
		public void ChangePassword(int userId, string currentPassword, string newPassword)
		{
			((IDataParameter) _loadUserFromId.Parameters["@iduser"]).Value = userId;
			var reader = _loadUserFromId.ExecuteReader();

			Action changeFailed = () =>
			{
				reader.Close();
				throw new WrongCredentialsException();
			};

			if (!reader.Read()) changeFailed();

			//Wrong current password
			if (!_passwordHasher.Verify(currentPassword, (string) reader["password"])) changeFailed();

			var currentKey = (byte[]) reader["encryptionkey"];

			reader.Close();

			var unencrypted = _keyEncryptorDecryptor.Decrypt(currentKey, currentPassword);

			((IDataParameter) _changePassword.Parameters["@password"]).Value = _passwordHasher.CalculateHash(newPassword);
			((IDataParameter) _changePassword.Parameters["@iduser"]).Value = userId;
			((IDataParameter) _changePassword.Parameters["@encryptionkey"]).Value = _keyEncryptorDecryptor.Encrypt(unencrypted,
				newPassword);

			_changePassword.ExecuteNonQuery();
		}

		/// <summary>
		///     Add a service to a specified user
		/// </summary>
		/// <param name="userId">User</param>
		/// <param name="typeId">Service typeId (dropbox...)</param>
		/// <param name="name">Custom name</param>
		/// <param name="loginToken">Login token used by the client</param>
		/// <returns></returns>
		public int AddService(int userId, int typeId, string name, string loginToken)
		{
			((IDbDataParameter)_addService.Parameters["@idservice"]).Value = typeId;
			((IDbDataParameter)_addService.Parameters["@iduser"]).Value = userId;
			((IDbDataParameter)_addService.Parameters["@name"]).Value = name;
			((IDbDataParameter)_addService.Parameters["@token"]).Value = loginToken;

			return (int)_addService.ExecuteScalar();
		}

		/// <summary>
		///     Change the properties of an existing service
		/// </summary>
		/// <param name="serviceId">Service</param>
		/// <param name="newName">Updated name</param>
		public void UpdateService(int serviceId, string newName)
		{
			((IDbDataParameter)_updateService.Parameters["@iduserservice"]).Value = serviceId;
			((IDbDataParameter)_updateService.Parameters["@name"]).Value = newName;

			_updateService.ExecuteNonQuery();
		}

		/// <summary>
		///     Remove a specified service from a user
		/// </summary>
		/// <param name="serviceId">Service</param>
		public void RemoveService(int serviceId)
		{
			((IDbDataParameter) _removeService.Parameters["@iduserservice"]).Value = serviceId;

			_removeService.ExecuteNonQuery();
		}

		/// <summary>
		///     Load all services to the client who can decide, which servicetypes he supports
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ServiceType> LoadAllServices()
		{
			var reader = _getServiceTypes.ExecuteReader();

			var serviceList = new List<ServiceType>();

			while (reader.Read())
				serviceList.Add(new ServiceType
				{
					Id = (int) reader["id_Service"],
					Key = (string) reader["appkey"],
					Name = (string) reader["name"],
					Secret = (string) reader["appsecret"]
				});

			reader.Close();

			return serviceList;
		}

		private IEnumerable<CloudService> LoadServices(int userId)
		{
			((IDbDataParameter) _getServiceByUser.Parameters["@iduser"]).Value = userId;

			var reader = _getServiceByUser.ExecuteReader();

			var result = new List<CloudService>();

			while (reader.Read())
				result.Add(
					new CloudService
					{
						Id = (int) reader["id_User_Service"],
						Name = (string) reader["customname"],
						LoginToken = (string) reader["token"],
						Type = new ServiceType
						{
							Id = (int)reader["id_Service"],
							Key = (string)reader["appkey"],
							Name = (string)reader["name"],
							Secret = (string)reader["appsecret"]
						}
					});

			reader.Close();

			return result;
		}
	}
}