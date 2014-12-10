using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cameronism.Json
{
	internal unsafe class DelegateBuilder
	{
		internal static bool UseSigilVerify = true;

		// call one of the static create methods
		private DelegateBuilder () { }

		#region instance state
		DestinationType Destination;
		int Depth;
		int NameCount;

		Sigil.Local LocalDestination;
		Sigil.Local LocalAvailable;
		Sigil.NonGeneric.Emit Emit;
		#endregion

		public static Sigil.NonGeneric.Emit CreateStream<T>(Schema schema)
		{
			return Create<T>(schema, DestinationType.Stream);
		}

		public static Sigil.NonGeneric.Emit CreatePointer<T>(Schema schema)
		{
			return Create<T>(schema, DestinationType.Pointer);
		}

		static Sigil.NonGeneric.Emit Create<T>(Schema schema, DestinationType destination)
		{
			ValueWriter simpleWriter = null;

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
					if (ValueWriter.TryGetWriter(schema.NetType, destination, out simpleWriter))
					{
						break;
					}
					throw new ArgumentException("JSON primitive writer not found for " + schema.NetType.Name);
				default:
					throw new ArgumentException("Unknown JsonType " + schema.JsonType);
			}

			var builder = new DelegateBuilder
			{
				Destination = destination,
			};

			Sigil.NonGeneric.Emit emit;
			string methodName = schema.NetType.Name + "_toJSON";
			if (destination == DestinationType.Pointer)
			{
				emit = Sigil.NonGeneric.Emit.NewDynamicMethod(typeof(int), new[] { 
					typeof(T).MakeByRefType(),      // value
					typeof(byte).MakePointerType(), // destination
					typeof(int),                    // available
				}, methodName, doVerify: UseSigilVerify);
			}
			else if (destination == DestinationType.Stream)
			{
				emit = Sigil.NonGeneric.Emit.NewDynamicMethod(typeof(void), new[] { 
					typeof(T).MakeByRefType(),      // value
					typeof(byte).MakePointerType(), // buffer
					typeof(System.IO.Stream),       // destination
					typeof(byte).MakeArrayType(),   // buffer
				}, methodName, doVerify: UseSigilVerify);
			}
			else
			{
				throw new ArgumentException();
			}
			builder.Emit = emit;

			if (simpleWriter != null)
			{
				if (destination == DestinationType.Stream)
				{
					if (!simpleWriter.MaxLength.HasValue)
					{
						// special case writers that might have to Flush multiple times
						builder.EmitSimpleStream(schema, simpleWriter);
						return emit;
					}

					builder.LocalAvailable = emit.DeclareLocal(typeof(int), "available");

					builder.PushTotalAvailable();
					emit.StoreLocal(builder.LocalAvailable);
				}

				emit.LoadArgument(0);
				builder.LoadIndirect(schema.NetType); // deref argument 0

				builder.EmitSimpleComplete(schema, simpleWriter);
				return emit;
			}

			Sigil.Local local;
			// local for dst
			builder.LocalDestination = local = emit.DeclareLocal(typeof(byte*), "destination");
			emit.LoadArgument(1);
			emit.StoreLocal(local);

			// local for avail
			builder.LocalAvailable = local = emit.DeclareLocal(typeof(int), "available");
			builder.PushTotalAvailable();
			emit.StoreLocal(local);

			bool pushResult = destination == DestinationType.Pointer;

			// CRITICAL before calling any composite methods top of stack must be `value` followed by `available` when pushResult is true
			if (pushResult) emit.LoadLocal(local);

			emit.LoadArgument(0);
			builder.LoadIndirect(schema.NetType); // deref argument 0


			if (schema.JsonType == JsonType.Array)
			{
				if (schema.NetType.IsArray)
				{
					builder.EmitArray(schema, pushResult: pushResult);
				}
				else
				{
					builder.EmitEnumerable(schema, pushResult: pushResult);
				}
			}
			else if (schema.JsonType == JsonType.Object)
			{
				if (schema.Keys != null)
				{
					builder.EmitDictionary(schema, pushResult: pushResult);
				}
				else
				{
					builder.EmitObject(schema, pushResult: pushResult);
				}
			}

			if (destination == DestinationType.Stream) builder.Flush(writtenTop: false);
			emit.Return();
			return emit;
		}

		/// <summary>Must be called with value on top of the stack</summary>
		void EmitInline(Schema schema)
		{
			if (Nullable.GetUnderlyingType(schema.NetType) != null)
			{
				PushAddress(schema.NetType);
			}

			switch (schema.JsonType)
			{
				case JsonType.Array:
					if (schema.NetType.IsArray)
					{
						EmitArray(schema, pushResult: false);
					}
					else
					{
						EmitEnumerable(schema, pushResult: false);
					}
					break;
				case JsonType.Object:
					if (schema.Keys != null)
					{
						EmitDictionary(schema, pushResult: false);
					}
					else
					{
						EmitObject(schema, pushResult: false);
					}
					break;
				case JsonType.String:
				case JsonType.Number:
				case JsonType.Integer:
				case JsonType.Boolean:
					ValueWriter simpleWriter;
					if (ValueWriter.TryGetWriter(schema.NetType, Destination, out simpleWriter))
					{
						EmitSimpleInline(schema, simpleWriter);
						return;
					}
					throw new ArgumentException("JSON primitive writer not found for " + schema.NetType.Name);
				default:
					throw new ArgumentException("Unknown JsonType " + schema.JsonType);
			}
		}

		void PushTotalAvailable()
		{
			if (Destination == DestinationType.Stream)
			{
				Emit.LoadArgument(ARG_BUFFER);
				Emit.LoadLength(typeof(byte));
				Emit.Convert<int>();
			}
			else
			{
				Emit.LoadArgument(2);
			}
		}

		void LoadIndirect(Type effective)
		{
			if (effective.IsClass || effective.IsInterface)
			{
				Emit.LoadIndirect(effective);
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
						Emit.LoadObject(effective);
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
						Emit.LoadIndirect(effective);
						break;
				}
			}
		}


		const ushort ARG_POINTER = 1;
		const ushort ARG_STREAM = 2;
		const ushort ARG_BUFFER = 3;

		void Flush(bool resetAvailable = false, bool writtenTop = true)
		{
			if (writtenTop)
			{
				// available -= pop()
				Emit.LoadConstant(-1);
				Emit.Multiply();
				Emit.LoadLocal(LocalAvailable);
				Emit.Add();
				Emit.StoreLocal(LocalAvailable);
			}


			//stream.Write(buffer, 0, buffer.Length - available);

			Emit.LoadArgument(ARG_STREAM);
			Emit.LoadArgument(ARG_BUFFER);
			Emit.LoadConstant(0);

			// buffer.Length - available
			PushTotalAvailable();
			Emit.LoadLocal(LocalAvailable);
			Emit.Subtract();
			// call .Write
			Emit.CallVirtual(typeof(Stream).GetMethod("Write", BindingFlags.Public | BindingFlags.Instance));

			if (resetAvailable)
			{
				// available = buffer.Length;
				Emit.LoadArgument(ARG_BUFFER);
				Emit.LoadLength(typeof(byte));
				Emit.Convert<int>();
				Emit.StoreLocal(LocalAvailable);

				// dst = arg 1
				Emit.LoadArgument(ARG_POINTER);
				Emit.StoreLocal(LocalDestination);
			}
		}

		static MethodInfo WriteNull = typeof(Cameronism.Json.Serializer).GetMethod("WriteNull", BindingFlags.NonPublic | BindingFlags.Static);

		void EmitSimpleComplete(Schema schema, ValueWriter writer)
		{
			var effective = schema.NetType;
			Sigil.Label theEnd = null;

			if (Nullable.GetUnderlyingType(effective) != null)
			{
				Emit.Duplicate(); // prepare argument 0 for GetValueOrDefault call

				Emit.Call(effective.GetProperty("HasValue").GetGetMethod());

				var hasValueTrue = Emit.DefineLabel("HasValue_true");
				Emit.BranchIfTrue(hasValueTrue);
				Emit.Pop(); // discard argument 0
				// byte* destination
				Emit.LoadArgument(1);
				// available
				if (Destination == DestinationType.Stream) Emit.LoadLocal(LocalAvailable);
				else Emit.LoadArgument(2);
				// .WriteNull
				Emit.Call(WriteNull);

				if (Destination == DestinationType.Stream) Flush();

				theEnd = Emit.DefineLabel("theEnd");
				Emit.Branch(theEnd);

				Emit.MarkLabel(hasValueTrue);

				Emit.Call(effective.GetMethod("GetValueOrDefault", Type.EmptyTypes));

				effective = Nullable.GetUnderlyingType(effective);
			}

			CallWriter(writer, effective, false);

			if (Destination == DestinationType.Stream) Flush();

			if (theEnd != null) Emit.MarkLabel(theEnd);

			Emit.Return();
		}

		// never pushes result
		void EmitSimpleInline(Schema schema, ValueWriter writer)
		{
			var effective = schema.NetType;
			Sigil.Label ifNull = null;

			var underlying = Nullable.GetUnderlyingType(effective);
			if (underlying != null)
			{
				ifNull = DefineLabel("ifNull");
				var hasValueTrue = DefineLabel("hasValueTrue");

				Emit.Duplicate(); // preserve value

				Emit.Call(effective.GetProperty("HasValue").GetGetMethod());

				Emit.BranchIfTrue(hasValueTrue);
				Emit.Pop(); // discard value
				WriteConstant("null", push: false);
				Emit.Branch(ifNull);

				Emit.MarkLabel(hasValueTrue);

				Emit.Call(effective.GetMethod("GetValueOrDefault", Type.EmptyTypes));

				effective = underlying;
			}

			if (writer.MaxLength.HasValue && Destination == DestinationType.Stream)
			{
				// flush if available < writer.MaxLength
				var afterFlush = DefineLabel("afterFlush");
				Emit.LoadLocal(LocalAvailable);
				Emit.LoadConstant(writer.MaxLength.Value);
				Emit.BranchIfGreaterOrEqual(afterFlush);
				Flush(writtenTop: false, resetAvailable: true);
				Emit.MarkLabel(afterFlush);
			}

			CallWriter(writer, effective, true);

			// simple writers must flush to stream directly
			if (Destination == DestinationType.Pointer)
			{
				Emit.Duplicate(); // preserve the written count

				// check if the write succeeded
				var success = DefineLabel("success");
				Emit.LoadConstant(0);
				Emit.BranchIfGreater(success);
				{
					Depth++;
					ReturnFailed();
					Depth--;
				}

				Emit.MarkLabel(success);
			}

			if (Destination == DestinationType.Pointer || writer.MaxLength.HasValue)
			{
				Emit.Duplicate(); // preserve written count

				// advance LocalDestination
				Emit.Convert<IntPtr>();
				Emit.LoadLocal(LocalDestination);
				Emit.Add();
				Emit.StoreLocal(LocalDestination);


				// Emit.Duplicate(); // preserve written count

				// decrement LocalAvailable
				Emit.LoadConstant(-1);
				Emit.Multiply();
				Emit.LoadLocal(LocalAvailable);
				Emit.Add();
				Emit.StoreLocal(LocalAvailable);
			}


			if (underlying != null)
			{
				Emit.MarkLabel(ifNull);
			}
		}

		void CallWriter(ValueWriter writer, Type effective, bool useLocals)
		{
			if (writer.MethodInfo == null)
			{
				if (effective == typeof(System.Net.IPAddress))
				{
					EmitIPAddress(useLocals);
					return;
				}
				throw new NotImplementedException();
			}

			if (Destination == DestinationType.Pointer || writer.MaxLength.HasValue)
			{
				var firstArg = writer.MethodInfo.GetParameters()[0].ParameterType;
				if (firstArg != effective)
				{
					Emit.Convert(firstArg);
				}

				if (useLocals)
				{
					Emit.LoadLocal(LocalDestination);
					Emit.LoadLocal(LocalAvailable);
				}
				else
				{
					Emit.LoadArgument(1);
					if (Destination == DestinationType.Pointer)
					{
						Emit.LoadArgument(2);
					}
					else
					{
						Emit.LoadLocal(LocalAvailable);
					}
				}
			}
			else
			{
				Emit.LoadArgument(ARG_STREAM);
				Emit.LoadArgument(ARG_BUFFER);
				Emit.LoadLocalAddress(LocalAvailable);
				Emit.LoadLocalAddress(LocalDestination);
			}

			Emit.Call(writer.MethodInfo);
		}

		void EmitSimpleStream(Schema schema, ValueWriter writer)
		{
			// push *value
			Emit.LoadArgument(0);
			LoadIndirect(schema.NetType);

			// push stream
			Emit.LoadArgument(ARG_STREAM);

			// push buffer
			Emit.LoadArgument(ARG_BUFFER);

			// push &available
			LocalAvailable = Emit.DeclareLocal(typeof(int), "available");
			PushTotalAvailable();
			Emit.StoreLocal(LocalAvailable);
			Emit.LoadLocalAddress(LocalAvailable);

			// push &ptr;
			Emit.LoadArgumentAddress(ARG_POINTER);

			// call writer
			Emit.Call(writer.MethodInfo);

			Flush(resetAvailable: false, writtenTop: false);

			Emit.Return();
		}

		void PushDestinationAvailable(bool useLocals)
		{
			if (useLocals)
			{
				Emit.LoadLocal(LocalDestination);
				Emit.LoadLocal(LocalAvailable);
			}
			else
			{
				Emit.LoadArgument(1);
				if (Destination == DestinationType.Pointer)
				{
					Emit.LoadArgument(2);
				}
				else
				{
					Emit.LoadLocal(LocalAvailable);
				}
			}
		}

		void EmitIPAddress(bool useLocals)
		{
			var ip = typeof(System.Net.IPAddress);

			var notNull = DefineLabel("notNull");
			var done = DefineLabel("done");

			// preserve ip
			Emit.Duplicate();
			Emit.BranchIfTrue(notNull);

			// value is null
			{
				Emit.Pop(); // discard null

				PushDestinationAvailable(useLocals);
				Emit.Call(typeof(Serializer).GetMethod("WriteNull", BindingFlags.Static | BindingFlags.NonPublic));

				Emit.Branch(done);
			}

			Emit.MarkLabel(notNull);

			// value is not null
			{
				var v6 = DefineLabel("ipv6");

				Emit.Duplicate();
				Emit.CallVirtual(ip.GetProperty("AddressFamily").GetMethod);
				Emit.LoadConstant((int)System.Net.Sockets.AddressFamily.InterNetwork);
				Emit.UnsignedBranchIfNotEqual(v6);

				// if v4
				{
					Emit.LoadField(ip.GetField("m_Address", BindingFlags.Instance | BindingFlags.NonPublic));
					PushDestinationAvailable(useLocals);
					Emit.Call(typeof(Serializer).GetMethod("WriteIPv4", BindingFlags.Static | BindingFlags.NonPublic));

					Emit.Branch(done);
				}
				
				// if not v4 (assume v6)
				Emit.MarkLabel(v6);
				{
					Emit.LoadField(ip.GetField("m_Numbers", BindingFlags.Instance | BindingFlags.NonPublic));
					PushDestinationAvailable(useLocals);
					Emit.Call(typeof(Serializer).GetMethod("WriteIPv6", BindingFlags.Static | BindingFlags.NonPublic));
				}
			}

			Emit.MarkLabel(done);
		}

		void EmitArray(Schema schema, bool pushResult = false)
		{
			var ifNotNull = DefineLabel("ifNotNull");
			var ifNull = DefineLabel("ifNull");

			Emit.Duplicate(); // preserve the value
			Emit.BranchIfTrue(ifNotNull);
			Emit.Pop(); // discard value
			if (pushResult) Emit.Pop(); // discard avail
			WriteConstant("null", push: pushResult);
			Emit.Branch(ifNull);

			Emit.MarkLabel(ifNotNull);

			int localDepth = pushResult ? 2 : 1;
			Depth += localDepth;

			// begin array
			WriteConstant("[");

			var endLoop = DefineLabel("endLoop");
			var beginLoop = DefineLabel("beginLoop");

			/* var ix = 0; */
			using (var ix = Emit.DeclareLocal<int>(GetName("ix")))
			{
				Emit.LoadConstant(0);
				Emit.StoreLocal(ix);

				/* goto endLoop; */
				Emit.Branch(endLoop);

				/* beginLoop: */
				Emit.MarkLabel(beginLoop);

				// write ','
				var afterComma = DefineLabel("afterComma");
				Emit.LoadLocal(ix);
				Emit.BranchIfFalse(afterComma);
				WriteConstant(",");

				Emit.MarkLabel(afterComma);

				/* `push` theArray[ix] */
				Emit.Duplicate(); // preserve array value
				Emit.LoadLocal(ix);
				Emit.LoadElement(schema.Items.NetType);

				// write the element
				EmitInline(schema.Items);

				/* ix += 1; */
				Emit.LoadLocal(ix);
				Emit.LoadConstant(1);
				Emit.Add();
				Emit.StoreLocal(ix);


				/* endLoop: */
				Emit.MarkLabel(endLoop);


				/* if (theArray.Length > ix) goto beginLoop; */
				Emit.Duplicate(); // preserve array value
				Emit.LoadLength(schema.Items.NetType);
				Emit.Convert<int>();
				Emit.LoadLocal(ix);
				Emit.BranchIfGreater(beginLoop);
			}

			// discard array value
			Emit.Pop();

			// end array
			Depth--;
			WriteConstant("]");
			Depth++;


			if (pushResult)
			{
				// push bytes written
				Emit.LoadLocal(LocalAvailable);
				Emit.Subtract();
			}

			if (schema.Nullable)
			{
				Emit.MarkLabel(ifNull);
			}

			Depth -= localDepth;
		}
		void EmitEnumerable(Schema schema, bool pushResult = false)
		{
			EmitEnumerable(schema, pushResult, isArray: true);
		}

		void EmitEnumerable(Schema schema, bool pushResult, bool isArray)
		{
			var enumerable = EnumerableInfo.FindMethods(schema.NetType);
			MethodInfo getKey = null;
			MethodInfo getValue = null;
			if (!isArray)
			{
				getKey = enumerable.get_Current.ReturnType.GetProperty("Key").GetMethod;
				getValue = enumerable.get_Current.ReturnType.GetProperty("Value").GetMethod;
			}

			var ifNotNull = DefineLabel("ifNotNull");
			var ifNull = DefineLabel("ifNull");

			Emit.Duplicate(); // preserve the value
			Emit.BranchIfTrue(ifNotNull);
			Emit.Pop(); // discard value
			if (pushResult) Emit.Pop(); // discard avail
			WriteConstant("null", push: pushResult);
			Emit.Branch(ifNull);

			Emit.MarkLabel(ifNotNull);

			int localDepth = pushResult ? 2 : 1;
			Depth += localDepth;

			// begin array
			WriteConstant(isArray ? "[" : "{");

			var loopBottom = DefineLabel("loopBottom");
			var loopTop = DefineLabel("loopTop");
			var closeArray = DefineLabel("closeArray");

			
			// call GetEnumerator
			CallCorrectly(enumerable.GetEnumerator, schema.NetType);
			var enumeratorType = enumerable.GetEnumerator.ReturnType;

			// use the pointer for enumerator structs
			if (enumeratorType.IsValueType)
			{
				PushAddress(enumeratorType);
			}

			// FIXME need to have some support for Dispose
			//var exceptionBlock = enumerable.Dispose == null ? null : Emit.BeginExceptionBlock();

			// unroll the first iteration so the loop can always write the comma

			/* if (!enumerator.MoveNext()) goto closeArray; */
			Emit.Duplicate(); // preserve enumerator
			CallCorrectly(enumerable.MoveNext, enumeratorType);
			Emit.BranchIfFalse(closeArray);

			// push enumerator.Current
			Emit.Duplicate(); // preserve enumerator
			CallCorrectly(enumerable.get_Current, enumeratorType);

			if (!isArray)
			{
				// we've got a KeyValuePair<,>
				PushAddress(getValue.DeclaringType);

				Emit.Duplicate(); // preserve KeyValuePair<,>*
				Emit.Call(getKey);
				// write the first key
				Depth++;
				EmitInline(schema.Keys);
				WriteConstant(":");
				Depth--;
				Emit.Call(getValue);
			}

			// write the first element
			EmitInline(schema.Items);

			Emit.Branch(loopBottom);
			Emit.MarkLabel(loopTop);

			// write ','
			WriteConstant(",");

			// push enumerator.Current
			Emit.Duplicate();// preserve enumerator
			CallCorrectly(enumerable.get_Current, enumeratorType);

			if (!isArray)
			{
				// we've got a KeyValuePair<,>
				PushAddress(getValue.DeclaringType);

				Emit.Duplicate(); // preserve KeyValuePair<,>*
				Emit.Call(getKey);
				// write the key
				Depth++;
				EmitInline(schema.Keys);
				WriteConstant(":");
				Depth--;
				Emit.Call(getValue);
			}

			// write the element
			EmitInline(schema.Items);


			/* endLoop: */
			Emit.MarkLabel(loopBottom);

			/* if (enumerator.MoveNext()) goto beginLoop; */
			Emit.Duplicate(); // preserve enumerator
			CallCorrectly(enumerable.MoveNext, enumeratorType);
			Emit.BranchIfTrue(loopTop);


			Emit.MarkLabel(closeArray);

			// this is where the finally block would go
			//if (exceptionBlock != null)
			//{
			//	var f = Emit.BeginFinallyBlock(exceptionBlock);
			//	CallCorrectly(enumerable.Dispose, enumeratorType);
			//	Emit.EndFinallyBlock(f);
			//	Emit.EndExceptionBlock(exceptionBlock);
			//}
			//else

			Emit.Pop(); // discard enumerator

			// end array
			Depth--;
			WriteConstant(isArray ? "]" : "}");
			Depth++;


			if (pushResult)
			{
				// push bytes written
				Emit.LoadLocal(LocalAvailable);
				Emit.Subtract();
			}

			if (schema.Nullable)
			{
				Emit.MarkLabel(ifNull);
			}

			Depth -= localDepth;
		}

		void CallCorrectly(MethodInfo mi, Type instanceType)
		{
			if (mi.IsVirtual)
			{
				Emit.CallVirtual(mi, constrained: instanceType != null && instanceType.IsValueType ? instanceType : null);
			}
			else
			{
				Emit.Call(mi);
			}
		}

		void EmitObject(Schema schema, bool pushResult = false)
		{
			// merge consecutive constant strings
			var spans = new List<KeyValuePair<string, Schema>>();
			for (int i = 0; i < schema.Members.Count; i++)
			{
				Append(spans, 
					(i == 0 ? "{\"" : ",\"") +
					Escape(schema.Members[i].Key) +
					"\":");

				string constant = null;
				var member = schema.Members[i].Value;
				var prop = member.PropertyInfo;
				if (prop != null)
				{
					constant = ConstantMethods.TryGetJson(prop.GetMethod, schema.NetType);
				}

				if (constant != null)
				{
					Append(spans, constant);
				}
				else
				{
					spans.Add(new KeyValuePair<string, Schema>(null, member));
				}

				if (i == schema.Members.Count - 1) Append(spans, "}");
			}

			var underlying = Nullable.GetUnderlyingType(schema.NetType);
			var systemNullable = underlying != null;
			Sigil.Label ifNull = null;
			bool anyMembers = spans.Any(span => span.Value != null);
			if (schema.Nullable)
			{
				var ifNotNull = DefineLabel("IfNotNull");
				ifNull = DefineLabel("ifNull");

				if (anyMembers)
				{
					Emit.Duplicate(); // preserve the value
				}
				if (systemNullable)
				{
					Emit.Call(schema.NetType.GetProperty("HasValue").GetMethod);
				}
				Emit.BranchIfTrue(ifNotNull);
				if (anyMembers) Emit.Pop(); // discard value
				if (pushResult) Emit.Pop(); // discard avail
				WriteConstant("null", push: pushResult);
				Emit.Branch(ifNull);

				Emit.MarkLabel(ifNotNull);
				if (systemNullable)
				{
					Emit.Call(schema.NetType.GetMethod("GetValueOrDefault", Type.EmptyTypes));
				}
			}

			if (schema.Members.Count == 0)
			{
				if (pushResult) Depth++;
				WriteConstant("{}");
				if (pushResult) Depth--;
			}
			else
			{
				int lastNonConstant = -1;
				int minLength = 0;

				// find last non constant and min length
				for (int i = 0; i < spans.Count; i++)
				{
					var span = spans[i];
					if (span.Key != null)
					{
						minLength += Encoding.UTF8.GetByteCount(span.Key);
					}
					else
					{
						lastNonConstant = i;
						minLength += span.Value.CalculateMinimumLength(considerNullMembers: true);
					}
				}

				for (int i = 0; i < spans.Count; i++)
				{
					var span = spans[i];

					// assert we've got enough
					var enoughAvailable = DefineLabel("enoughAvailable");
					Emit.LoadLocal(LocalAvailable);
					Emit.LoadConstant(minLength);
					Emit.BranchIfGreaterOrEqual(enoughAvailable);
					{
						int extra = pushResult ? 2 : 1;
						if (i > lastNonConstant) extra--;
						Depth += extra;
						ReturnFailed();
						Depth -= extra;
					}

					Emit.MarkLabel(enoughAvailable);

					if (span.Key != null)
					{
						int written = WriteConstant(span.Key, assertAvailable: false);
						minLength -= written;
						continue;
					}

					var member = span.Value;
					minLength -= member.CalculateMinimumLength(considerNullSelf: true);

					var lastMember = i == lastNonConstant;
					if (!lastMember)
					{
						// sanity check minLength
						if (minLength < 6) // ,"a":0
						{
							// FIXME use/create appropriate Exception type
							throw new InvalidOperationException("Minimum length calculation failed");
						}

						// preserve value except last time through the loop
						Emit.Duplicate();
					}
					int loopDepth = (lastMember ? 1 : 2) - (pushResult ? 0 : 1);
					Depth += loopDepth;

					// get the member value
					if (member.FieldInfo != null)
					{
						Emit.LoadField(member.FieldInfo);
					}
					else
					{
						if (schema.NetType.IsValueType)
						{
							PushAddress(systemNullable ? underlying : schema.NetType);
						}
						Emit.Call(member.PropertyInfo.GetMethod);
					}

					// write the member value
					EmitInline(member);

					Depth -= loopDepth;

					// if we ran out of room inner writer will have bailed completely rather than push negative (currently)

					// we probably won't care about result until we create more fine grained estimates when out of room
				}
			}

			if (pushResult)
			{
				// push bytes written
				Emit.LoadLocal(LocalAvailable);
				Emit.Subtract();
			}

			if (schema.Nullable)
			{
				Emit.MarkLabel(ifNull);
			}
		}

		static void Append(List<KeyValuePair<string, Schema>> list, string value)
		{
			var last = list.LastOrDefault();
			if (last.Key == null)
			{
				list.Add(new KeyValuePair<string, Schema>(value, null));
			}
			else
			{
				list[list.Count - 1] = new KeyValuePair<string, Schema>(last.Key + value, null);
			}
		}

		static string Escape(string value)
		{
			// FIXME
			return value;
		}

		void EmitDictionary(Schema schema, bool pushResult = false)
		{
			if (schema.Keys.JsonType != JsonType.String) throw new NotImplementedException();

			EmitEnumerable(schema, pushResult, isArray: false);
		}

		void PushAddress(Type type)
		{
			using (var copy = Emit.DeclareLocal(type, GetName("tmp")))
			{
				Emit.StoreLocal(copy);
				Emit.LoadLocalAddress(copy);
			}
		}

		/// <summary>
		/// Write a constant value
		/// </summary>
		/// <param name="s"></param>
		/// <param name="assertAvailable"></param>
		/// <param name="push">Push the written byte count on success</param>
		/// <returns></returns>
		int WriteConstant(string s, bool assertAvailable = true, bool push = false)
		{
			// ASSUMPTION: the string does not require JSON escaping
			var bytes = Encoding.UTF8.GetBytes(s);

			if (assertAvailable)
			{
				var enoughAvailable = DefineLabel("enoughAvailable");
				Emit.LoadLocal(LocalAvailable);
				Emit.LoadConstant(bytes.Length);
				Emit.BranchIfGreaterOrEqual(enoughAvailable);
				{
					ReturnFailed();
				}

				Emit.MarkLabel(enoughAvailable);
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

				Emit.LoadLocal(LocalDestination);
				Emit.Duplicate();
				Emit.LoadConstant(1);
				Emit.Convert<IntPtr>();
				Emit.Add();
				Emit.StoreLocal(LocalDestination);
				Emit.LoadConstant(b);
				Emit.StoreIndirect<byte>();
			}

			// avail -= bytes.Length
			Emit.LoadLocal(LocalAvailable);
			Emit.LoadConstant(bytes.Length);
			Emit.Subtract();
			Emit.StoreLocal(LocalAvailable);

			if (push)
			{
				Emit.LoadConstant(bytes.Length);
			}

			return bytes.Length;
		}

		void ReturnFailed()
		{
			if (Destination == DestinationType.Pointer)
			{
				for (int i = 0; i < Depth; i++) Emit.Pop();

				// ask for double
				Emit.LoadArgument(2); // avail
				Emit.LoadConstant(-2);
				Emit.Multiply();
				Emit.Return();
			}
			else
			{
				Flush(resetAvailable: true, writtenTop: false);
			}
		}

		string GetName(string prefix)
		{
			return prefix + "_" + (NameCount++);
		}

		Sigil.Label DefineLabel(string prefix)
		{
			return Emit.DefineLabel(GetName(prefix));
		}
	}
}
