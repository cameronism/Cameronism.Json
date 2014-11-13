using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cameronism.Json.Tests
{
	static class NewtonsoftConverters
	{
		public class TypeConverter : Newtonsoft.Json.JsonConverter
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

				writer.WriteValue(SchemaTest.HumanName((Type)value));
			}
		}

		public class MemberInfoConverter : Newtonsoft.Json.JsonConverter
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

		public class IPAddressConverter : Newtonsoft.Json.JsonConverter
		{

			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(System.Net.IPAddress);
			}

			public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}

			public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
			{
				if (value == null) writer.WriteNull();
				else writer.WriteValue(value.ToString());
			}
		}
	}
}
