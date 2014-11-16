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

		[Fact]
		public void SimpleValues()
		{
			var ms = new MemoryStream();

			Assert.Equal("1", ToJson(1, ms));

			// FIXME submit a sigil bug with testcase for this
			DelegateBuilder.UseSigilVerify = false;
			Assert.Equal("1", ToJson((int?)1, ms));
			Assert.Equal("null", ToJson((int?)null, ms));
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
