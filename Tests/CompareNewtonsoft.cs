using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tests.Util;
using Xunit;
using Xunit.Extensions;

namespace Tests
{
	public class CompareNewtonsoft
	{
		public unsafe delegate int LowWriter<T>(T value, byte* dst, int avail);

		static readonly Encoding _utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
		static readonly Newtonsoft.Json.JsonSerializer _serializer = new Newtonsoft.Json.JsonSerializer();

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
				UnsafeJson.ConvertUTF.WriteStringUtf8,
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
				UnsafeJson.Convert.WriteUInt32,
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
				UnsafeJson.Convert.WriteInt32,
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
				UnsafeJson.Convert.WriteGuidFormatD,
				(sb, g) => sb.AppendLine("# " + g));
		}

		[Fact, MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe void DumpBoolean()
		{
			Approve(
				new[] { true, false },
				UnsafeJson.Convert.WriteBoolean,
				(sb, b) => sb.Append("# " + b),
				5);
		}

		static unsafe void Approve<T>(IEnumerable<T> values, LowWriter<T> writer, Action<StringBuilder, T> heading, int? minBuffer = null)
		{
			bool shouldPass = true;
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
				sb.AppendLine("## UnsafeJson");
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

				if (!equal) shouldPass = false;
			}

			ApprovalTests.Approvals.Verify(sb.ToString());

			Assert.True(shouldPass, "Look at the approval, this failed");
		}

		static int IndexOfDiff(IEnumerable<byte> xs, IEnumerable<byte> ys)
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
