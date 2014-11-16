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
		byte[] _Buffer = new byte[4096];

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

		[Fact(Skip="In progress")]
		public void EnumerableValues()
		{
			var ms = new MemoryStream();

			Assert.Equal("[1]", ToJson(new List<int> { 1 }, ms));
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

		string ToJson<T>(T value, MemoryStream ms)
		{
			ms.Position = 0;
			try
			{
				Serializer.Serialize(value, ms, _Buffer);
			}
			catch (Sigil.SigilVerificationException sve)
			{
				Trace.WriteLine(sve);
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
