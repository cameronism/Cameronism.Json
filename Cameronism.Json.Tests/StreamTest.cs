using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cameronism.Json.Tests
{
	public class StreamTest : IDisposable
	{
		byte[] _Buffer = new byte[64];

		static DelegateBuilderTest.B<T> A<T>(T i)
		{
			return new DelegateBuilderTest.B<T> { i = i };
		}

		static DelegateBuilderTest.B<T1, T2> A<T1, T2>(T1 i, T2 j)
		{
			return new DelegateBuilderTest.B<T1, T2> { i = i, j = j };
		}

		[Fact]
		public void SimpleValues()
		{
			var ms = new MemoryStream();

			AssertValue(1, "1", ms);
			AssertValue(true, "true", ms);
			AssertValue(false, "false", ms);

			var g = Guid.NewGuid();
			AssertValue(g, "\"" + g + "\"", ms);

			var d = DateTime.UtcNow;
			AssertValue(d, Newtonsoft.Json.JsonConvert.SerializeObject(d), ms);
		}

		[Fact]
		public void EnumerableValues()
		{
			var ms = new MemoryStream();

			string instructions;
			Assert.Equal("[1]", ToJson(new List<int> { 1 }, ms, out instructions));
			Assert.Equal("[12]", ToJson(new List<int> { 12 }, ms, out instructions));

			Assert.Equal("[1,null]", ToJson(new List<int?> { 1, null }, ms, out instructions));

			Assert.Equal("[1]", ToJson(new[] { 1 }, ms, out instructions));
			Assert.Equal("[1,null]", ToJson(new int?[] { 1, null }, ms, out instructions));

			Assert.Equal("{\"i\":1}", ToJson(A(1), ms, out instructions));
			Assert.Equal("{\"i\":0.5,\"j\":\"00000000-0000-0000-0000-000000000000\"}", ToJson(A(0.5, Guid.Empty), ms, out instructions));

			Assert.Equal("{\"i\":1,\"j\":2}", ToJson(new Dictionary<char, int> { 
				{ 'i', 1 },
				{ 'j', 2 },
			}, ms, out instructions));

			Assert.Equal("[{\"i\":1}]", ToJson(new[] { A(1) }, ms, out instructions));
			Assert.Equal("{\"i\":[1]}", ToJson(A(new[] { 1 }), ms, out instructions));
		}

		[Fact]
		public void CourtesyFlush()
		{
			var ms = new MemoryStream();

			string instructions;

			var xs = Enumerable.Repeat(1, 64);
			var json = "[" + String.Join(",", xs) + "]";
			Assert.Equal(json, ToJson(xs, ms, out instructions));

			xs = Enumerable.Repeat(int.MinValue, 64);
			json = "[" + String.Join(",", xs) + "]";
			Assert.Equal(json, ToJson(xs, ms, out instructions));
		}

		[Fact]
		public void StreamInstructions()
		{
			var ms = new MemoryStream();
			var sb = new StringBuilder();

			string instructions;

			sb.AppendLine("# IEnumerable<int>");
			sb.AppendLine();
			ToJson<IEnumerable<int>>(null, ms, out instructions);
			sb.AppendLine(instructions);

			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		void AssertValue<T>(T value, string json, MemoryStream ms) where T : struct
		{
			Assert.Equal(json, ToJson(value, ms));

			// FIXME submit a sigil bug with testcase for this
			DelegateBuilder.UseSigilVerify = false;
			Assert.Equal(json, ToJson((T?)value, ms));
			Assert.Equal("null", ToJson((T?)null, ms));
			DelegateBuilder.UseSigilVerify = true;
		}

		unsafe string ToJson<T>(T value, MemoryStream ms)
		{
			string instructions;
			return ToJson(value, ms, out instructions);
		}

		unsafe string ToJson<T>(T value, MemoryStream ms, out string instructions)
		{
			ms.Position = 0;
			try
			{
				var schema = Schema.Reflect(typeof(T));
				var emit = DelegateBuilder.CreateStream<T>(schema);
				var del = emit.CreateDelegate<Serializer.WriteToStream<T>>(out instructions);
				fixed (byte* ptr = _Buffer)
				{
					del.Invoke(ref value, ptr, ms, _Buffer);
				}
			}
			catch (Sigil.SigilVerificationException sve)
			{
				Trace.WriteLine(sve);
				instructions = sve.GetDebugInfo();
				throw;
			}
			return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position);
		}

		public void Dispose()
		{
			_Buffer = null;
		}
	}
}
