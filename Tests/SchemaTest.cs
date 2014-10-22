using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
	public class SchemaTest
	{
		#region converters
		class TypeConverter : Newtonsoft.Json.JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return typeof(Type).IsAssignableFrom(objectType);
			}

			public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
			{
				throw new NotSupportedException();
			}

			public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
			{
				if (value == null)
				{
					writer.WriteNull();
					return;
				}

				writer.WriteValue(HumanName((Type)value));
			}
		}

		class MemberInfoConverter : Newtonsoft.Json.JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return typeof(FieldInfo).IsAssignableFrom(objectType) || typeof(PropertyInfo).IsAssignableFrom(objectType);
			}

			public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
			{
				throw new NotSupportedException();
			}

			public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
			{
				var fi = value as FieldInfo;
				var pi = value as PropertyInfo;
				var details = new
				{
					Name = 
						fi != null ? fi.Name : 
						pi != null ? pi.Name :
						null,
					MemberType =
						fi != null ? fi.FieldType : 
						pi != null ? pi.PropertyType :
						null,
				};

				if (details.Name != null)
				{
					serializer.Serialize(writer, details);
				}
				else
				{
					writer.WriteNull();
				}
			}
		}
		#endregion

		[System.Runtime.Serialization.DataContract]
		class ExplicitDataMemberOrder
		{
			[System.Runtime.Serialization.DataMember(Order=3)]
			public int C;
			[System.Runtime.Serialization.DataMember(Order=2)]
			public int B;
			[System.Runtime.Serialization.DataMember(Order=1)]
			public int A;
		}

		public static string HumanName(Type t)
		{
			var ns = t.Namespace;
			var name = t.Name;
			if (t.IsGenericType)
			{
				name = name.Substring(0, name.IndexOf('`')) + "<" +  String.Join(", ", t.GetGenericArguments().Select(a => HumanName(a))) + ">";

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
				new TypeConverter(), 
				new Newtonsoft.Json.Converters.StringEnumConverter { },
				new MemberInfoConverter(),
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
			})
			{
				var schema = UnsafeJson.Schema.Reflect(t);
				var json = Newtonsoft.Json.JsonConvert.SerializeObject(schema, Newtonsoft.Json.Formatting.Indented, converters);

				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("# " + HumanName(t));
				sb.AppendLine(json);
			}


			ApprovalTests.Approvals.Verify(sb.ToString());
		}
	}
}
