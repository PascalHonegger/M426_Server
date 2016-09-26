using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PrettySecureCloud
{
	public class LoginService : ILoginService
	{

		private static SqlCommand InsertUser = DBConn.Command;
		private static SqlCommand LoginUser = DBConn.Command;

		public bool UsernameUnique(string username)
		{
			//TODO
			return true;
		}

		public bool EmailUnique(string username)
		{
			return false;
		}

		public void Register(string username, string mail, string password)
		{
			//TODO
		}

		public User Login(string username, string password)
		{
			var serviceTypeDropbox = LoadAllServices().First();

			return new User
			{
				Id = 42,
				Username = username,
				Services = new List<CloudService>
				{
					new CloudService
					{
						LoginToken = "QWERASDFQWERJAKVJASD",
						Name = "Privat DropBox",
						Type = serviceTypeDropbox
					},
					new CloudService
					{
						LoginToken = "asdfadsfjqerjküapsfdasdf",
						Name = "Geschäft DropBox",
						Type = serviceTypeDropbox
					}
				}
			};
		}

		public void Update(User newUserData)
		{
			//TODO
		}

		public IEnumerable<ServiceType> LoadAllServices()
		{
			return new List<ServiceType>
			{
				new ServiceType
				{
					Key = "XASDFQER",
					Name = "DropBox",
					Secret = "1324185678456245134134123"
				}
			};
		}

		public LoginService()
		{
			InsertUser.CommandText = "insert into tbl_user(username, e-mail, password, publickey, privatkey) values(@username, @e-mail, @password, @publikey, @privatkey)";
			LoginUser.CommandText = "select * from tbl_user where username = @username";

			InsertUser.Parameters.Add("@username", SqlDbType.NText);
			InsertUser.Parameters.Add("@e-mail", SqlDbType.NText);
			InsertUser.Parameters.Add("@password", SqlDbType.NText);
			InsertUser.Parameters.Add("@publickey", SqlDbType.NText);
			InsertUser.Parameters.Add("@privatekey", SqlDbType.NText);

			LoginUser.Parameters.Add("@username", SqlDbType.NText);
		}
	}
}