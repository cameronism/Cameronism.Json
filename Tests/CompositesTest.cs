using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
	public class CompositesTest
	{
		class B<T>
		{
			public T i { get; set; }
		}

		class B<T1, T2>
		{
			public T1 i { get; set; }
			public T2 j { get; set; }
		}

		public struct MyStructWithFields<T1, T2, T3>
		{
			public T1 i;
			public T2 j { get; set; }
			public T3 k;
		}

		static B<T> A<T>(T i)
		{
			return new B<T> { i = i };
		}

		static B<T1, T2> A<T1, T2>(T1 i, T2 j)
		{
			return new B<T1, T2> { i = i, j = j };
		}

		static Type GetType<T>(T sample)
		{
			return typeof(T);
		}

		[Fact]
		public void Reflect()
		{
			int failed = 0;
			var sb = new StringBuilder();

			foreach (var t in new[] { 
				typeof(System.Int32),
				typeof(System.Boolean),
				typeof(System.Byte),
				typeof(System.Char),
				typeof(System.DateTime),
				typeof(System.Decimal),
				typeof(System.Double),
				typeof(System.Int16),
				typeof(System.Int64),
				typeof(System.SByte),
				typeof(System.Single),
				typeof(System.String),
				typeof(System.UInt16),
				typeof(System.UInt32),
				typeof(System.UInt64),

				typeof(System.Int32?),
				typeof(System.Boolean?),
				typeof(System.Byte?),
				typeof(System.Char?),
				typeof(System.DateTime?),
				typeof(System.Decimal?),
				typeof(System.Double?),
				typeof(System.Int16?),
				typeof(System.Int64?),
				typeof(System.SByte?),
				typeof(System.Single?),
				typeof(System.UInt16?),
				typeof(System.UInt32?),
				typeof(System.UInt64?),

				typeof(System.Guid),
				typeof(System.Guid?),

				GetType(A(1)),
				GetType(A(1, 2)),
				GetType(A(1, A(1))),
				GetType(A(A(1), 1)),
				GetType(A(A(1, A(1)), 1)),
				GetType(A(1, A(A(1, A(1))))),
				GetType(A(Guid.Empty, A(A(1, A(1, (Guid?)null))))),
				typeof(KeyValuePair<int, int>),
				typeof(KeyValuePair<int, int>?),
				typeof(MyStructWithFields<int, int?, string>),
				typeof(MyStructWithFields<int, int?, string>?),

				//typeof(string[]),
				//typeof(IEnumerable<int>),
				//typeof(IReadOnlyList<Guid>),

				//GetType(new[] { new { s = "" } }.ToList()),

				//typeof(Dictionary<string, string>),
				//typeof(IReadOnlyDictionary<Guid, System.DayOfWeek>),


				//typeof(DateTimeOffset),
				//typeof(DateTimeOffset?),

				//typeof(System.Net.IPAddress),

				//typeof(ExplicitDataMemberOrder),
			})
			{
				var schema = UnsafeJson.Schema.Reflect(t);
				bool success = (bool)DescribeMethod.MakeGenericMethod(t).Invoke(null, new object[] { sb });
				if (!success) failed++;
			}

			ApprovalTests.Approvals.Verify(sb.ToString());

			Assert.True(failed == 0, "Look at the approval " + failed + " tests failed");
		}

		static readonly MethodInfo DescribeMethod = typeof(CompositesTest).GetMethod("Describe", BindingFlags.Static | BindingFlags.NonPublic);

		static bool Describe<T>(StringBuilder sb)
		{
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("# " + SchemaTest.HumanName(typeof(T)));

			var emit = UnsafeJson.Composites.Create<T>(UnsafeJson.Schema.Reflect(typeof(T)));

			string instructions;
			try
			{
				var writer = emit.CreateDelegate(out instructions);
				sb.AppendLine(instructions);
				return true;
			}
			catch (Sigil.SigilVerificationException sve)
			{
				sb.AppendLine("## failed");
				sb.AppendLine(sve.ToString());

				return false;
			}
		}
	}
}
