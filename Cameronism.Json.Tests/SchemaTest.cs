using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cameronism.Json.Tests
{
	public class SchemaTest
	{
		interface IMarker<T>
		{
		}

		class Marked : IMarker<int>
		{
		}

		[System.Runtime.Serialization.DataContract]
		public class ExplicitDataMemberOrder
		{
			[System.Runtime.Serialization.DataMember(Order=3)]
			public int C;
			[System.Runtime.Serialization.DataMember(Order=2)]
			public int B;
			[System.Runtime.Serialization.DataMember(Order=1)]
			public int A;
		}

		[System.Runtime.Serialization.DataContract]
		public class DataMemberSomeOrder
		{
			[System.Runtime.Serialization.DataMember]
			public int C { get; set; }
			[System.Runtime.Serialization.DataMember(Order=2)]
			public int B;
			[System.Runtime.Serialization.DataMember(Order=1)]
			public int A;
		}

		[System.Runtime.Serialization.DataContract]
		public class NoDataMember
		{
			public int A;
			public int B { get; set; }
		}

		public static string HumanName(Type t)
		{
			var name = t.Name;
			if (t.IsGenericParameter) return name;

			var original = t;
			var ns = t.Namespace;

			var ix = name.IndexOf('`');
			if (ix != -1)
			{
				var genericArgs = t.GetGenericArguments();
				name = name.Substring(0, ix) + "<" + String.Join(", ", genericArgs.Select(a => HumanName(a))) + ">";
				if (t.IsArray) name += "[]";
			}

			while (t.IsNested)
			{
				t = t.DeclaringType;
				var next = t.Name;

				ix = next.IndexOf('`');
				if (ix != -1)
				{
					var genericArgs = original.GetGenericArguments();
					next = next.Substring(0, ix) + "<" + String.Join(", ", genericArgs.Select(a => HumanName(a))) + ">";
				}

				name = next + "+" + name;
			}


			return String.IsNullOrEmpty(ns) ? name : ns + "." + name;
		}

		static Type GetType<T>(T sample)
		{
			return typeof(T);
		}

		[Fact]
		public void Reflect()
		{
			var converters = new Newtonsoft.Json.JsonConverter[] { 
				new NewtonsoftConverters.TypeConverter(), 
				new Newtonsoft.Json.Converters.StringEnumConverter { },
				new NewtonsoftConverters.MemberInfoConverter(),
			};
			var sb = new StringBuilder();

			foreach (var t in new[] { 
				typeof(System.Int32),
				typeof(System.Boolean),
				typeof(System.Byte),
				typeof(System.Char),
				typeof(System.DateTime),
				typeof(System.Decimal),
				typeof(System.Double),
				typeof(System.Int16),
				typeof(System.Int64),
				typeof(System.SByte),
				typeof(System.Single),
				typeof(System.String),
				typeof(System.UInt16),
				typeof(System.UInt32),
				typeof(System.UInt64),

				typeof(System.Int32?),
				typeof(System.Boolean?),
				typeof(System.Byte?),
				typeof(System.Char?),
				typeof(System.DateTime?),
				typeof(System.Decimal?),
				typeof(System.Double?),
				typeof(System.Int16?),
				typeof(System.Int64?),
				typeof(System.SByte?),
				typeof(System.Single?),
				typeof(System.UInt16?),
				typeof(System.UInt32?),
				typeof(System.UInt64?),

				typeof(System.Guid),
				typeof(System.Guid?),

				typeof(IEnumerable<int>),
				typeof(string[]),
				typeof(IReadOnlyList<Guid>),

				GetType(new { i = 1 }),
				GetType(new[] { new { s = "" } }.ToList()),

				typeof(Dictionary<string, string>),
				typeof(IReadOnlyDictionary<Guid, System.DayOfWeek>),

				typeof(KeyValuePair<int, int>),
				typeof(KeyValuePair<int, int>?),

				typeof(DateTimeOffset),
				typeof(DateTimeOffset?),

				typeof(System.Net.IPAddress),

				typeof(ExplicitDataMemberOrder),
				typeof(DataMemberSomeOrder),
				typeof(NoDataMember),

				typeof(Marked),
			})
			{
				var schema = Cameronism.Json.Schema.Reflect(t);
				var json = Newtonsoft.Json.JsonConvert.SerializeObject(schema, Newtonsoft.Json.Formatting.Indented, converters);

				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("# " + HumanName(t));
				sb.AppendLine(json);
			}


			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		[Fact]
		public void MinimumLengthCalculation()
		{
			Schema schema;

			schema = new Schema
			{
				JsonType = (JsonType)int.MaxValue,
			};
			Assert.Throws<ArgumentException>(() => schema.CalculateMinimumLength());


			schema = new Schema
			{
				JsonType = JsonType.Object,
				Members = new KeyValuePair<string, Schema>[0],
			};
			Assert.Equal(2, schema.CalculateMinimumLength());


			schema = new Schema
			{
				JsonType = JsonType.Object,
				Members = null,
			};
			Assert.Throws<ArgumentException>(() => schema.CalculateMinimumLength());
		}
	}
}
