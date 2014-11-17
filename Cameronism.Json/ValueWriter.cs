using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cameronism.Json
{
	unsafe internal class ValueWriter
	{
		#region inner types
		delegate int PrimitiveWriter<T>(T value, byte* dst, int avail);
		internal struct Lookup : IEquatable<Lookup>
		{
			public readonly Type Type;
			public readonly long Flags;

			public Lookup(Type type, long flags = 0)
			{
				this.Type = type;
				this.Flags = flags;
			}

			public Lookup(Type type, DestinationType destination) : this(type, (long)destination)
			{
			}


			public bool Equals(Lookup that)
			{
				return
					this.Flags == that.Flags &&
					this.Type == that.Type;
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int hash = 17;
					hash = hash * 23 + this.Type.GetHashCode();
					hash = hash * 23 + this.Flags.GetHashCode();
					return hash;
				}
			}

			public static Lookup Default<T>()
			{
				return new Lookup(typeof(T));
			}

			public static implicit operator Lookup(Type type)
			{
				return new Lookup(type);
			}
		}
		#endregion

		#region static
		internal static Dictionary<Lookup, ValueWriter> _Writers = new Dictionary<Lookup, ValueWriter>();

		static void Add<T>(PrimitiveWriter<T> writer, Dictionary<Lookup, ValueWriter> dict, long flags = 0)
		{
			_Writers[new Lookup(typeof(T), flags)] = ValueWriter.From(writer);
		}

		static ValueWriter()
		{
			Add<int>(Serializer.WriteInt32, _Writers);
			Add<uint>(Serializer.WriteUInt32, _Writers);
			Add<long>(Serializer.WriteInt64, _Writers);
			Add<ulong>(Serializer.WriteUInt64, _Writers);
			Add<bool>(Serializer.WriteBoolean, _Writers);
			Add<Guid>(Serializer.WriteGuidFormatD, _Writers);
			Add<DateTime>(Serializer.WriteDateTime8601, _Writers);
			Add<float>(Serializer.WriteSingle, _Writers);
			Add<double>(Serializer.WriteDouble, _Writers);
			Add<decimal>(Serializer.WriteDecimal, _Writers);
			Add<char>(ConvertUTF.WriteCharUtf8, _Writers);

			_Writers[typeof(byte)] = new ValueWriter
			{
				MaxLength = 3,
				Type = typeof(byte),
				MethodInfo = _Writers[typeof(uint)].MethodInfo,
			};

			_Writers[typeof(sbyte)] = new ValueWriter
			{
				MaxLength = 4,
				Type = typeof(sbyte),
				MethodInfo = _Writers[typeof(int)].MethodInfo,
			};

			_Writers[typeof(ushort)] = new ValueWriter
			{
				MaxLength = 5,
				Type = typeof(ushort),
				MethodInfo = _Writers[typeof(uint)].MethodInfo,
			};

			_Writers[typeof(short)] = new ValueWriter
			{
				MaxLength = 6,
				Type = typeof(short),
				MethodInfo = _Writers[typeof(int)].MethodInfo,
			};

			_Writers[typeof(System.Net.IPAddress)] = new ValueWriter
			{
				MaxLength = 48,
				Type = typeof(System.Net.IPAddress),
				MethodInfo = null, // IPAddress is a special case to handle v4 and v6 effeciently
			};

			// 2 string options
			Add<string>(ConvertUTF.WriteStringUtf8, _Writers, (long)DestinationType.Pointer);
			_Writers[new Lookup(typeof(string), DestinationType.Stream)] = new ValueWriter
			{
				Type = typeof(string),
				MethodInfo = typeof(ConvertUTF).GetMethod("WriteToStreamUtf8", BindingFlags.Static | BindingFlags.NonPublic),
			};
		}

		static ValueWriter From<T>(PrimitiveWriter<T> writer)
		{
			var w = new ValueWriter
			{
				MethodInfo = writer.Method,
				Type = typeof(T),
			};

			var attr = w.MethodInfo.GetCustomAttribute<ValueWriterAttribute>();
			if (attr != null && attr.MaxLength > 0)
			{
				w.MaxLength = attr.MaxLength;
			}

			return w;
		}

		internal static bool TryGetWriter(Type type, DestinationType destination, out ValueWriter writer)
		{
			var effective = Nullable.GetUnderlyingType(type) ?? type;

			// TODO support enums as strings too - this won't apply then
			if (effective.IsEnum)
			{
				effective = effective.GetEnumUnderlyingType();
			}

			var lookup = effective == typeof(string) ?
				new Lookup(effective, destination) : 
				new Lookup(effective);

			return _Writers.TryGetValue(lookup, out writer);
		}
		#endregion

		public MethodInfo MethodInfo;
		public int? MaxLength;
		public Type Type;
	}
}
