using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using UnsafeJson;
using Tests.Util;
using Convert = UnsafeJson.Convert;

namespace Tests
{
	public class Lookups
	{
		[Fact]
		public unsafe void Approve()
		{
			var bs = new byte[Convert.SIZEOF_STATIC_LOOKUPS];
			fixed (byte* b = bs)
			{
				ushort* digitPairs;
				byte* jsonEscapes;
				Convert.CreateStaticLookups(b, out digitPairs, out jsonEscapes);
			}

			var sb = Hex.Dump(bs);
			ApprovalTests.Approvals.Verify(sb.ToString());
		}
	}
}
