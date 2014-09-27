using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Util;
using Xunit;
using Xunit.Extensions;

namespace Tests
{
	public class CompareNewtonsoft
	{
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

		[Fact]
		public unsafe void DumpStrings()
		{
			bool shouldPass = true;
			var sb = new StringBuilder();
			foreach (var s in InterestingStrings)
			{
				sb.AppendLine();
				sb.AppendLine();
				if (s == null)
				{
					sb.Append("# null");
				}
				else
				{
					sb.AppendLine("# utf8");
					Hex.Dump(sb, Encoding.UTF8.GetBytes(s));
				}

				IEnumerable<byte> newtonsoft;
				IEnumerable<byte> mine;
				GetBoth(s, out newtonsoft, out mine);

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
				bool equal = Enumerable.SequenceEqual(newtonsoft, mine);
				sb.AppendFormat("### Equal: {0}", equal);
				int diff = IndexOfDiff(newtonsoft, mine);
				if (diff != -1)
				{
					sb.AppendFormat("### Difference at: {0:x}", diff);
				}

				if (!equal) shouldPass = false;
			}

			ApprovalTests.Approvals.Verify(sb.ToString());

			Assert.True(shouldPass, "Look at the approval, this failed");
		}

		[Fact(Skip="Crashes the process.  FIXME")]
		public unsafe void DumpUInt32()
		{
			var rand = new Random(42);
			var numbers = Enumerable
				.Range(0, 128)
				.Select(i => rand.Next(int.MinValue, int.MaxValue))
				.Concat(new int[] {
					0,
					-1,
					1,
					int.MinValue,
					int.MaxValue,
					int.MinValue + 1,
					int.MaxValue - 1,
				});

			bool shouldPass = true;
			var sb = new StringBuilder();
			foreach (var i in numbers)
			{
				var u = (uint)i;
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("# " + u.ToString("n0"));

				IEnumerable<byte> newtonsoft;
				IEnumerable<byte> mine;
				GetBoth(UnsafeJson.Convert.WriteUInt32, u, out newtonsoft, out mine);

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
				bool equal = Enumerable.SequenceEqual(newtonsoft, mine);
				sb.AppendFormat("### Equal: {0}", equal);
				int diff = IndexOfDiff(newtonsoft, mine);
				if (diff != -1)
				{
					sb.AppendFormat("### Difference at: {0:x}", diff);
				}

				if (!equal) shouldPass = false;
			}

			ApprovalTests.Approvals.Verify(sb.ToString());

			Assert.True(shouldPass, "Look at the approval, this failed");
		}

		[Fact]
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
					int.MinValue,
					int.MaxValue,
					int.MinValue + 1,
					int.MaxValue - 1,
				});

			bool shouldPass = true;
			var sb = new StringBuilder();
			foreach (var i in numbers)
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("# " + i.ToString("n0"));

				IEnumerable<byte> newtonsoft;
				IEnumerable<byte> mine;
				GetBoth(UnsafeJson.Convert.WriteInt32, i, out newtonsoft, out mine);

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
				bool equal = Enumerable.SequenceEqual(newtonsoft, mine);
				sb.AppendFormat("### Equal: {0}", equal);
				int diff = IndexOfDiff(newtonsoft, mine);
				if (diff != -1)
				{
					sb.AppendFormat("### Difference at: {0:x}", diff);
				}

				if (!equal) shouldPass = false;
			}

			ApprovalTests.Approvals.Verify(sb.ToString());

			Assert.True(shouldPass, "Look at the approval, this failed");
		}

		[Fact]
		public unsafe void DumpGuid()
		{
			var rand = new Random(42);
			var bytes = new byte[16];
			var numbers = new[] { 
					Guid.Empty,
				}
				.Concat(Enumerable
					.Range(0, 128)
					.Select(_ =>
					{
						rand.NextBytes(bytes);
						return new Guid(bytes);
					}));

			bool shouldPass = true;
			var sb = new StringBuilder();
			foreach (var g in numbers)
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("# " + g);

				IEnumerable<byte> newtonsoft;
				IEnumerable<byte> mine;
				GetBoth(UnsafeJson.Convert.WriteGuidFormatD, g, out newtonsoft, out mine);

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
				bool equal = Enumerable.SequenceEqual(newtonsoft, mine);
				sb.AppendFormat("### Equal: {0}", equal);
				int diff = IndexOfDiff(newtonsoft, mine);
				if (diff != -1)
				{
					sb.AppendFormat("### Difference at: {0:x}", diff);
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


		public unsafe delegate int LowWriter<T>(T value, byte* dst, int avail);

		static unsafe void GetBoth<T>(LowWriter<T> lw, T s, out IEnumerable<byte> newtonsoft, out IEnumerable<byte> unsafeJson)
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

			var mine = new byte[len];

			fixed (byte* bs = mine)
			{
				var result = lw.Invoke(s, bs, len);
				unsafeJson = result < 0 ? null : mine.Take(result);
			}
		}

		static unsafe void GetBoth(string s, out IEnumerable<byte> newtonsoft, out IEnumerable<byte> unsafeJson)
		{
			GetBoth(UnsafeJson.ConvertUTF.WriteStringUtf8, s, out newtonsoft, out unsafeJson);
		}
	}
}
