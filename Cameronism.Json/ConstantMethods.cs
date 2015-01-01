using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cameronism.Json
{
	internal static class ConstantMethods
	{
		static MethodInfo _Disassemble = typeof(ConstantMethods).GetMethod("Disassemble", BindingFlags.Static | BindingFlags.NonPublic);

		public static string TryGetJson(MethodInfo mi, Type type)
		{
			var declaringType = mi.DeclaringType;
			if (declaringType.IsValueType) return null; // Don't know how to make Sigil.Disassembler happy with these - generics + struct

			if (mi.IsVirtual && !type.IsSealed) return null; // Do not inline

			return (string)_Disassemble.MakeGenericMethod(declaringType, mi.ReturnType).Invoke(null, new object[] { mi });
		}

		static string Disassemble<T1, TReturn>(MethodInfo mi)
		{
			var del = (Func<T1, TReturn>)mi.CreateDelegate(typeof(Func<T1, TReturn>));
			Exception ex;
			var ops = TryDisassemble(del, out ex);
			if (ops == null) return null;

			return GetConstantJson(ops, mi);
		}

		public static Sigil.DisassembledOperations<T> TryDisassemble<T>(T func, out Exception ex) where T : class
		{
			try
			{
				ex = null;
				return Sigil.Disassembler<T>.Disassemble(func);
			}
			catch (Exception e)
			{
				ex = e;
				// probably sigil bug
				return null;
			}
		}

		const int MIN_INTEGRAL = (int)TypeCode.Boolean;
		const int MAX_INTEGRAL = (int)TypeCode.UInt64;

		static string GetConstantJson<T>(Sigil.DisassembledOperations<T> ops, MethodInfo mi)
		{
			return GetConstantJson(ops.Select(op => new KeyValuePair<System.Reflection.Emit.OpCode, IEnumerable<object>>(op.OpCode, op.Parameters)), mi);
		}
		static string GetConstantJson(IEnumerable<KeyValuePair<System.Reflection.Emit.OpCode, IEnumerable<object>>> ops, MethodInfo mi)
		{
			string theValue = null;
			var typeCode = (int)Type.GetTypeCode(Nullable.GetUnderlyingType(mi.ReturnType) ?? mi.ReturnType);
			
			foreach (var op in ops)
			{
				switch ((ushort)op.Key.Value)
				{
					case 0x00: // Nop
					case 0x2a: // Ret
					case 0x2b: // Br_S
						continue;
						
					case 0x14: // Ldnull
						theValue = "null";
						continue;

					case 0xfe15: // Initobj
						theValue = GetDefaultValue(op.Value.FirstOrDefault() as Type);
						continue;
						
					case 0x0a: // Stloc_0
					case 0x06: // Ldloc_0
					case 0x07: // Ldloc_1
					case 0x12: // Ldloca_S
					case 0x73: // Newobj
					case 0x6a: // Conv_I8
						continue;
						
					case 0x72: // Ldstr
						if (mi.ReturnType == typeof(string))
						{
							theValue = GetEscapedString(op.Value.FirstOrDefault() as string);
							continue;
						}
						else
						{
							return null;
						}

					case 0x15: // Ldc_I4_M1
					case 0x16: // Ldc_I4_0
					case 0x17: // Ldc_I4_1
					case 0x18: // Ldc_I4_2
					case 0x19: // Ldc_I4_3
					case 0x1a: // Ldc_I4_4
					case 0x1b: // Ldc_I4_5
					case 0x1c: // Ldc_I4_6
					case 0x1d: // Ldc_I4_7
					case 0x1e: // Ldc_I4_8
					case 0x1f: // Ldc_I4_S
					case 0x20: // Ldc_I4
						if (typeCode >= MIN_INTEGRAL && typeCode <= MAX_INTEGRAL)
						{
							theValue = GetInt32Value(op.Value.FirstOrDefault(), mi.ReturnType);
							continue;
						}
						else
						{
							return null;
						}
						
					case 0x21: // Ldc_I8
						theValue = GetInt64Value(op.Value.FirstOrDefault(), mi.ReturnType);
						continue;

					case 0x22: // Ldc_R4
					case 0x23: // Ldc_R8
						theValue = GetDoubleValue(op.Value.FirstOrDefault(), mi.ReturnType);
						continue;
					
					// assume non constant if we encounter anything else
					default:
						//op.Dump("bailing");
						//op.OpCode.Value.ToString("x").Dump();
						return null;
				}
			}
			
			return theValue;
		}

		static string GetDefaultValue(Type t)
		{
			if (Nullable.GetUnderlyingType(t) != null) return "null";

			if (t == typeof(DateTime)) return "\"0001-01-01T00:00:00\"";
			if (t == typeof(Guid)) return "\"00000000-0000-0000-0000-000000000000\"";

			return null;
		}

		static string GetInt32Value(object value, Type returnType)
		{
			if (!(value is int)) return null;
			
			if (returnType == typeof(ulong) || returnType == typeof(ulong?))
			{
				return GetInt64Value((long)(int)value, returnType);
			}
			
			var num = (int)value;
			if (returnType == typeof(char) || returnType == typeof(char?))
			{
				return GetEscapedString(((char)num).ToString());
			}
			
			if (returnType == typeof(bool) || returnType == typeof(bool?))
			{
				return num == 0 ? "false" : "true";
			}
			
			if (returnType == typeof(uint) || returnType == typeof(uint?))
			{
				var unum = (uint)num;
				return unum.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}
			
			return num.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		static string GetInt64Value(object value, Type returnType)
		{
			if (!(value is long)) return null;
			
			var num = (long)value;	

			if (returnType == typeof(ulong) || returnType == typeof(ulong?))
			{
				var unum = (ulong)num;
				return unum.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}

			return num.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		unsafe static string GetEscapedString(string value)
		{
			var required = Encoding.UTF8.GetByteCount(value);
			byte[] buffer;
			required = required * 2 + 2;
			int result;
			while (true)
			{
				buffer = new byte[required];
				fixed (byte* ptr = buffer)
				{
					result = ConvertUTF.WriteStringUtf8(value, ptr, buffer.Length);
				}
				if (result > 2) break;

				required *= 2;
			}
			return Encoding.UTF8.GetString(buffer, 0, result);
		}

		static string GetDoubleValue(object value, Type returnType)
		{
			if (!(value is double))
			{
				if (value is float)
				{
					value = (double)(float)value;
				}
				else
				{
					return null;
				}
			}
			
			var num = (double)value;	
			var stringVal = num.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
			if (stringVal.IndexOf('.') == -1) stringVal += ".0";
			return stringVal;
		}
	}
}
