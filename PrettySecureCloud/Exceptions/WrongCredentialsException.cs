using System;
using System.ServiceModel;

namespace PrettySecureCloud.Exceptions
{
	[Serializable]
	public class WrongCredentialsException : FaultException
	{
		public WrongCredentialsException() : base("Anmeldedaten nicht gültig")
		{
			
		}
	}
}