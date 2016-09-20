using System.ServiceModel;

namespace PrettySecureCloud
{
	[ServiceContract]
	public interface ILoginService
	{
		[OperationContract]
		bool UsernameUnique(string username);

		[OperationContract]
		void Register(string username, string password);

		[OperationContract]
		User Login(string username, string password);

		[OperationContract]
		bool Update(User newUserData);
	}
}
