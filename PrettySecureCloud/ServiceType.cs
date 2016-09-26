using System.Runtime.Serialization;

namespace PrettySecureCloud
{
	[DataContract]
	public class ServiceType
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Key { get; set; }

		[DataMember]
		public string Secret { get; set; }
	}
}