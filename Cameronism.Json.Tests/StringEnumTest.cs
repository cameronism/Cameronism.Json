using System;
using System.Collections.Generic;
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
	}
}
