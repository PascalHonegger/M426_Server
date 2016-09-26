using System.Collections.Generic;
using System.Linq;

namespace PrettySecureCloud
{
	public class LoginService : ILoginService
	{
		public bool UsernameUnique(string username)
		{
			return true;
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
	}
}