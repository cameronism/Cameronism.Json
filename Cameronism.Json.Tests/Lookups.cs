using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Cameronism.Json;
using Cameronism.Json.Tests.Util;
using Convert = Cameronism.Json.Serializer;

namespace Cameronism.Json.Tests
{
	public class Lookups
	{
		[Fact]
		public unsafe void Approve()
		{
			var bs = new byte[Serializer.SIZEOF_STATIC_LOOKUPS];
			fixed (byte* b = bs)
			{
				ushort* digitPairs, hexPairs;
				byte* jsonEscapes, base64;
				Serializer.CreateStaticLookups(b, out digitPairs, out jsonEscapes, out hexPairs, out base64);
			}

			var sb = Hex.Dump(bs);
			ApprovalTests.Approvals.Verify(sb.ToString());
		}
	}
}
