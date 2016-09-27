using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PrettySecureCloud
{
	public class LoginService : ILoginService
	{
		private static readonly SqlCommand InsertUser = DbConn.Command;
		private static readonly SqlCommand LoginUser = DbConn.Command;
		private static readonly SqlCommand CheckEMail = DbConn.Command;
		private static readonly SqlCommand UpdateUser = DbConn.Command;
		private static readonly SqlCommand GetServiceTypes = DbConn.Command;



		public LoginService()
		{
			InsertUser.CommandText =
				"insert into tbl_User(username, email, password, public_Key, privat_Key) values(@username, @email, @password, @publi_Key, @privat_Key)";
			LoginUser.CommandText = "select * from tbl_User where username = @username";
			CheckEMail.CommandText = "select * from tbl_User where email = @email";
			UpdateUser.CommandText = "update tbl_User set username=@username, email=@email, public_Key=@public_Key, private_Key=@private_Key where id_User = @iduser";
			GetServiceTypes.CommandText = "select * from tbl_Service";

			InsertUser.Parameters.Add("@username", SqlDbType.NText);
			InsertUser.Parameters.Add("@email", SqlDbType.NText);
			InsertUser.Parameters.Add("@password", SqlDbType.NText);
			InsertUser.Parameters.Add("@public_Key", SqlDbType.NText);
			InsertUser.Parameters.Add("@private_Key", SqlDbType.NText);

			LoginUser.Parameters.Add("@username", SqlDbType.NText);

			CheckEMail.Parameters.Add("@email", SqlDbType.NText);

			UpdateUser.Parameters.Add("@username", SqlDbType.NText);
			UpdateUser.Parameters.Add("@email", SqlDbType.NText);
			UpdateUser.Parameters.Add("@iduser", SqlDbType.Int);
			UpdateUser.Parameters.Add("@public_Key", SqlDbType.NText);
			UpdateUser.Parameters.Add("@private_Key", SqlDbType.NText);
		}

		public bool UsernameUnique(string username)
		{
			LoginUser.Parameters["@username"].Value = username;

			LoginUser.Prepare();

			SqlDataReader reader = LoginUser.ExecuteReader();
			if (reader.HasRows)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool EmailUnique(string mail)
		{
			CheckEMail.Parameters["@email"].Value = mail;

			CheckEMail.Prepare();

			SqlDataReader reader = CheckEMail.ExecuteReader();
			if (reader.HasRows)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public void Register(string username, string mail, string password)
		{
			if (!EmailUnique(mail)) throw new ArgumentException("The given E-Mail Address is already registered");
			if(!UsernameUnique(username)) throw new ArgumentException("The given Username is already in use");


			InsertUser.Parameters["@username"].Value = username;
			InsertUser.Parameters["@email"].Value = mail;
			InsertUser.Parameters["@password"].Value = password;
			InsertUser.Parameters["@public_Key"].Value = "TestKey";
			InsertUser.Parameters["@public_Key"].Value = "TestKey";

			InsertUser.Prepare();

			InsertUser.ExecuteNonQuery();
		}

		public User Login(string username, string password)
		{
			LoginUser.Parameters["@username"].Value = username;

			LoginUser.Prepare();

			SqlDataReader reader = LoginUser.ExecuteReader();

			if (reader.HasRows)
			{
				var pw = (string)reader["password"];

				if (pw == password) // TODO: hash pw from client
				{
					var user = new User
					{
						Id = (int)reader["id_User"],
						Username = (string)reader["username"],
						Mail = (string)reader["email"],
						PrivateKey = (string)reader["private_Key"],
						PublicKey = (string)reader["public_Key"]
					};

					return user;
				}
				else
				{
					throw new ArgumentException("Password or Username is wrong");
				}
			}
			else
			{
				throw new ArgumentException("Passwor or Username is wrong");
			}
		}

		public void Update(User newUserData) //TODO: normal params
		{
			UpdateUser.Parameters["@username"].Value = newUserData.Username;
			UpdateUser.Parameters["@email"].Value = newUserData.Mail;
			UpdateUser.Parameters["@private_Key"].Value = newUserData.PrivateKey;
			UpdateUser.Parameters["@public_Key"].Value = newUserData.PublicKey;
			UpdateUser.Parameters["@iduser"].Value = newUserData.Id;

			UpdateUser.Prepare();

			UpdateUser.ExecuteNonQuery();
		}

		public IEnumerable<ServiceType> LoadAllServices()
		{
			GetServiceTypes.Prepare();

			SqlDataReader reader = GetServiceTypes.ExecuteReader();

			var ServiceList = new List<ServiceType>();
			
			foreach(SqlDataReader value in reader)
			{
				ServiceList.Add(new ServiceType() { Key = (string)value["appkey"], Name = (string)value["name"], Secret = (string)value["appsecret"] });
			}

			return ServiceList;
	}
}