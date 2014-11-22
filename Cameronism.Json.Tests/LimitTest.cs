using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cameronism.Json.Tests.Util;
using Xunit;

namespace Cameronism.Json.Tests
{
	public class LimitTest
	{
		[Fact]
		public void OneByte()
		{
			var sb = new StringBuilder();

			Describe(1, sb, 1);
			Describe(11, sb, 1);
			Describe<uint>((uint)int.MaxValue + 1, sb, 8);
			Describe<ulong>((ulong)uint.MaxValue + 1, sb, 8);
			Describe<long>((uint)int.MaxValue + 1, sb, 8);

			Describe<string>(null, sb, 1);
			Describe("\u0000", sb, 3);
			Describe("aa", sb, 3);
			Describe('a', sb, 2);
			Describe('\u0000', sb, 4);

			Describe(double.NaN, sb, 1);
			Describe(double.MinValue, sb, 1);
			Describe(float.NaN, sb, 1);
			Describe(float.MinValue, sb, 1);

			Describe(IPAddress.Parse("::1"), sb, 1);
			Describe(IPAddress.Parse("1.2.3.4"), sb, 1);

			Describe(new DateTime(2010, 1, 1, 1, 1, 1, DateTimeKind.Utc), sb, 1);
			Describe(false, sb, 4);

			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		static byte[] _Buffer = new byte[64];

		[Fact]
		public void InsufficientStrings()
		{
			Assert.Equal(0, ConvertUTF.EscapeJson(null, _Buffer));
			Assert.Equal(0, ConvertUTF.EscapeJson("a", null));
			Assert.Equal(1, ConvertUTF.EscapeJson("a", _Buffer));
			Assert.Equal(0, ConvertUTF.EscapeJson("\uD800", _Buffer)); // lonely high surrogate

			// unicode replacement char?
			//Assert.Equal(3, ConvertUTF.EscapeJson("\uDBFF\uDFFF", _Buffer));
		}

		unsafe static void Describe<T>(T value, StringBuilder sb, int count)
		{
			if (count < 1) throw new ArgumentException();

			var buffer = _Buffer;
			for (int i = 0; i < count; i++) buffer[i] = 0;

			int result;
			fixed (byte* ptr = buffer)
			{
				result = Serializer.Serialize(value, ptr, count);
			}

			sb.AppendLine("# " + Newtonsoft.Json.JsonConvert.SerializeObject(value, new NewtonsoftConverters.IPAddressConverter()));
			sb.AppendLine("## count: " + count);
			sb.AppendLine("## result: " + result);
			Hex.Dump(sb, buffer.Take(count));
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine();
		}
	}
}
