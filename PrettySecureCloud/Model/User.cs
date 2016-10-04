using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PrettySecureCloud.Model
{
	[DataContract]
	public class User
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Username { get; set; }

		[DataMember]
		public string Mail { get; set; }

		[DataMember]
		public byte[] EncryptionKey { get; set; }

		[DataMember]
		public IEnumerable<CloudService> Services { get; set; }
	}
}