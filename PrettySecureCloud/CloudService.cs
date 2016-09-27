using System.Runtime.Serialization;

namespace PrettySecureCloud
{
	[DataContract]
	public class CloudService
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string LoginToken { get; set; }

		[DataMember]
		public ServiceType Type { get; set; }
	}
}