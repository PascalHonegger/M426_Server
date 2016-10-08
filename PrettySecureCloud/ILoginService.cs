using System.Collections.Generic;
using System.ServiceModel;
using PrettySecureCloud.Model;

namespace PrettySecureCloud
{
	[ServiceContract]
	public interface ILoginService
	{
		/// <summary>
		///     Verifies that a username is unique
		/// </summary>
		/// <param name="username">Username to be verified</param>
		/// <returns>True if the username is unique</returns>
		[OperationContract]
		bool UsernameUnique(string username);

		/// <summary>
		///     Verifies that a Email is unique
		/// </summary>
		/// <param name="mail">Email to be verified</param>
		/// <returns>True if the Email is unique</returns>
		[OperationContract]
		bool EmailUnique(string mail);

		/// <summary>
		///     Registers a new user
		/// </summary>
		/// <param name="username">Name of the user</param>
		/// <param name="mail">Mail of the user</param>
		/// <param name="password">
		///     Cleartext password of the user, which will be user to encrypt the key and is only saved in a
		///     hashed form
		/// </param>
		[OperationContract]
		void Register(string username, string mail, string password);

		/// <summary>
		///     Logs a user in
		/// </summary>
		/// <param name="username">Name of the user</param>
		/// <param name="password">Cleartext password used to decrypt the key</param>
		/// <returns></returns>
		[OperationContract]
		User Login(string username, string password);

		/// <summary>
		///     Update the stored password
		/// </summary>
		/// <param name="userId">User</param>
		/// <param name="currentPassword">Current (old) cleartext password</param>
		/// <param name="newPassword">New password</param>
		[OperationContract]
		void ChangePassword(int userId, string currentPassword, string newPassword);

		/// <summary>
		///     Add a service to a specified user
		/// </summary>
		/// <param name="userId">User</param>
		/// <param name="typeId">Service typeId (dropbox...)</param>
		/// <param name="name">Custom name</param>
		/// <param name="loginToken">Login token used by the client</param>
		/// <returns>Id of the added service</returns>
		[OperationContract]
		int AddService(int userId, int typeId, string name, string loginToken);

		/// <summary>
		///     Change the properties of an existing service
		/// </summary>
		/// <param name="serviceId">Service</param>
		/// <param name="newName">Updated name</param>
		[OperationContract]
		void UpdateService(int serviceId, string newName);

		/// <summary>
		///     Remove a specified service from a user
		/// </summary>
		/// <param name="serviceId">Service</param>
		[OperationContract]
		void RemoveService(int serviceId);

		/// <summary>
		///     Load all services to the client can decide, which servicetypes he supports
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		IEnumerable<ServiceType> LoadAllServices();
	}
}