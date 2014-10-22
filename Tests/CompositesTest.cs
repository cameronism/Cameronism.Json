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
		static Type GetType<T>(T sample)
		{
			return typeof(T);
		}

		[Fact]
		public void Reflect()
		{
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

				//typeof(IEnumerable<int>),
				//typeof(string[]),
				//typeof(IReadOnlyList<Guid>),

				//GetType(new { i = 1 }),
				//GetType(new[] { new { s = "" } }.ToList()),

				//typeof(Dictionary<string, string>),
				//typeof(IReadOnlyDictionary<Guid, System.DayOfWeek>),

				//typeof(KeyValuePair<int, int>),
				//typeof(KeyValuePair<int, int>?),

				//typeof(DateTimeOffset),
				//typeof(DateTimeOffset?),

				//typeof(System.Net.IPAddress),

				//typeof(ExplicitDataMemberOrder),
			})
			{
				var schema = UnsafeJson.Schema.Reflect(t);
				DescribeMethod.MakeGenericMethod(t).Invoke(null, new object[] { sb });
			}

			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		static readonly MethodInfo DescribeMethod = typeof(CompositesTest).GetMethod("Describe", BindingFlags.Static | BindingFlags.NonPublic);

		static void Describe<T>(StringBuilder sb)
		{
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("# " + SchemaTest.HumanName(typeof(T)));

			var emit = UnsafeJson.Composites.Create<T>(UnsafeJson.Schema.Reflect(typeof(T)));

			string instructions;
			var writer = emit.CreateDelegate(out instructions);
			sb.AppendLine(instructions);
		}
	}
}
