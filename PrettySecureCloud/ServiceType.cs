using System.Runtime.Serialization;

namespace PrettySecureCloud
{
	[DataContract]
	public enum ServiceType
	{
		DropBox,
		OneDrive,
		AppleICloud,
		GoogleDrive
	}
}