using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cameronism.Json.Tests.Util;
using Xunit;
using Xunit.Extensions;

namespace Cameronism.Json.Tests
{
	public class CompareNewtonsoft
	{
		public unsafe delegate int LowWriter<T>(T value, byte* dst, int avail);

		internal static readonly Encoding _utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
		internal static readonly Newtonsoft.Json.JsonSerializer _serializer = new Newtonsoft.Json.JsonSerializer()
		{
			Converters = { new NewtonsoftConverters.IPAddressConverter() },
		};

		public static IEnumerable<string> InterestingStrings
		{
			get
			{
				return new string[] {
					"",
					null,
					String.Concat(Enumerable.Range(0, 256).Select(i => ((char)i))),
					String.Concat(Enumerable.Range(256, 256).Select(i => ((char)i))),
					String.Concat(Enumerable.Range(512, 256).Select(i => ((char)i))),
					String.Concat(Enumerable.Range(768, 256).Select(i => ((char)i))),
					//"乁 乂 乃 乄 久 乆 乇 么 义 乊 ",
					//"உ ஊ எ ஏ ஐ ஒ ஓ ஔ க ",
					//"֑ ֒ ֓ ֔ ֕ ֖ ֗ ֘ ֙ ֚ ֛ ֜ ֝ ֞ ֟ ֠ ֡ ֣ ֤ ֥ ֦ ֧ ֨ ֩ ֪ ֫ ֬ ֭ ֮ ֯ ְ ֱ ֲ ֳ ִ ֵ ֶ ַ ָ ֹ ֻ ּ ֽ ־ ֿ ׀ ׁ ׂ ׃ ׄ א ב ג ד ה ו ז ח ט י ך כ ל ם מ ן נ ס ע ף פ ץ צ ק ר ש ת װ ױ ײ ׳ ״",
					//"ټ ٽ پ ٿ ڀ ځ ڂ ڃ ڄ څ چ ڇ ڈ ډ ڊ ڋ ",
					char.ConvertFromUtf32(0x1f300),  // CYCLONE
					char.ConvertFromUtf32(0x1f601),  // GRINNING FACE WITH SMILING EYES
					char.ConvertFromUtf32(0x2702),   // BLACK SCISSORS
					char.ConvertFromUtf32(0x10FFFF), // max code point (probably)
				};
			}
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpStrings()
		{
			Approve(
				InterestingStrings,
				Cameronism.Json.ConvertUTF.WriteStringUtf8,
				(sb, s) =>
				{
					if (s == null)
					{
						sb.Append("# null");
					}
					else
					{
						sb.AppendLine("# utf8");
						Hex.Dump(sb, Encoding.UTF8.GetBytes(s));
					}
				});
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpChar()
		{
			Approve(
				new char[] {
					'0',
				}.Concat(Enumerable.Range(0, 128).Select(i => (char)i)),
				Cameronism.Json.ConvertUTF.WriteCharUtf8,
				(sb, s) =>
				{
					sb.AppendLine("# utf8");
					Hex.Dump(sb, Encoding.UTF8.GetBytes(s.ToString()));
				});
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpDouble()
		{
			var rand = new Random(42);
			var buffer = new byte[8];
			var numbers = new double[] {
					0,
					double.NaN,
					double.PositiveInfinity,
					double.NegativeInfinity,
					1,
					12,
					123,
					1234,
					12345,
					123456,
					1234567,
					12345678,
					123456789,
					1234567890,
					-1,
					int.MinValue,
					int.MaxValue,
					int.MinValue + 1,
					int.MaxValue - 1,
				}.Concat(Enumerable
					.Range(0, 128)
					.Select(i => 
					{
						rand.NextBytes(buffer);
						return BitConverter.ToDouble(buffer, 0);
					}));

			Approve(
				numbers,
				Cameronism.Json.Serializer.WriteDouble,
				(sb, g) => sb.AppendLine("# " + g),
				64);
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpSingle()
		{
			var rand = new Random(42);
			var buffer = new byte[4];
			var numbers = new float[] {
					0,
					float.NaN,
					float.PositiveInfinity,
					float.NegativeInfinity,
					1,
					12,
					123,
					1234,
					12345,
					123456,
					1234567,
					12345678,
					123456789,
					1234567890,
					-1,
					int.MinValue,
					int.MaxValue,
					int.MinValue + 1,
					int.MaxValue - 1,
				}.Concat(Enumerable
					.Range(0, 128)
					.Select(i => 
					{
						rand.NextBytes(buffer);
						return BitConverter.ToSingle(buffer, 0);
					}));

			Approve(
				numbers,
				Cameronism.Json.Serializer.WriteSingle,
				(sb, g) => sb.AppendLine("# " + g),
				64);
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpDecimal()
		{
			var rand = new Random(42);
			var buffer = new byte[16];
			var numbers = new decimal[] {
					0,
					decimal.MinValue,
					decimal.MaxValue,
					1,
					12,
					123,
					1234,
					12345,
					123456,
					1234567,
					12345678,
					123456789,
					1234567890,
					-1,
					int.MinValue,
					int.MaxValue,
					int.MinValue + 1,
					int.MaxValue - 1,
				}.Concat(Enumerable
					.Range(0, 128)
					.Select(i => 
					{
						rand.NextBytes(buffer);

						byte scale = (byte)(buffer[13] & 31);
						if (scale > 28) scale = 0; // more small numbers

						return new decimal(
							lo: BitConverter.ToInt32(buffer, 0),
							mid: BitConverter.ToInt32(buffer, 4),
							hi: BitConverter.ToInt32(buffer, 8),
							isNegative: buffer[12] % 2 == 0,
							scale: scale);
					}));

			Approve(
				numbers,
				Cameronism.Json.Serializer.WriteDecimal,
				(sb, g) => sb.AppendLine("# " + g),
				64);
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpUInt32()
		{
			var rand = new Random(42);
			var numbers = new int[] {
					0,
					1,
					12,
					123,
					1234,
					12345,
					123456,
					1234567,
					12345678,
					123456789,
					1234567890,
					-1,
					int.MinValue,
					int.MaxValue,
					int.MinValue + 1,
					int.MaxValue - 1,
				}.Concat(Enumerable
					.Range(0, 128)
					.Select(i => rand.Next(int.MinValue, int.MaxValue)))
				.Select(i => (uint)i);

			Approve(
				numbers,
				Cameronism.Json.Serializer.WriteUInt32,
				(sb, g) => sb.AppendLine("# " + g.ToString("n0")));
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpUInt64()
		{
			var numbers = new ulong[] {
					0,
					1,
					12,
					123,
					1234,
					12345,
					123456,
					1234567,
					12345678,
					123456789,
					1234567890,
					12345678901,
					123456789012,
					1234567890123,
					12345678901234,
					123456789012345,
					1234567890123456,
					12345678901234567,
					123456789012345678,
					1234567890123456789,
					12345678901234567890,
					long.MaxValue,
					ulong.MaxValue,
				};

			Approve(
				numbers,
				Cameronism.Json.Serializer.WriteUInt64,
				(sb, g) => sb.AppendLine("# " + g.ToString("n0")));
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpInt64()
		{
			var numbers = new long[] {
					0,
					1,
					12,
					123,
					1234,
					12345,
					123456,
					1234567,
					12345678,
					123456789,
					1234567890,
					12345678901,
					123456789012,
					1234567890123,
					12345678901234,
					123456789012345,
					1234567890123456,
					12345678901234567,
					123456789012345678,
					1234567890123456789,
					long.MaxValue,
					long.MinValue,
					-1,
					-12,
					-123,
					-1234,
					-12345,
					-123456,
					-1234567,
					-12345678,
					-123456789,
					-1234567890,
					-12345678901,
					-123456789012,
					-1234567890123,
					-12345678901234,
					-123456789012345,
					-1234567890123456,
					-12345678901234567,
					-123456789012345678,
					-1234567890123456789,
				};

			Approve(
				numbers,
				Cameronism.Json.Serializer.WriteInt64,
				(sb, g) => sb.AppendLine("# " + g.ToString("n0")));
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpInt32()
		{
			var rand = new Random(42);
			var numbers = Enumerable
				.Range(0, 128)
				.Select(i => rand.Next(int.MinValue, int.MaxValue))
				.Concat(new int[] {
					0,
					-1,
					1,
					12,
					123,
					1234,
					12345,
					123456,
					1234567,
					12345678,
					123456789,
					1234567890,
					int.MinValue,
					int.MaxValue,
					int.MinValue + 1,
					int.MaxValue - 1,
				});


			Approve(
				numbers,
				Cameronism.Json.Serializer.WriteInt32,
				(sb, g) => sb.AppendLine("# " + g.ToString("n0")));
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpGuid()
		{
			var rand = new Random(42);
			var bytes = new byte[16];
			var guids = new[] { 
					Guid.Empty,
				}
				.Concat(Enumerable
					.Range(0, 128)
					.Select(_ =>
					{
						rand.NextBytes(bytes);
						return new Guid(bytes);
					}));

			Approve(
				guids,
				Cameronism.Json.Serializer.WriteGuidFormatD,
				(sb, g) => sb.AppendLine("# " + g));
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpBoolean()
		{
			Approve(
				new[] { true, false },
				Cameronism.Json.Serializer.WriteBoolean,
				(sb, b) => sb.Append("# " + b),
				5);
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpDateTime()
		{
			var someEpoch = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			var rand = new Random(42);
			var bytes = new byte[16];
			var dts = new[] { 
					DateTime.MinValue,
					DateTime.MaxValue,
					someEpoch,
					someEpoch.AddTicks(1000000),
					someEpoch.AddTicks(1200000),
					someEpoch.AddTicks(1230000),
					someEpoch.AddTicks(1234000),
					someEpoch.AddTicks(1234500),
					someEpoch.AddTicks(1234560),
					someEpoch.AddTicks(1234567),
				}
				.Concat(Enumerable
					.Range(0, 16)
					.Select(_ =>
					{
						return someEpoch.AddTicks(rand.Next(int.MinValue, int.MaxValue));
					}))
				.Concat(Enumerable
					.Range(0, 16)
					.Select(_ =>
					{
						return someEpoch.AddSeconds(rand.Next(int.MinValue, int.MaxValue));
					}));

			Approve(
				dts,
				Cameronism.Json.Serializer.WriteDateTime8601,
				(sb, g) => sb.AppendLine("# " + g),
				Cameronism.Json.Serializer.SIZEOF_DATETIME_8601);
		}

		static unsafe void Approve<T>(IEnumerable<T> values, LowWriter<T> writer, Action<StringBuilder, T> heading, int? minBuffer = null)
		{
			int failed = 0;
			var sb = new StringBuilder();
			foreach (var v in values)
			{
				sb.AppendLine();
				sb.AppendLine();
				heading(sb, v);

				IEnumerable<byte> newtonsoft;
				IEnumerable<byte> mine;
				GetBoth(writer, v, out newtonsoft, out mine, minBuffer);

				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("## Newtonsoft");
				Hex.Dump(sb, newtonsoft);

				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("## Cameronism.Json");
				Hex.Dump(sb, mine);

				sb.AppendLine();
				sb.AppendLine();
				bool equal = mine != null && Enumerable.SequenceEqual(newtonsoft, mine);
				sb.AppendFormat("### Equal: {0}", equal);
				if (mine != null)
				{
					int diff = IndexOfDiff(newtonsoft, mine);
					if (diff != -1)
					{
						sb.AppendFormat("### Difference at: {0:x}", diff);
					}
				}

				if (!equal) failed++;
			}

			ApprovalTests.Approvals.Verify(sb.ToString());

			Assert.True(failed == 0, String.Format("Look at the approval, {0} comparisons failed", failed));
		}

		internal static int IndexOfDiff(IEnumerable<byte> xs, IEnumerable<byte> ys)
		{
			using (var en = xs.GetEnumerator())
			{
				int i = 0;
				foreach (var y in ys)
				{
					if (!en.MoveNext() || en.Current != y) return i;
					i++;
				}

				if (en.MoveNext()) return i;
			}

			return -1;
		}

		static unsafe void GetBoth<T>(LowWriter<T> lw, T s, out IEnumerable<byte> newtonsoft, out IEnumerable<byte> unsafeJson, int? minBuffer = null)
		{
			var ms = new MemoryStream();
			int len;
			using (var writer = new StreamWriter(ms, _utf8))
			{
				_serializer.Serialize(writer, s, typeof(string));
				writer.Flush();
				len = (int)ms.Position;
			}
			newtonsoft = ms.GetBuffer().Take(len);

			if (minBuffer.HasValue && minBuffer.Value > len) len = minBuffer.Value;
			var mine = new byte[len];

			fixed (byte* bs = mine)
			{
				var result = lw.Invoke(s, bs, len);
				unsafeJson = result < 0 ? null : mine.Take(result);
			}
		}
	}
}
