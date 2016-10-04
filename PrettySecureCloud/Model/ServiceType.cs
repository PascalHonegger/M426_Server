using System.Runtime.Serialization;

namespace PrettySecureCloud.Model
{
	[DataContract]
	public class ServiceType
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Key { get; set; }

		[DataMember]
		public string Secret { get; set; }
	}
}