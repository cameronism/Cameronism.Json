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
		internal static bool CompletelyIgnoringDipose = true; // FIXME

		// public delegate int LowWriter<T>(ref T value, byte* dst, int avail);

		delegate int PrimitiveWriter<T>(T value, byte* dst, int avail);

		const string LOCAL_DST = "destination";
		const string LOCAL_AVAIL = "available";

		public static Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> Create<T>(Schema schema)
		{
			MethodInfo simpleWriter = null;
			bool isSimple = false;

			switch (schema.JsonType)
			{
				case JsonType.Array:
				case JsonType.Object:
					// handle these after the switch
					break;
				case JsonType.String:
				case JsonType.Number:
				case JsonType.Integer:
				case JsonType.Boolean:
					isSimple = TryGetSimpleWriter(schema.NetType, out simpleWriter);
					if (isSimple)
					{
						break;
					}
					throw new ArgumentException("JSON primitive writer not found for " + schema.NetType.Name);
				default:
					throw new ArgumentException("Unknown JsonType " + schema.JsonType);
			}

			var emit = Sigil.Emit<Convert.LowWriter<T>>.NewDynamicMethod(schema.NetType.Name + "_toJSON");

			if (isSimple)
			{
				emit.LoadArgument(0);
				LoadIndirect(emit, schema.NetType); // deref argument 0

				EmitSimpleComplete(schema, emit, simpleWriter);
				return emit;
			}

			Sigil.Local local;
			// local for dst
			local = emit.DeclareLocal(typeof(byte*), LOCAL_DST);
			emit.LoadArgument(1);
			emit.StoreLocal(local);

			// local for avail
			local = emit.DeclareLocal(typeof(int), LOCAL_AVAIL);
			emit.LoadArgument(2);
			emit.StoreLocal(local);

			// CRITICAL before calling any composite methods top of stack must be `value` followed by `available` when pushResult is true
			emit.LoadArgument(2);

			emit.LoadArgument(0);
			LoadIndirect(emit, schema.NetType); // deref argument 0

			if (schema.JsonType == JsonType.Array)
			{
				if (schema.NetType.IsArray)
				{
					EmitArray(schema, emit, pushResult: true);
				}
				else
				{
					EmitEnumerable(schema, emit, pushResult: true);
				}
			}
			else if (schema.JsonType == JsonType.Object)
			{
				if (schema.Keys != null)
				{
					EmitDictionary(schema, emit, pushResult: true);
				}
				else
				{
					EmitObject(schema, emit, pushResult: true);
				}
			}

			emit.Return();
			return emit;
		}

		/// <summary>Must be called with value on top of the stack</summary>
		static void EmitInline<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth)
		{
			if (Nullable.GetUnderlyingType(schema.NetType) != null)
			{
				PushAddress(emit, schema.NetType);
			}

			switch (schema.JsonType)
			{
				case JsonType.Array:
					if (schema.NetType.IsArray)
					{
						EmitArray(schema, emit, depth, pushResult: false);
					}
					else
					{
						EmitEnumerable(schema, emit, depth, pushResult: false);
					}
					break;
				case JsonType.Object:
					if (schema.Keys != null)
					{
						EmitDictionary(schema, emit, depth, pushResult: false);
					}
					else
					{
						EmitObject(schema, emit, depth, pushResult: false);
					}
					break;
				case JsonType.String:
				case JsonType.Number:
				case JsonType.Integer:
				case JsonType.Boolean:
					MethodInfo simpleWriter;
					if (TryGetSimpleWriter(schema.NetType, out simpleWriter))
					{
						EmitSimpleInline(schema, emit, simpleWriter, depth);
						return;
					}
					throw new ArgumentException("JSON primitive writer not found for " + schema.NetType.Name);
				default:
					throw new ArgumentException("Unknown JsonType " + schema.JsonType);
			}
		}

		static void LoadIndirect<T>(Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, Type effective)
		{
			if (effective.IsClass || effective.IsInterface)
			{
				emit.LoadIndirect(effective);
			}
			else if (Nullable.GetUnderlyingType(effective) != null)
			{
				// do nothing for Nullable<> since it's methods actually expect a pointer (Nullable<>*)
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
		}

		static bool TryGetSimpleWriter(Type type, out MethodInfo method)
		{
			var effective = Nullable.GetUnderlyingType(type) ?? type;
			Delegate primitive;
			switch (Type.GetTypeCode(effective))
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
					if (effective == typeof(Guid))
					{
						primitive = (PrimitiveWriter<Guid>)Convert.WriteGuidFormatD;
						break;
					}
					if (effective == typeof(System.Net.IPAddress))
					{
						method = null;
						return true;
					}
					method = null;
					return false;
			}

			method = primitive.Method;
			return true;
		}

		static MethodInfo WriteNull = typeof(UnsafeJson.Convert).GetMethod("WriteNull", BindingFlags.NonPublic | BindingFlags.Static);

		static void EmitSimpleComplete<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, MethodInfo writer)
		{
			var effective = schema.NetType;

			if (Nullable.GetUnderlyingType(effective) != null)
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

			CallWriter(emit, writer, effective, false);

			emit.Return();
		}

		// never pushes result
		static void EmitSimpleInline<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, MethodInfo writer, int depth)
		{
			var effective = schema.NetType;
			Sigil.Label ifNull = null;

			var underlying = Nullable.GetUnderlyingType(effective);
			if (underlying != null)
			{
				ifNull = emit.DefineLabel();
				var hasValueTrue = emit.DefineLabel();

				emit.Duplicate(); // preserve value

				emit.Call(effective.GetProperty("HasValue").GetGetMethod());

				emit.BranchIfTrue(hasValueTrue);
				emit.Pop(); // discard value
				WriteConstant(emit, "null", push: false, depth: depth);
				emit.Branch(ifNull);

				emit.MarkLabel(hasValueTrue);

				emit.Call(effective.GetMethod("GetValueOrDefault", Type.EmptyTypes));

				effective = underlying;
			}

			CallWriter(emit, writer, effective, true);

			emit.Duplicate(); // preserve the written count

			// check if the write succeeded
			var success = emit.DefineLabel();
			emit.LoadConstant(0);
			emit.BranchIfGreater(success);
			{
				ReturnFailed(emit, depth + 1);
			}

			emit.MarkLabel(success);
			emit.Duplicate(); // preserve written count

			// advance LOCAL_DST
			emit.Convert<IntPtr>();
			emit.LoadLocal(LOCAL_DST);
			emit.Add();
			emit.StoreLocal(LOCAL_DST);


			// emit.Duplicate(); // preserve written count

			// decrement LOCAL_AVAIL
			emit.LoadConstant(-1);
			emit.Multiply();
			emit.LoadLocal(LOCAL_AVAIL);
			emit.Add();
			emit.StoreLocal(LOCAL_AVAIL);


			if (underlying != null)
			{
				emit.MarkLabel(ifNull);
			}
		}

		static void CallWriter<T>(Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, MethodInfo writer, Type effective, bool useLocals)
		{
			if (writer == null)
			{
				if (effective == typeof(System.Net.IPAddress))
				{
					EmitIPAddress(emit, useLocals);
					return;
				}
				throw new NotImplementedException();
			}

			var firstArg = writer.GetParameters()[0].ParameterType;
			if (firstArg != effective)
			{
				emit.Convert(firstArg);
			}

			if (useLocals)
			{
				emit.LoadLocal(LOCAL_DST);
				emit.LoadLocal(LOCAL_AVAIL);
			}
			else
			{
				emit.LoadArgument(1);
				emit.LoadArgument(2);
			}

			emit.Call(writer);
		}

		static void EmitIPAddress<T>(Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, bool useLocals)
		{
			Action pushAvailable = () =>
			{
				if (useLocals)
				{
					emit.LoadLocal(LOCAL_DST);
					emit.LoadLocal(LOCAL_AVAIL);
				}
				else
				{
					emit.LoadArgument(1);
					emit.LoadArgument(2);
				}
			};

			var ip = typeof(System.Net.IPAddress);

			var notNull = emit.DefineLabel();
			var done = emit.DefineLabel();

			// preserve ip
			emit.Duplicate();
			emit.BranchIfTrue(notNull);

			// value is null
			{
				emit.Pop(); // discard null

				pushAvailable();
				emit.Call(typeof(Convert).GetMethod("WriteNull", BindingFlags.Static | BindingFlags.NonPublic));

				emit.Branch(done);
			}

			emit.MarkLabel(notNull);

			// value is not null
			{
				var v6 = emit.DefineLabel();

				emit.Duplicate();
				emit.CallVirtual(ip.GetProperty("AddressFamily").GetMethod);
				emit.LoadConstant((int)System.Net.Sockets.AddressFamily.InterNetwork);
				emit.UnsignedBranchIfNotEqual(v6);

				// if v4
				{
					emit.LoadField(ip.GetField("m_Address", BindingFlags.Instance | BindingFlags.NonPublic));
					pushAvailable();
					emit.Call(typeof(Convert).GetMethod("WriteIPv4", BindingFlags.Static | BindingFlags.NonPublic));

					emit.Branch(done);
				}
				
				// if not v4 (assume v6)
				emit.MarkLabel(v6);
				{
					emit.LoadField(ip.GetField("m_Numbers", BindingFlags.Instance | BindingFlags.NonPublic));
					pushAvailable();
					emit.Call(typeof(Convert).GetMethod("WriteIPv6", BindingFlags.Static | BindingFlags.NonPublic));
				}
			}

			emit.MarkLabel(done);
		}

		static void EmitArray<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth = 0, bool pushResult = false)
		{

			var ifNotNull = emit.DefineLabel();
			var ifNull = emit.DefineLabel();

			emit.Duplicate(); // preserve the value
			emit.BranchIfTrue(ifNotNull);
			emit.Pop(); // discard value
			if (pushResult) emit.Pop(); // discard avail
			WriteConstant(emit, "null", push: pushResult, depth: depth);
			emit.Branch(ifNull);

			emit.MarkLabel(ifNotNull);

			int localDepth = pushResult ? 2 : 1;

			// begin array
			WriteConstant(emit, "[", depth: depth + localDepth);

			var endLoop = emit.DefineLabel();
			var beginLoop = emit.DefineLabel();

			/* var ix = 0; */
			using (var ix = emit.DeclareLocal<int>())
			{
				emit.LoadConstant(0);
				emit.StoreLocal(ix);

				/* goto endLoop; */
				emit.Branch(endLoop);

				/* beginLoop: */
				emit.MarkLabel(beginLoop);

				// write ','
				var afterComma = emit.DefineLabel();
				emit.LoadLocal(ix);
				emit.BranchIfFalse(afterComma);
				WriteConstant(emit, ",", depth: depth + localDepth);

				emit.MarkLabel(afterComma);

				/* `push` theArray[ix] */
				emit.Duplicate(); // preserve array value
				emit.LoadLocal(ix);
				emit.LoadElement(schema.Items.NetType);

				// write the element
				EmitInline(schema.Items, emit, depth + localDepth);

				/* ix += 1; */
				emit.LoadLocal(ix);
				emit.LoadConstant(1);
				emit.Add();
				emit.StoreLocal(ix);


				/* endLoop: */
				emit.MarkLabel(endLoop);


				/* if (theArray.Length > ix) goto beginLoop; */
				emit.Duplicate(); // preserve array value
				emit.LoadLength(schema.Items.NetType);
				emit.Convert<int>();
				emit.LoadLocal(ix);
				emit.BranchIfGreater(beginLoop);
			}

			// discard array value
			emit.Pop();

			// end array
			WriteConstant(emit, "]", depth: depth + localDepth - 1);



			if (pushResult)
			{
				// push bytes written
				emit.LoadLocal(LOCAL_AVAIL);
				emit.Subtract();
			}

			if (schema.Nullable)
			{
				emit.MarkLabel(ifNull);
			}
		}
		static void EmitEnumerable<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth = 0, bool pushResult = false)
		{
			EmitEnumerable(schema, emit, depth, pushResult, isArray: true);
		}

		static void EmitEnumerable<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth, bool pushResult, bool isArray)
		{
			var enumerable = EnumerableInfo.FindMethods(schema.NetType);
			MethodInfo getKey = null;
			MethodInfo getValue = null;
			if (!isArray)
			{
				getKey = enumerable.get_Current.ReturnType.GetProperty("Key").GetMethod;
				getValue = enumerable.get_Current.ReturnType.GetProperty("Value").GetMethod;
			}

			var ifNotNull = emit.DefineLabel();
			var ifNull = emit.DefineLabel();

			emit.Duplicate(); // preserve the value
			emit.BranchIfTrue(ifNotNull);
			emit.Pop(); // discard value
			if (pushResult) emit.Pop(); // discard avail
			WriteConstant(emit, "null", push: pushResult, depth: depth);
			emit.Branch(ifNull);

			emit.MarkLabel(ifNotNull);

			int localDepth = pushResult ? 2 : 1;

			// begin array
			WriteConstant(emit, isArray ? "[" : "{", depth: depth + localDepth);

			var loopBottom = emit.DefineLabel();
			var loopTop = emit.DefineLabel();
			var closeArray = emit.DefineLabel();

			
			// call GetEnumerator
			CallCorrectly(emit, enumerable.GetEnumerator, schema.NetType);
			var enumeratorType = enumerable.GetEnumerator.ReturnType;

			// use the pointer for enumerator structs
			if (enumeratorType.IsValueType)
			{
				PushAddress(emit, enumeratorType);
			}

			var exceptionBlock = CompletelyIgnoringDipose || enumerable.Dispose == null ? null : emit.BeginExceptionBlock();

			// unroll the first iteration so the loop can always write the comma

			/* if (!enumerator.MoveNext()) goto closeArray; */
			emit.Duplicate(); // preserve enumerator
			CallCorrectly(emit, enumerable.MoveNext, enumeratorType);
			emit.BranchIfFalse(closeArray);

			// push enumerator.Current
			emit.Duplicate(); // preserve enumerator
			CallCorrectly(emit, enumerable.get_Current, enumeratorType);

			if (!isArray)
			{
				// we've got a KeyValuePair<,>
				PushAddress(emit, getValue.DeclaringType);

				emit.Duplicate(); // preserve KeyValuePair<,>*
				emit.Call(getKey);
				// write the first key
				EmitInline(schema.Keys, emit, depth + localDepth + 1);
				WriteConstant(emit, ":", depth: depth + localDepth + 1);
				emit.Call(getValue);
			}

			// write the first element
			EmitInline(schema.Items, emit, depth + localDepth);

			emit.Branch(loopBottom);
			emit.MarkLabel(loopTop);

			// write ','
			WriteConstant(emit, ",", depth: depth + localDepth);

			// push enumerator.Current
			emit.Duplicate();// preserve enumerator
			CallCorrectly(emit, enumerable.get_Current, enumeratorType);

			if (!isArray)
			{
				// we've got a KeyValuePair<,>
				PushAddress(emit, getValue.DeclaringType);

				emit.Duplicate(); // preserve KeyValuePair<,>*
				emit.Call(getKey);
				// write the key
				EmitInline(schema.Keys, emit, depth + localDepth + 1);
				WriteConstant(emit, ":", depth: depth + localDepth + 1);
				emit.Call(getValue);
			}

			// write the element
			EmitInline(schema.Items, emit, depth + localDepth);


			/* endLoop: */
			emit.MarkLabel(loopBottom);

			/* if (enumerator.MoveNext()) goto beginLoop; */
			emit.Duplicate(); // preserve enumerator
			CallCorrectly(emit, enumerable.MoveNext, enumeratorType);
			emit.BranchIfTrue(loopTop);


			emit.MarkLabel(closeArray);

			if (exceptionBlock != null)
			{
				var f = emit.BeginFinallyBlock(exceptionBlock);
				CallCorrectly(emit, enumerable.Dispose, enumeratorType);
				emit.EndFinallyBlock(f);
				emit.EndExceptionBlock(exceptionBlock);
			}
			else
			{
				emit.Pop(); // discard enumerator
			}

			// end array
			WriteConstant(emit, isArray ? "]" : "}", depth: depth + localDepth - 1);



			if (pushResult)
			{
				// push bytes written
				emit.LoadLocal(LOCAL_AVAIL);
				emit.Subtract();
			}

			if (schema.Nullable)
			{
				emit.MarkLabel(ifNull);
			}
		}

		static void CallCorrectly<T>(Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, MethodInfo mi, Type instanceType)
		{
			if (mi.IsVirtual)
			{
				emit.CallVirtual(mi, constrained: instanceType != null && instanceType.IsValueType ? instanceType : null);
			}
			else
			{
				emit.Call(mi);
			}
		}

		static void EmitObject<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth = 0, bool pushResult = false)
		{
			var underlying = Nullable.GetUnderlyingType(schema.NetType);
			var systemNullable = underlying != null;
			Sigil.Label ifNull = null;
			bool anyMembers = schema.Members.Any();
			if (schema.Nullable)
			{
				var ifNotNull = emit.DefineLabel();
				ifNull = emit.DefineLabel();

				if (anyMembers)
				{
					emit.Duplicate(); // preserve the value
				}
				if (systemNullable)
				{
					emit.Call(schema.NetType.GetProperty("HasValue").GetMethod);
				}
				emit.BranchIfTrue(ifNotNull);
				if (anyMembers) emit.Pop(); // discard value
				if (pushResult) emit.Pop(); // discard avail
				WriteConstant(emit, "null", push: pushResult, depth: depth);
				emit.Branch(ifNull);

				emit.MarkLabel(ifNotNull);
				if (systemNullable)
				{
					emit.Call(schema.NetType.GetMethod("GetValueOrDefault", Type.EmptyTypes));
				}
			}

			// assert we've got at least minimum length
			var minLength = schema.CalculateMinimumLength();

			if (schema.Members.Count == 0)
			{
				WriteConstant(emit, "{}", depth: depth + 1);
			}
			else
			{
				for (int i = 0; i < schema.Members.Count; i++)
				{
					// assert we've got enough
					var enoughAvailable = emit.DefineLabel();
					emit.LoadLocal(LOCAL_AVAIL);
					emit.LoadConstant(minLength);
					emit.BranchIfGreaterOrEqual(enoughAvailable);
					{
						ReturnFailed(emit, depth + (pushResult ? 2: 1));
					}

					emit.MarkLabel(enoughAvailable);

					var member = schema.Members[i];
					var lastMember = i == schema.Members.Count - 1;

					// write the member name
					var label =
						(i == 0 ? "{" : ",") +
						"\"" + member.Key + "\":";
					minLength -= WriteConstant(emit, label, assertAvailable: false);
					minLength -= member.Value.CalculateMinimumLength(considerNullSelf: true);

					if (!lastMember)
					{
						// sanity check minLength
						if (minLength < 6) // ,"a":0
						{
							// FIXME use/create appropriate Exception type
							throw new InvalidOperationException("Minimum length calculation failed");
						}

						// preserve value except last time through the loop
						emit.Duplicate();
					}
					int loopDepth = depth + (lastMember ? 1 : 2) - (pushResult ? 0 : 1);

					// get the member value
					if (member.Value.FieldInfo != null)
					{
						emit.LoadField(member.Value.FieldInfo);
					}
					else
					{
						if (schema.NetType.IsValueType)
						{
							PushAddress(emit, systemNullable ? underlying : schema.NetType);
						}
						emit.Call(member.Value.PropertyInfo.GetMethod);
					}

					// write the member value
					EmitInline(member.Value, emit, loopDepth);

					// if we ran out of room inner writer will have bailed completely rather than push negative (currently)

					// we probably won't care about result until we create more fine grained estimates when out of room
				}
				WriteConstant(emit, "}", depth: depth + (pushResult ? 1 : 0));
			}

			if (pushResult)
			{
				// push bytes written
				emit.LoadLocal(LOCAL_AVAIL);
				emit.Subtract();
			}

			if (schema.Nullable)
			{
				emit.MarkLabel(ifNull);
			}
		}

		static void EmitDictionary<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth = 0, bool pushResult = false)
		{
			if (schema.Keys.JsonType != JsonType.String) throw new NotImplementedException();

			EmitEnumerable(schema, emit, depth, pushResult, isArray: false);
		}

		static void PushAddress<T>(Sigil.Emit<Convert.LowWriter<T>> emit, Type type)
		{
			using (var copy = emit.DeclareLocal(type))
			{
				emit.StoreLocal(copy);
				emit.LoadLocalAddress(copy);
			}
		}

		/// <param name="push">Push the written byte count on success</param>
		static int WriteConstant<T>(Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, string s, bool assertAvailable = true, bool push = false, int depth = 0)
		{
			// ASSUMPTION: the string does not require JSON escaping
			var bytes = Encoding.UTF8.GetBytes(s);

			if (assertAvailable)
			{
				var enoughAvailable = emit.DefineLabel();
				emit.LoadLocal(LOCAL_AVAIL);
				emit.LoadConstant(bytes.Length);
				emit.BranchIfGreaterOrEqual(enoughAvailable);
				{
					ReturnFailed(emit, depth);
				}

				emit.MarkLabel(enoughAvailable);
			}


			foreach (var b in bytes)
			{
				//IL_000A:  ldarg.0     
				//IL_000B:  dup         
				//IL_000C:  ldc.i4.1    
				//IL_000D:  conv.i      
				//IL_000E:  add         
				//IL_000F:  starg.s     00 
				//IL_0011:  ldc.i4.s    21 
				//IL_0013:  stind.i1    

				emit.LoadLocal(LOCAL_DST);
				emit.Duplicate();
				emit.LoadConstant(1);
				emit.Convert<IntPtr>();
				emit.Add();
				emit.StoreLocal(LOCAL_DST);
				emit.LoadConstant(b);
				emit.StoreIndirect<byte>();
			}

			// avail -= bytes.Length
			emit.LoadLocal(LOCAL_AVAIL);
			emit.LoadConstant(bytes.Length);
			emit.Subtract();
			emit.StoreLocal(LOCAL_AVAIL);

			if (push)
			{
				emit.LoadConstant(bytes.Length);
			}

			return bytes.Length;
		}


		static void ReturnFailed<T>(Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth)
		{
			for (int i = 0; i < depth; i++) emit.Pop();

			// ask for double
			emit.LoadArgument(2); // avail
			emit.LoadConstant(-2);
			emit.Multiply();
			emit.Return();
		}
	}
}
