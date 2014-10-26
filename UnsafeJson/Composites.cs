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

		const string LOCAL_DST = "destination";
		const string LOCAL_AVAIL = "available";

		public static Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> Create<T>(Schema schema)
		{
			MethodInfo simpleWriter = null;

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
					if (TryGetSimpleWriter(schema.NetType, out simpleWriter))
					{
						break;
					}
					throw new ArgumentException("JSON primitive writer not found for " + schema.NetType.Name);
				default:
					throw new ArgumentException("Unknown JsonType " + schema.JsonType);
			}

			var emit = Sigil.Emit<Convert.LowWriter<T>>.NewDynamicMethod(schema.NetType.Name + "_toJSON");

			if (simpleWriter != null)
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

			// CRITICAL before calling any composite methods top of stack must be `value` followed by `available`
			emit.LoadArgument(2);

			emit.LoadArgument(0);
			LoadIndirect(emit, schema.NetType); // deref argument 0

			if (schema.JsonType == JsonType.Array)
			{
				if (schema.NetType.IsArray)
				{
					EmitArray(schema, emit);
				}
				else
				{
					EmitEnumerable(schema, emit);
				}
			}
			else if (schema.JsonType == JsonType.Object)
			{
				if (schema.Keys != null)
				{
					EmitDictionary(schema, emit);
				}
				else
				{
					EmitObject(schema, emit);
				}
			}

			emit.Return();
			return emit;
		}

		/// <summary>Must be called with value on top of the stack</summary>
		static void EmitInline<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth, out bool simple)
		{
			switch (schema.JsonType)
			{
				case JsonType.Array:
					simple = false;
					PrepareStackComposite(schema, emit);
					if (schema.NetType.IsArray)
					{
						EmitArray(schema, emit, depth);
					}
					else
					{
						EmitEnumerable(schema, emit, depth);
					}
					break;
				case JsonType.Object:
					simple = false;
					PrepareStackComposite(schema, emit);
					if (schema.Keys != null)
					{
						EmitDictionary(schema, emit, depth);
					}
					else
					{
						EmitObject(schema, emit, depth);
					}
					break;
				case JsonType.String:
				case JsonType.Number:
				case JsonType.Integer:
				case JsonType.Boolean:
					MethodInfo simpleWriter;
					if (TryGetSimpleWriter(schema.NetType, out simpleWriter))
					{
						simple = true;
						EmitSimpleInline(schema, emit, simpleWriter, depth);
						return;
					}
					throw new ArgumentException("JSON primitive writer not found for " + schema.NetType.Name);
				default:
					throw new ArgumentException("Unknown JsonType " + schema.JsonType);
			}
		}

		/// <summary>Make the top of the stack `value` then `avail`</summary>
		static void PrepareStackComposite<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit)
		{
			var type = schema.NetType;

			// nullable methods expect Nullable<>*
			if (Nullable.GetUnderlyingType(type) != null) type = type.MakePointerType();

			using (var value = emit.DeclareLocal(type))
			{
				emit.StoreLocal(value);
				emit.LoadLocal(LOCAL_AVAIL);
				emit.LoadLocal(value);
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
				WriteConstant(emit, "null", push: true, depth: depth);
				emit.Branch(ifNull);

				emit.MarkLabel(hasValueTrue);

				emit.Call(effective.GetMethod("GetValueOrDefault", Type.EmptyTypes));

				effective = underlying;
			}

			var firstArg = writer.GetParameters()[0].ParameterType;
			if (firstArg != effective)
			{
				emit.Convert(firstArg);
			}

			emit.LoadLocal(LOCAL_DST);
			emit.LoadLocal(LOCAL_AVAIL);
			emit.Call(writer);

			if (underlying != null)
			{
				emit.MarkLabel(ifNull);
			}
		}

		static void EmitArray<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth = 0)
		{
			// FIXME null check
			// FIXME write '['

			var endLoop = emit.DefineLabel("endLoop");
			var beginLoop = emit.DefineLabel("beginLoop");

			/* var theArray = arg0; */
			var theArray = emit.DeclareLocal(schema.NetType, "theArray");
			emit.StoreLocal(theArray);

			/* var ix = 0; */
			var ix = emit.DeclareLocal<int>("ix");
			emit.LoadConstant(0);
			emit.StoreLocal(ix);

			/* goto endLoop; */
			emit.Branch(endLoop);

			/* beginLoop: */
			emit.MarkLabel(beginLoop);

			//FIXME write ','

			/* `push` theArray[ix] */
			emit.LoadLocal(theArray);
			emit.LoadLocal(ix);
			emit.LoadElement(schema.Items.NetType);

			//FIXME do something with element

			/* ix += 1; */
			emit.LoadLocal(ix);
			emit.LoadConstant(1);
			emit.Add();
			emit.StoreLocal(ix);


			/* endLoop: */
			emit.MarkLabel(endLoop);


			/* if (ix < theArray.Length) goto beginLoop; */
			emit.LoadLocal(ix);
			emit.LoadLocal(theArray);
			emit.LoadLength(schema.Items.NetType);
			emit.Convert<int>();
			emit.BranchIfLess(beginLoop);

			// FIXME write ']'

			// FIXME return bytes written

			/* return 0; */
			emit.LoadConstant(0);

			// FIXME this will probably need locals:
			//   destination
			//   available length
			//   result (maybe)
			throw new NotImplementedException();
		}

		static void EmitEnumerable<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth = 0)
		{
			// FIXME same rules as EmitObject
			throw new NotImplementedException();
		}

		static void EmitObject<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth = 0)
		{
			var underlying = Nullable.GetUnderlyingType(schema.NetType);
			var systemNullable = underlying != null;
			Sigil.Label ifNull = null;
			if (schema.Nullable)
			{
				var ifNotNull = emit.DefineLabel();
				ifNull = emit.DefineLabel();

				emit.Duplicate(); // preserve the value
				if (systemNullable)
				{
					emit.Call(schema.NetType.GetProperty("HasValue").GetMethod);
				}
				emit.BranchIfTrue(ifNotNull);
				emit.Pop(); // discard value
				emit.Pop(); // discard avail
				WriteConstant(emit, "null", push: true, depth: depth);
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
				WriteConstant(emit, "{}", depth: depth);
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
						ReturnFailed(emit, depth + 2);
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
					int loopDepth = depth + (lastMember ? 2 : 3);

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

					if (Nullable.GetUnderlyingType(member.Value.NetType) != null)
					{
						PushAddress(emit, member.Value.NetType);
					}

					// write the member value
					bool simpleWriter;
					EmitInline(member.Value, emit, loopDepth - 1, out simpleWriter);

					// negative check if simple writer was used
					// if another composite was inlined it (currently) will have bailed completely rather than push negative
					if (simpleWriter)
					{
						emit.Duplicate(); // preserve the written count

						// check if the write succeeded
						var success = emit.DefineLabel();
						emit.LoadConstant(0);
						emit.BranchIfGreater(success);
						{
							ReturnFailed(emit, loopDepth);
						}

						emit.MarkLabel(success);
					}

					emit.Duplicate(); // preserve written count
					// advance LOCAL_DST
					emit.Convert<IntPtr>();
					emit.LoadLocal(LOCAL_DST);
					emit.Add();
					emit.StoreLocal(LOCAL_DST);

					// decrement LOCAL_AVAIL
					emit.LoadConstant(-1);
					emit.Multiply();
					emit.LoadLocal(LOCAL_AVAIL);
					emit.Add();
					emit.StoreLocal(LOCAL_AVAIL);
				}
				WriteConstant(emit, "}", depth: depth + 1);
			}

			// push bytes written
			emit.LoadLocal(LOCAL_AVAIL);
			emit.Subtract();

			// subtract was in the wrong order
			// TODO verify assembly just does subtraction in the right order, if not we'll need to swap before subtract
			emit.LoadConstant(-1);
			emit.Multiply();

			if (schema.Nullable)
			{
				emit.MarkLabel(ifNull);
			}
		}

		static void EmitDictionary<T>(Schema schema, Sigil.Emit<UnsafeJson.Convert.LowWriter<T>> emit, int depth = 0)
		{
			// FIXME same rules as EmitObject
			throw new NotImplementedException();
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
