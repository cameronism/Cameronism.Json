using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnsafeJson
{
	internal class Schema
	{
		/// <summary>.NET type</summary>
		public Type NetType { get; set; }
		/// <summary>JSON type</summary>
		public JsonType JsonType { get; set; }
		/// <summary>If JSON value may be null</summary>
		public bool Nullable { get; set; }
		/// <summary>T for IEnumerable of T OR TValue from IEnumerable of KeyValuePair</summary>
		public Schema Items { get; set; }
		/// <summary>TKey from IEnumerable of KeyValuePair</summary>
		public Schema Keys { get; set; }
		/// <summary>Members</summary>
		public IReadOnlyList<KeyValuePair<string, Schema>> Members { get; set; }

		public FieldInfo FieldInfo { get; set; }
		public PropertyInfo PropertyInfo { get; set; }

		public int CalculateMinimumLength(bool considerNullMembers = true, bool considerNullSelf = false)
		{
			switch (JsonType)
			{
				case JsonType.String:
					return 2; // ""
				case JsonType.Number:
					return 3; // 0.0
				case JsonType.Integer:
					return 1; // 0
				case JsonType.Boolean:
					return 4; // true
				case JsonType.Array:
					return 2; // []
				case JsonType.Object:
					if (Keys != null || (Members != null && Members.Count == 0)) return 2; // {}

					if (Members == null) break;

					var min = 2 + Members.Count - 1; // {} + ,
					foreach (var member in Members)
					{
						min += member.Key.Length + 3; // "member name":

						var memberMin = member.Value.CalculateMinimumLength();
						min += considerNullMembers && member.Value.Nullable && memberMin > 4 ? 4 : memberMin;
					}

					if (considerNullSelf && Nullable && min > 4) return 4;

					return min;
			}

			throw new ArgumentException();
		}

		static KeyValuePair<string, Schema> Reflect(FieldInfo fi)
		{
			var s = Reflect(fi.FieldType);
			s.FieldInfo = fi;
			return new KeyValuePair<string, Schema>(fi.Name, s);
		}

		static KeyValuePair<string, Schema> Reflect(PropertyInfo pi)
		{
			var s = Reflect(pi.PropertyType);
			s.PropertyInfo = pi;
			return new KeyValuePair<string, Schema>(pi.Name, s);
		}

		const string DATA_CONTRACT_ATTRIBUTE = "System.Runtime.Serialization.DataContractAttribute";
		const string DATA_MEMBER_ATTRIBUTE = "System.Runtime.Serialization.DataMemberAttribute";

		static bool IsDataMember(object a)
		{
			return a.GetType().FullName == DATA_MEMBER_ATTRIBUTE;
		}

		static void SortDataMember(List<KeyValuePair<string, Schema>> members)
		{
			var dataMembers = members
				.Select(kvp => new
					{
						Key = kvp.Key,
						Value = kvp.Value,
						DataMember = 
							kvp.Value.FieldInfo != null ? kvp.Value.FieldInfo.GetCustomAttributes(true).FirstOrDefault(IsDataMember) :
							kvp.Value.PropertyInfo != null ? kvp.Value.PropertyInfo.GetCustomAttributes(true).FirstOrDefault(IsDataMember) :
							null
					})
				.ToList();

			var firstDataMember = dataMembers.FirstOrDefault(dm => dm.DataMember != null);

			if (firstDataMember == null) return;

			var order = firstDataMember.DataMember.GetType().GetProperty("Order", BindingFlags.Public | BindingFlags.Instance);

			// start over
			members.Clear();

			int unordered = 1;
			var sorted = dataMembers
				.Where(dm => dm.DataMember != null)
				.OrderBy(dm =>
				{
					int memberOrder = (int)order.GetValue(dm.DataMember, null);
					return memberOrder >= 0 ? int.MinValue + memberOrder : ++unordered;
				});

			foreach (var m in sorted)
			{
				members.Add(new KeyValuePair<string, Schema>(m.Key, m.Value));
			}
		}

		static void SortMembers(Type t, List<KeyValuePair<string, Schema>> members)
		{
			if (!members.Any()) return;

			var attr = t.GetCustomAttributes(true);

			var dataContract = attr.FirstOrDefault(a => a.GetType().FullName == DATA_CONTRACT_ATTRIBUTE);
			if (dataContract != null)
			{
				SortDataMember(members);
				return;
			}

			// FIXME
			// TODO
		}

		public static Schema Reflect(Type t)
		{
			var underlying = System.Nullable.GetUnderlyingType(t);
			Type itemType;
			Type keyType;
			var s = new Schema
			{
				NetType = t,
				Nullable = GetNullable(t, underlying),
				JsonType = GetJsonType(t, underlying, out itemType, out keyType),
				Items = itemType == null ? null : Reflect(itemType),
				Keys = keyType == null ? null : Reflect(keyType),
			};

			if (s.JsonType == JsonType.Object && s.Keys == null)
			{
				var memberBag = underlying ?? t;
				var members = new List<KeyValuePair<string, Schema>>();

				members.AddRange(memberBag.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(Reflect));
				members.AddRange(memberBag.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(Reflect));

				SortMembers(memberBag, members);
				s.Members = members;
			}

			return s;
		}

		static bool GetNullable(Type t, Type underlying)
		{
			if (t.IsClass || t.IsInterface) return true;
			return underlying != null;
		}

		static JsonType GetJsonType(Type t, Type underlying, out Type itemType, out Type keyType)
		{
			itemType = null;
			keyType = null;

			if (underlying == null) underlying = t;

			switch (Type.GetTypeCode(underlying))
			{
				case TypeCode.Boolean: return JsonType.Boolean;
				case TypeCode.Byte: return JsonType.Integer;
				case TypeCode.Char: return JsonType.String;
				case TypeCode.DateTime: return JsonType.String;
				case TypeCode.Decimal: return JsonType.Number;
				case TypeCode.Double: return JsonType.Number;
				case TypeCode.Int16: return JsonType.Integer;
				case TypeCode.Int32: return JsonType.Integer;
				case TypeCode.Int64: return JsonType.Integer;
				case TypeCode.SByte: return JsonType.Integer;
				case TypeCode.Single: return JsonType.Number;
				case TypeCode.String: return JsonType.String;
				case TypeCode.UInt16: return JsonType.Integer;
				case TypeCode.UInt32: return JsonType.Integer;
				case TypeCode.UInt64: return JsonType.Integer;
			}

			if (underlying == typeof(Guid) || underlying == typeof(DateTimeOffset) || underlying == typeof(System.Net.IPAddress)) return JsonType.String;

			IEnumerable<Type> interfaces = t.GetInterfaces();
			if (t.IsInterface) interfaces = new[] { t }.Concat(interfaces);

			// limit to generic
			var genericInterfaces = interfaces
				.Where(ti => ti.IsGenericType)
				.Select(ti => new { gd = ti.GetGenericTypeDefinition(), ti })
				.ToList();

			if (genericInterfaces.Any())
			{
				var id = genericInterfaces.FirstOrDefault(ti => ti.gd == typeof(IDictionary<,>) || ti.gd == typeof(IReadOnlyDictionary<,>));
				if (id != null)
				{
					var args = id.ti.GetGenericArguments();
					keyType = args[0];
					itemType = args[1];
					return JsonType.Object;
				}

				var ie = genericInterfaces.FirstOrDefault(ti => ti.gd == typeof(IEnumerable<>));
				if (ie != null)
				{
					itemType = ie.ti.GetGenericArguments()[0];
					return JsonType.Array;
				}
			}

			// we're left with object
			return JsonType.Object;
		}

	}
}
