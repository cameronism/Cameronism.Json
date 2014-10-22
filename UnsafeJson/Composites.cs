using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnsafeJson
{
	internal unsafe class Composites
	{
		// public delegate int LowWriter<T>(ref T value, byte* dst, int avail);

		delegate int PrimitiveWriter<T>(T value, byte* dst, int avail);

		public static Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> Create<T>(Schema schema)
		{
			var emit = Sigil.Emit<Convert.LowWriter<T>>.NewDynamicMethod(schema.NetType.Name + "_toJSON");


			var underlying = Nullable.GetUnderlyingType(schema.NetType);
			bool systemNullable = underlying != null;
			var actual = underlying ?? schema.NetType;

			Delegate primitive = null;
			switch (Type.GetTypeCode(actual))
			{
				case TypeCode.Boolean:
					primitive = (PrimitiveWriter<bool>)Convert.WriteBoolean;
					break;
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
					primitive = (PrimitiveWriter<uint>)Convert.WriteUInt32;
					break;
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
					primitive = (PrimitiveWriter<int>)Convert.WriteInt32;
					break;
				case TypeCode.Int64:
					primitive = (PrimitiveWriter<long>)Convert.WriteInt64; break;
				case TypeCode.UInt64:
					primitive = (PrimitiveWriter<ulong>)Convert.WriteUInt64; break;
				case TypeCode.DateTime:
					primitive = (PrimitiveWriter<DateTime>)Convert.WriteDateTime8601; break;
				case TypeCode.Decimal:
					primitive = (PrimitiveWriter<Decimal>)Convert.WriteDecimal; break;
				case TypeCode.Double:
					primitive = (PrimitiveWriter<double>)Convert.WriteDouble; break;
				case TypeCode.Single:
					primitive = (PrimitiveWriter<float>)Convert.WriteSingle; break;
				case TypeCode.Char:
					primitive = (PrimitiveWriter<char>)ConvertUTF.WriteCharUtf8; break;
				case TypeCode.String:
					primitive = (PrimitiveWriter<string>)ConvertUTF.WriteStringUtf8; break;
				default:
					if (actual == typeof(Guid))
					{
						primitive = (PrimitiveWriter<Guid>)Convert.WriteGuidFormatD;
					}
					break;
			}

			if (primitive != null)
			{
				CreatePrimitive(schema, emit, primitive.Method, systemNullable);
			}

			return emit;
		}

		static MethodInfo WriteNull = typeof(UnsafeJson.Convert).GetMethod("WriteNull", BindingFlags.NonPublic | BindingFlags.Static);

		static void CreatePrimitive<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, MethodInfo writer, bool systemNullable)
		{
			var effective = schema.NetType;

			emit.LoadArgument(0);
			if (effective.IsClass || effective.IsInterface)
			{
				emit.LoadIndirect(effective);
			}
			else if (systemNullable)
			{
				//emit.LoadObject(effective);
			}
			else
			{
				switch (Type.GetTypeCode(effective))
				{
					case TypeCode.DateTime:
					case TypeCode.Decimal:
					default:
						emit.LoadObject(effective);
						break;
					case TypeCode.Byte:
					case TypeCode.Double:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.SByte:
					case TypeCode.Single:
					case TypeCode.UInt16:
					case TypeCode.UInt32:
					case TypeCode.UInt64:
					case TypeCode.Char:
					case TypeCode.Boolean:
						emit.LoadIndirect(effective);
						break;
				}
			}

			if (systemNullable)
			{
				emit.Duplicate(); // prepare argument 0 for GetValueOrDefault call

				emit.Call(effective.GetProperty("HasValue").GetGetMethod());

				var hasValueTrue = emit.DefineLabel("HasValue_true");
				emit.BranchIfTrue(hasValueTrue);
				emit.Pop(); // discard argument 0
				emit.LoadArgument(1);
				emit.LoadArgument(2);
				emit.Call(WriteNull);
				emit.Return();

				emit.MarkLabel(hasValueTrue);

				emit.Call(effective.GetMethod("GetValueOrDefault", Type.EmptyTypes));

				effective = Nullable.GetUnderlyingType(effective);
			}

			var firstArg = writer.GetParameters()[0].ParameterType;
			if (firstArg != effective)
			{
				emit.Convert(firstArg);
			}

			emit.LoadArgument(1);
			emit.LoadArgument(2);
			emit.Call(writer);

			emit.Return();
		}
	}
}
