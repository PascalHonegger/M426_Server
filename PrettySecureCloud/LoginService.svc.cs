using System.Collections.Generic;

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
						Type = ServiceType.DropBox
					},
					new CloudService
					{
						LoginToken = "asdfadsfjqerjküapsfdasdf",
						Name = "Geschäft OneDrive",
						Type = ServiceType.OneDrive
					}
				}
			};
		}

		public void Update(User newUserData)
		{
			//TODO
		}
	}
}
