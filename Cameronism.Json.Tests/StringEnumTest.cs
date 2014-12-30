using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cameronism.Json.Tests
{
	public unsafe class StringEnumTest
	{
		enum Skippy
		{
			Foo = 1,
			Bar = 3,
			Bop = 5,
			Baz = 7,
		}

		enum Sparse : uint
		{
			v1 = 1 << 0,
			v2 = 1 << 1,
			v3 = 1 << 2,
			v4 = 1 << 3,
			v5 = 1 << 4,
			v6 = 1 << 5,
			v7 = 1 << 6,
			v8 = 1 << 7,
			v9 = 1 << 8,
			v10 = 1 << 9,
			v11 = 1 << 10,
			v12 = 1 << 11,
			v13 = 1 << 12,
			v14 = 1 << 13,
			v15 = 1 << 14,
			v16 = 1 << 15,
			v17 = 1 << 16,
			v18 = 1 << 17,
			v19 = 1 << 18,
			v20 = 1 << 19,
			v21 = 1 << 20,
			v22 = 1 << 21,
			v23 = 1 << 22,
			v24 = 1 << 23,
			v25 = 1 << 24,
			v26 = 1 << 25,
			v27 = 1 << 26,
			v28 = 1 << 27,
			v29 = 1 << 28,
			v30 = 1 << 29,
			v31 = 1 << 30,
			v32 = ((uint)1 << 31),
		}

		enum Negatives : int
		{
			NegativeOne = -1,
			One = 1,
			Min = int.MinValue,
			Max = int.MaxValue,
			\uFF72\u3093a = short.MinValue - 1,
			\uFF72\u3093aa = short.MinValue - 2,
		}

		enum FakeStatusCodes : uint
		{
			OK,
			Bad,
			SoSew,
			Grrrrreat,
		}

		enum Empty { }

		#region theory
		[Flags]
		enum FunWithFlags0
		{
			Your = 1,
			Host = 2,
			Sheldon = 4,
			Cooper = 8,

			YourHostSheldon = 7,
			//SheldonCooper = 12,
		}
		[Flags]
		enum FunWithFlags1
		{
			Your = 1,
			Host = 2,
			Sheldon = 4,
			Cooper = 8,

			//YourHost = 3,
			YourSheldon = 5,
		}
		[Flags]
		enum FunWithFlags2 : ulong
		{
			Your = 1,
			Host = 2,
			Sheldon = 4,
			Cooper = long.MaxValue,

			//YourSheldon = 5,
			HostSheldon = 6,
		}
		[Flags]
		enum FunWithFlags3 : sbyte
		{
			Your = 1,
			Host = 2,
			Sheldon = 4,
			Cooper = -128,
		}
		[Flags]
		enum FunWithFlags4
		{
			Your = 4,
			Host = 8,
			Sheldon = 16,
			Cooper = 32,
		}
		[Flags]
		enum FunWithFlags5 : byte
		{
			Your = 4,
			Host = 8,
			Sheldon = 16,
			Cooper = 32,
		}
		[Flags]
		enum FunWithFlags6 : byte
		{
			Your = 1,
			Host = 2,
		}
		#endregion

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public void SkippyEnum()
		{
			var sb = new StringBuilder();
			ApproveIndexedEnum<Skippy>(sb);
			sb.AppendLine();
			sb.AppendLine();
			ApproveSortedEnum<Skippy>(sb);
			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public void SparseEnum()
		{
			var sb = new StringBuilder();
			ApproveSortedEnum<Sparse>(sb);

			ApproveSortedEnum<Negatives>(sb, expectGenerated: false);
			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public void VerboseEnum()
		{
			var sb = new StringBuilder();
			ApproveVerboseEnum<DayOfWeek>(sb);
			sb.AppendLine();
			sb.AppendLine();
			ApproveVerboseEnum<Sparse>(sb);
			sb.AppendLine();
			sb.AppendLine();
			ApproveVerboseEnum<Skippy>(sb);
			sb.AppendLine();
			sb.AppendLine();
			ApproveVerboseEnum<Negatives>(sb);

			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public void DayOfWeekEnum()
		{
			var sb = new StringBuilder();
			ApproveIndexedEnum<DayOfWeek>(sb);
			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		public void ApproveIndexedEnum<T>(StringBuilder sb)
		{
			ApproveEnumCommon<T>("Indexed", sb, true, StringEnum.GenerateIndexedLookup, delegate(StringEnum.Lookup lookup, ulong value, out byte* str, out int length)
			{
				lookup.FindIndexed(value, out str, out length);
			});
		}

		public void ApproveSortedEnum<T>(StringBuilder sb, bool expectGenerated = true)
		{
			ApproveEnumCommon<T>("Sorted", sb, expectGenerated, StringEnum.GenerateSortedLookup, delegate(StringEnum.Lookup lookup, ulong value, out byte* str, out int length)
			{
				lookup.FindSequential(value, out str, out length);
			});
		}

		void ApproveVerboseEnum<T>(StringBuilder sb, bool expectGenerated = true)
		{
			ApproveEnumCommon<T>("Verbose", sb, expectGenerated, StringEnum.GenerateVerboseLookup, delegate(StringEnum.Lookup lookup, ulong value, out byte* str, out int length)
			{
				lookup.FindVerbose(value, out str, out length);
			});
		}

		delegate StringEnum.Lookup? GenerateLookup(KeyValuePair<ulong, string>[] values, byte* ptr, int length);
		delegate void FindString(StringEnum.Lookup lookup, ulong value, out byte* str, out int length);

		void ApproveEnumCommon<T>(string label, StringBuilder sb, bool expectGenerated, GenerateLookup generateLookup, FindString findString)
		{
			var buffer = new byte[1024];
			for (int i = 0; i < buffer.Length; i++) buffer[i] = 255;
			var values = StringEnum.SortValues(typeof(T));

			fixed (byte* ptr = buffer)
			{
				var lookup = generateLookup(values, ptr, buffer.Length);

				if (!expectGenerated)
				{
					Assert.False(lookup.HasValue, "lookup must not have been generated");
					return;
				}

				Assert.True(lookup.HasValue, "lookup must have been generated");
				var stringStart = (int)(lookup.Value.StringStart - ptr);


				sb.AppendLine("# " + label + " " + SchemaTest.HumanName(typeof(T)));
				sb.AppendLine();
				sb.AppendLine(String.Format("GetLookupType: {0}", StringEnum.GetLookupType(values)));
				sb.AppendLine(String.Format("Values count: {0}", values.Length));
				sb.AppendLine(String.Format("Max value: {0}", values.LastOrDefault().Key));
				sb.AppendLine();
				sb.AppendLine("## Lookups");
				Util.Hex.Dump(sb, buffer.Take(stringStart));
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("## Strings");
				Util.Hex.Dump(sb, buffer.Skip(stringStart).Take((int)lookup.Value.StringLength));


				bool signed = false;
				switch (Type.GetTypeCode(typeof(T)))
				{
					case TypeCode.SByte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
						signed = true;
						break;
				}

				var chars = new char[64];
				foreach (T dow in Enum.GetValues(typeof(T)))
				{
					byte* str;
					int len;
					ulong enumValue = signed ? (ulong)Convert.ToInt64(dow) : Convert.ToUInt64(dow);
					findString(lookup.Value, enumValue, out str, out len);
					Assert.True(str != null, "string must not be null");
					var dowString = dow.ToString();
					Assert.Equal(Encoding.UTF8.GetByteCount(dowString), len);

					string actualString;
					fixed (char* ch = chars)
					{
						var strLen = Encoding.UTF8.GetChars(str, len, ch, chars.Length);
						actualString = new string(ch, 0, strLen);
					}
					Assert.Equal(dowString, actualString);
				}
			}
		}

		[Fact]
		public void SortEnums()
		{
			var rand = new Random(42);
			var values = Enumerable.Range(0, 128).OrderBy(_ => rand.Next()).ToArray();
			var sorted = StringEnum.SortValues(values, true);
			Array.Sort(values);

			Assert.Equal(values, sorted.Select(kvp => (int)kvp.Key));

			var empty = new KeyValuePair<ulong, string>[0];
			var buffer = new byte[256];
			fixed (byte* ptr = buffer)
			{
				Assert.Null(StringEnum.GenerateIndexedLookup(empty, ptr, buffer.Length));
				Assert.Null(StringEnum.GenerateIndexedLookup(sorted, ptr, buffer.Length));

				Assert.Null(StringEnum.GenerateSortedLookup(empty, ptr, buffer.Length));
				Assert.Null(StringEnum.GenerateSortedLookup(sorted, ptr, buffer.Length));

				Assert.Null(StringEnum.GenerateVerboseLookup(empty, ptr, buffer.Length));
				Assert.Null(StringEnum.GenerateVerboseLookup(sorted, ptr, buffer.Length));
			}
		}

		[Fact]
		public void TryEnumLookup()
		{
			var enumTypes = new[] 
			{
				typeof(Skippy),
				typeof(Sparse),
				typeof(Negatives),
				typeof(DayOfWeek),
				typeof(FakeStatusCodes),
				typeof(Empty),
				typeof(FunWithFlags0),
				typeof(FunWithFlags1),
				typeof(FunWithFlags2),
				typeof(FunWithFlags3),
				typeof(FunWithFlags4),
				typeof(FunWithFlags5),
				typeof(FunWithFlags6),
			}
			.Concat(typeof(Util.CorEnums).GetNestedTypes())
			.ToList();

			var buffer = new byte[256];
			var newtEnumConverter = new Newtonsoft.Json.Converters.StringEnumConverter();
			var values = Enumerable.Range(0, 256).Select(i => (ulong)i).ToArray();

			foreach (var enumType in enumTypes)
			{
				var lookup = StringEnum.GetCachedLookup(enumType);

				foreach (var value in values)
				{
					var count = GetStringEnum(lookup, value, buffer, enumType);
					Assert.True(count > 0, "Positive count expected");

					if (buffer[0] == (byte)'"')
					{
						// string
						Assert.True(count > 2, "Count > 2 expected");
						var name = Encoding.UTF8.GetString(buffer, 1, count - 2);
						var parsedValue = Enum.Parse(enumType, name, ignoreCase: false);
						Assert.Equal(value, ToUInt64(enumType, parsedValue));
					}
					else
					{
						var number = Encoding.UTF8.GetString(buffer, 0, count);
						bool signed = IsSigned(enumType);
						ulong parsedValue = signed ?
							(ulong)long.Parse(number, CultureInfo.InvariantCulture) :
							ulong.Parse(number, CultureInfo.InvariantCulture);
						Assert.Equal(value, parsedValue);
					}
				}
			}

			Assert.True(StringEnum.CachedLookups.Length >= enumTypes.Count, "All tested enum types should be cached");
			Assert.Equal(2, StringEnum.PagesAllocated);
		}

		static bool IsSigned(Type enumType)
		{
			switch (Type.GetTypeCode(enumType))
			{
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
					return true;
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return false;
				default:
					throw new NotSupportedException();
			}
		}

		static ulong ToUInt64(Type enumType, object value)
		{
			return IsSigned(enumType) ?
				(ulong)Convert.ToInt64(value) :
				Convert.ToUInt64(value);
		}

		static int GetStringEnum(StringEnum.Lookup lookup, ulong value, byte[] buffer, Type enumType)
		{
			bool isFlag = enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;

			int @sizeof;
			bool signed;
			switch (Type.GetTypeCode(enumType))
			{
				case TypeCode.SByte:
					@sizeof = 1;
					signed = true;
					break;
				case TypeCode.Int16:
					@sizeof = 2;
					signed = true;
					break;
				case TypeCode.Int32:
					@sizeof = 4;
					signed = true;
					break;
				case TypeCode.Int64:
					@sizeof = 8;
					signed = true;
					break;
				default:
					@sizeof = -1;
					signed = false;
					break;
			}

			int result;
			fixed (byte* destination = buffer)
			{
				switch (lookup.Type)
				{
					case StringEnum.LookupTypes.Indexed:
						if (signed)
						{
							if (isFlag) result = StringEnum.WriteIndexedFlag((long)value, lookup.TableStart, lookup.StringStart, destination, buffer.Length, @sizeof);
							else result = StringEnum.WriteIndexed((long)value, lookup.TableStart, lookup.StringStart, destination, buffer.Length);
						}
						else
						{
							if (isFlag) result = StringEnum.WriteIndexedFlag(value, lookup.TableStart, lookup.StringStart, destination, buffer.Length);
							else result = StringEnum.WriteIndexed(value, lookup.TableStart, lookup.StringStart, destination, buffer.Length);
						}
						break;
					case StringEnum.LookupTypes.Sorted:
						if (signed)
						{
							if (isFlag) result = StringEnum.WriteSortedFlag((long)value, lookup.TableStart, lookup.StringStart, destination, buffer.Length, @sizeof);
							else result = StringEnum.WriteSorted((long)value, lookup.TableStart, lookup.StringStart, destination, buffer.Length);
						}
						else
						{
							if (isFlag) result = StringEnum.WriteSortedFlag(value, lookup.TableStart, lookup.StringStart, destination, buffer.Length);
							else result = StringEnum.WriteSorted(value, lookup.TableStart, lookup.StringStart, destination, buffer.Length);
						}
						break;
					case StringEnum.LookupTypes.Verbose:
						if (signed)
						{
							if (isFlag) result = StringEnum.WriteVerboseFlag((long)value, lookup.TableStart, lookup.StringStart, destination, buffer.Length, @sizeof);
							else result = StringEnum.WriteVerbose((long)value, lookup.TableStart, lookup.StringStart, destination, buffer.Length);
						}
						else
						{
							if (isFlag) result = StringEnum.WriteVerboseFlag(value, lookup.TableStart, lookup.StringStart, destination, buffer.Length);
							else result = StringEnum.WriteVerbose(value, lookup.TableStart, lookup.StringStart, destination, buffer.Length);
						}
						break;
					case StringEnum.LookupTypes.Numeric:
						if (signed) result = Serializer.WriteInt64((long)value, destination, buffer.Length);
						else result = Serializer.WriteUInt64(value, destination, buffer.Length);
						break;
					default:
						throw new NotImplementedException();
				}
			}

			return result;
		}
	}
}
