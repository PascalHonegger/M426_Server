using System.Runtime.Serialization;

namespace PrettySecureCloud
{
	[DataContract]
	public enum ServiceType
	{
		[EnumMember]
		DropBox,
		[EnumMember]
		OneDrive,
		[EnumMember]
		AppleICloud,
		[EnumMember]
		GoogleDrive
	}
}