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
			ApproveSequentialEnum<Skippy>();
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public void DayOfWeekEnum()
		{
			ApproveSequentialEnum<DayOfWeek>();
		}

		public void ApproveSequentialEnum<T>()
		{
			var buffer = new byte[1024];
			int stringStart;
			int freeStart;

			fixed (byte* ptr = buffer)
			{
				var lookup = StringEnum.GenerateSequentialLookup(typeof(T), ptr, buffer.Length, out freeStart);

				Assert.True(lookup.HasValue, "lookup must have been generated");
				stringStart = (int)(lookup.Value.StringStart - ptr);

				var chars = new char[64];
				foreach (T dow in Enum.GetValues(typeof(T)))
				{
					byte* str;
					int len;
					lookup.Value.FindSequential(Convert.ToUInt64(dow), out str, out len);
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

			var sb = new StringBuilder();
			sb.AppendLine("## Lookups");
			Util.Hex.Dump(sb, buffer.Take(stringStart));
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("## Strings");
			Util.Hex.Dump(sb, buffer.Skip(stringStart).Take(freeStart - stringStart));

			ApprovalTests.Approvals.Verify(sb.ToString());
		}
	}
}
