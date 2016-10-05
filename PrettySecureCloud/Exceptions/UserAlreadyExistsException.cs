using System;
using System.ServiceModel;

namespace PrettySecureCloud.Exceptions
{
	[Serializable]
	public class UserAlreadyExistsException : FaultException
	{
		public UserAlreadyExistsException(string field)
			: base($"Es ist bereits ein Benutzer mit diesen Daten ({field}) registriert")
		{
		}
	}
}