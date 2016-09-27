using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PrettySecureCloud
{
	public class LoginService : ILoginService
	{
		private readonly SqlCommand _insertUser = DbConn.Command;
		private readonly SqlCommand _loadUserFromName = DbConn.Command;
		private readonly SqlCommand _loadUserFromEmail = DbConn.Command;
		private readonly SqlCommand _updateUser = DbConn.Command;
		private readonly SqlCommand _getServiceTypes = DbConn.Command;

		public LoginService()
		{
			_insertUser.CommandText =
				"insert into tbl_User(username, email, password, public_Key, private_Key) values(@username, @email, @password, @public_Key, @private_Key)";
			_loadUserFromName.CommandText = "select * from tbl_User where username = @username";
			_loadUserFromEmail.CommandText = "select * from tbl_User where email = @email";
			_updateUser.CommandText =
				"update tbl_User set username=@username, email=@email, public_Key=@public_Key, private_Key=@private_Key where id_User = @iduser";
			_getServiceTypes.CommandText = "select * from tbl_Service";

			_insertUser.Parameters.Add("@username", SqlDbType.VarChar);
			_insertUser.Parameters.Add("@email", SqlDbType.VarChar);
			_insertUser.Parameters.Add("@password", SqlDbType.VarChar);
			_insertUser.Parameters.Add("@public_Key", SqlDbType.VarChar);
			_insertUser.Parameters.Add("@private_Key", SqlDbType.VarChar);

			_loadUserFromName.Parameters.Add("@username", SqlDbType.VarChar);

			_loadUserFromEmail.Parameters.Add("@email", SqlDbType.VarChar);

			_updateUser.Parameters.Add("@username", SqlDbType.VarChar);
			_updateUser.Parameters.Add("@email", SqlDbType.VarChar);
			_updateUser.Parameters.Add("@iduser", SqlDbType.Int);
			_updateUser.Parameters.Add("@public_Key", SqlDbType.VarChar);
			_updateUser.Parameters.Add("@private_Key", SqlDbType.VarChar);
		}

		public bool UsernameUnique(string username)
		{
			_loadUserFromName.Parameters["@username"].Value = username;

			var reader = _loadUserFromName.ExecuteReader();
			var unique = !reader.HasRows;
			reader.Close();

			return unique;
		}

		public bool EmailUnique(string mail)
		{
			_loadUserFromEmail.Parameters["@email"].Value = mail;

			var reader = _loadUserFromEmail.ExecuteReader();
			var unique = !reader.HasRows;
			reader.Close();

			return unique;
		}

		public void Register(string username, string mail, string password)
		{
			if (!EmailUnique(mail)) throw new ArgumentException("The given E-Mail Address is already registered");
			if (!UsernameUnique(username)) throw new ArgumentException("The given Username is already in use");


			_insertUser.Parameters["@username"].Value = username;
			_insertUser.Parameters["@email"].Value = mail;
			_insertUser.Parameters["@password"].Value = password;
			_insertUser.Parameters["@public_Key"].Value = "TestKey";
			_insertUser.Parameters["@private_Key"].Value = "TestKey";

			_insertUser.ExecuteNonQuery();
		}

		public User Login(string username, string password)
		{
			_loadUserFromName.Parameters["@username"].Value = username;

			var reader = _loadUserFromName.ExecuteReader();

			if (!reader.HasRows) throw new ArgumentException("Passwor or Username is wrong");

			reader.Read();
			var pw = (string) reader["password"];

			if (!Equals(pw, password)) throw new ArgumentException("Password or Username is wrong");
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
			_updateUser.Parameters["@username"].Value = newUserData.Username;
			_updateUser.Parameters["@email"].Value = newUserData.Mail;
			_updateUser.Parameters["@private_Key"].Value = newUserData.PrivateKey;
			_updateUser.Parameters["@public_Key"].Value = newUserData.PublicKey;
			_updateUser.Parameters["@iduser"].Value = newUserData.Id;

			_updateUser.ExecuteNonQuery();
		}

		public IEnumerable<ServiceType> LoadAllServices()
		{
			var reader = _getServiceTypes.ExecuteReader();

			var serviceList = new List<ServiceType>();

			//TODO Works?
			foreach (SqlDataReader value in reader)
			{
				serviceList.Add(new ServiceType()
				{
					Key = (string) value["appkey"],
					Name = (string) value["name"],
					Secret = (string) value["appsecret"]
				});
			}

			reader.Close();

			return serviceList;
		}
	}
}