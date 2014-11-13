using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cameronism.Json.Benchmarks
{
	[DataContract]
	internal class ImpressionDetail
	{
		[DataMember(Order=1)]
		public string Name { get; set; }
		[DataMember(Order=2)]
		public DateTime Timestamp { get; set; }
		[DataMember(Order=3)]
		public uint MessageId { get; set; }
		[DataMember(Order=4)]
		public Guid FPSessionId { get; set; }
		[DataMember(Order=5)]
		public string UserAgent { get; set; }
		[DataMember(Order=6)]
		public Dictionary<string, string> SessionValues { get; set; }
		[DataMember(Order=7)]
		public List<GroupInfo> Groups { get; set; }

		[DataContract]
		internal class GroupInfo
		{
			[DataMember(Order=1)]
			public string Name { get; set; }
			[DataMember(Order=2)]
			public uint Id { get; set; }
		}
	}
}
