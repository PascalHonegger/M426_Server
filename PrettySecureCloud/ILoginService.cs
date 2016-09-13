using System.ServiceModel;

namespace PrettySecureCloud
{
	[ServiceContract]
	public interface ILoginService
	{
		[OperationContract]
		bool MailValid(string mail);

		[OperationContract]
		void Register(string mail, string password);

		[OperationContract]
		void Login(string mail, string password);
	}
}
