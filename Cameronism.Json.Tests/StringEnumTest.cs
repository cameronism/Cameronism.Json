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
		public void DayOfWeekEnum()
		{
			var sb = new StringBuilder();
			ApproveIndexedEnum<DayOfWeek>(sb);
			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		public void ApproveIndexedEnum<T>(StringBuilder sb)
		{
			var buffer = new byte[1024];

			fixed (byte* ptr = buffer)
			{
				int freeStart;
				var lookup = StringEnum.GenerateIndexedLookup(typeof(T), ptr, buffer.Length, out freeStart);

				Assert.True(lookup.HasValue, "lookup must have been generated");
				var stringStart = (int)(lookup.Value.StringStart - ptr);


				sb.AppendLine("# Indexed " + SchemaTest.HumanName(typeof(T)));
				sb.AppendLine("## Lookups");
				Util.Hex.Dump(sb, buffer.Take(stringStart));
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("## Strings");
				Util.Hex.Dump(sb, buffer.Skip(stringStart).Take(freeStart - stringStart));


				var chars = new char[64];
				foreach (T dow in Enum.GetValues(typeof(T)))
				{
					byte* str;
					int len;
					lookup.Value.FindIndexed(Convert.ToUInt64(dow), out str, out len);
					Assert.True(str != null, "string must not be null");
					var dowString = dow.ToString();
					Assert.Equal(dowString.Length, len);

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

		public void ApproveSortedEnum<T>(StringBuilder sb)
		{
			var buffer = new byte[1024];

			fixed (byte* ptr = buffer)
			{
				int freeStart;
				var lookup = StringEnum.GenerateSortedLookup(typeof(T), ptr, buffer.Length, out freeStart);

				Assert.True(lookup.HasValue, "lookup must have been generated");
				var stringStart = (int)(lookup.Value.StringStart - ptr);


				sb.AppendLine("# Sorted " + SchemaTest.HumanName(typeof(T)));
				sb.AppendLine("## Lookups");
				Util.Hex.Dump(sb, buffer.Take(stringStart));
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("## Strings");
				Util.Hex.Dump(sb, buffer.Skip(stringStart).Take(freeStart - stringStart));


				var chars = new char[64];
				//foreach (T dow in Enum.GetValues(typeof(T)))
				//{
				//	byte* str;
				//	int len;
				//	lookup.Value.FindSequential(Convert.ToUInt64(dow), out str, out len);
				//	Assert.True(str != null, "string must not be null");
				//	var dowString = dow.ToString();
				//	Assert.Equal(dowString.Length, len);

				//	string actualString;
				//	fixed (char* ch = chars)
				//	{
				//		var strLen = Encoding.UTF8.GetChars(str, len, ch, chars.Length);
				//		actualString = new string(ch, 0, strLen);
				//	}
				//	Assert.Equal(dowString, actualString);
				//}
			}
		}
	}
}
