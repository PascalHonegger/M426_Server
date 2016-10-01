using System;
using System.Collections.Generic;
using System.Data;
using PrettySecureCloud.Exceptions;
using PrettySecureCloud.Model;
using PrettySecureCloud.Security;

namespace PrettySecureCloud
{
	public class LoginService : ILoginService
	{
		private readonly IPasswordHasher _passwordHasher;

		private readonly IDbCommand _insertUser;
		private readonly IDbCommand _loadUserFromName;
		private readonly IDbCommand _loadUserFromEmail;
		private readonly IDbCommand _updateUser;
		private readonly IDbCommand _getServiceTypes;

		public LoginService()
		{
			_passwordHasher = new BCryptHasher();
			IDatabaseConnection databaseConnection = new MsSqlConnection();

			_insertUser = databaseConnection.Command;
			_loadUserFromName = databaseConnection.Command;
			_loadUserFromEmail = databaseConnection.Command;
			_updateUser = databaseConnection.Command;
			_getServiceTypes = databaseConnection.Command;

			//Insert user
			_insertUser.CommandText =
				"insert into tbl_User(username, email, password, public_Key, private_Key) values(@username, @email, @password, @public_Key, @private_Key)";

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
			insertUserParam.ParameterName = "@public_Key";
			insertUserParam.DbType = DbType.String;
			_insertUser.Parameters.Add(insertUserParam);

			insertUserParam = _insertUser.CreateParameter();
			insertUserParam.ParameterName = "@private_Key";
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
				"update tbl_User set username=@username, email=@email, public_Key=@public_Key, private_Key=@private_Key where id_User = @iduser";
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
			updateUserParam.ParameterName = "@public_Key";
			updateUserParam.DbType = DbType.String;
			_updateUser.Parameters.Add(updateUserParam);

			updateUserParam = _updateUser.CreateParameter();
			updateUserParam.ParameterName = "@private_Key";
			updateUserParam.DbType = DbType.String;
			_updateUser.Parameters.Add(updateUserParam);
		}

		public bool UsernameUnique(string username)
		{
			((IDbDataParameter)_loadUserFromName.Parameters["@username"]).Value = username;

			var reader = _loadUserFromName.ExecuteReader();
			var unique = !reader.Read();
			reader.Close();

			return unique;
		}

		public bool EmailUnique(string mail)
		{
			((IDbDataParameter)_loadUserFromEmail.Parameters["@email"]).Value = mail;

			var reader = _loadUserFromEmail.ExecuteReader();
			var unique = !reader.Read();
			reader.Close();

			return unique;
		}

		public void Register(string username, string mail, string password)
		{
			if (!EmailUnique(mail)) throw new UserAlreadyExistsException("E-Mail");
			if (!UsernameUnique(username)) throw new UserAlreadyExistsException("Benutzername");


			((IDbDataParameter)_insertUser.Parameters["@username"]).Value = username;
			((IDbDataParameter)_insertUser.Parameters["@email"]).Value = mail;
			((IDbDataParameter)_insertUser.Parameters["@password"]).Value = _passwordHasher.CalculateHash(password);
			((IDbDataParameter)_insertUser.Parameters["@public_Key"]).Value = "TestKey";
			((IDbDataParameter)_insertUser.Parameters["@private_Key"]).Value = "TestKey";

			_insertUser.ExecuteNonQuery();
		}

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
			
			//Validate Password
			if (!_passwordHasher.Validate(password, hash)) wrongLogin();

			var user = new User
			{
				Id = (int) reader["id_User"],
				Username = (string) reader["username"],
				Mail = (string) reader["email"],
				PrivateKey = (string) reader["private_Key"],
				PublicKey = (string) reader["public_Key"],
				Services = new List<CloudService>()
			};

			reader.Close();

			return user;
		}

		public void Update(User newUserData) //TODO: normal params
		{
			((IDbDataParameter)_updateUser.Parameters["@username"]).Value = newUserData.Username;
			((IDbDataParameter)_updateUser.Parameters["@email"]).Value = newUserData.Mail;
			((IDbDataParameter)_updateUser.Parameters["@private_Key"]).Value = newUserData.PrivateKey;
			((IDbDataParameter)_updateUser.Parameters["@public_Key"]).Value = newUserData.PublicKey;
			((IDbDataParameter)_updateUser.Parameters["@iduser"]).Value = newUserData.Id;

			_updateUser.ExecuteNonQuery();
		}

		public IEnumerable<ServiceType> LoadAllServices()
		{
			var reader = _getServiceTypes.ExecuteReader();

			var serviceList = new List<ServiceType>();

			while (reader.Read())
			{
				serviceList.Add(new ServiceType
				{
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