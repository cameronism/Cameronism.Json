using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cameronism.Json.Tests.Util;
using Xunit;

namespace Cameronism.Json.Tests
{
	public class DelegateBuilderTest
	{
		class NoMembers
		{
		}

		internal class B<T>
		{
			public T i { get; set; }
		}

		internal class B<T1, T2>
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

				GetType(new { }),

				typeof(int[]),
				typeof(int[][]),
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
				var schema = Cameronism.Json.Schema.Reflect(t);
				bool success = (bool)DescribeMethod.MakeGenericMethod(t).Invoke(null, new object[] { sb });
				if (!success) failed++;
			}

			ApprovalTests.Approvals.Verify(sb.ToString());

			Assert.True(failed == 0, "Look at the approval " + failed + " tests failed");
		}

		[Fact]
		public unsafe void AttemptSerialize()
		{
			var buffer = new byte[4096];
			var sb = new StringBuilder();
			int failed = 0;

			failed += SerializeValues<int?>(sb, buffer, 1, int.MaxValue, null);
			failed += SerializeValues<Guid?>(sb, buffer, Guid.Empty, Guid.Parse("41d091ba-d6bb-4795-969a-28d1509de6b6"), null);
			failed += SerializeValues<DayOfWeek?>(sb, buffer, DayOfWeek.Monday, DayOfWeek.Friday, null);
			failed += SerializeValues(sb, buffer, new { }, null);
			failed += SerializeValues<NoMembers[]>(sb, buffer, new[] { new NoMembers() }, new[] { new NoMembers(), null, new NoMembers() }, new NoMembers[0], null);
			failed += SerializeValues<IEnumerable<NoMembers>>(sb, buffer, new[] { new NoMembers() }, new[] { new NoMembers() }.ToList(), null);
			failed += SerializeValues(sb, buffer, A(1), null);
			failed += SerializeValues(sb, buffer, A(1,2), null);
			failed += SerializeValues(sb, buffer, A((int?)1,2),  A((int?)null,2));
			failed += SerializeValues(sb, buffer, A(2,(int?)1),  A(2,(int?)null));
			failed += SerializeValues(sb, buffer, A(2,(int?)1),  A(2,(int?)null));
			failed += SerializeValues<KeyValuePair<string, Guid>>(sb, buffer, new KeyValuePair<string, Guid>(null, Guid.Empty));
			failed += SerializeValues<KeyValuePair<string, Guid>?>(sb, buffer, new KeyValuePair<string, Guid>(null, Guid.Empty), null);
			failed += SerializeValues(sb, buffer, A(A(3,4),(int?)1),  A(A(3,4),(int?)null), A((B<int, int>)null,(int?)null));
			failed += SerializeValues(sb, buffer, A((int?)1,A(3,4)),  A((int?)null, A(3,4)), A((int?)null, (B<int, int>)null));
			failed += SerializeValues(sb, buffer, new int[0], new[] { 1 }, new[] { 1, 2 }, new[] { 1, 2, 3 }, (int[])null);
			failed += SerializeValues(sb, buffer, new int?[] { 1 }, new int?[] { null });
			failed += SerializeValues(sb, buffer, new int[][] { new[] { 1 }, null, new[] { 1, 2, 3 } }, null);
			failed += SerializeValues(sb, buffer, new int[][][] { new[] { new[] { 1 }, null, new int[0] }, null, new[] { new[] { 1, 2, 3 } }, null }, null);
			failed += SerializeValues(sb, buffer, new[] { A((int?)1), A((int?)null), }, null);
			failed += SerializeValues(sb, buffer, System.Net.IPAddress.Any, System.Net.IPAddress.IPv6Any, System.Net.IPAddress.Parse("1.2.3.4"), System.Net.IPAddress.Parse("1:2::3:4"), System.Net.IPAddress.Parse("11AA:22BB::33CC:44DD"), null);
			failed += SerializeValues(sb, buffer,
				new { a = System.Net.IPAddress.Any },
				new { a = System.Net.IPAddress.IPv6Any },
				new { a = System.Net.IPAddress.Parse("1.2.3.4") },
				new { a = System.Net.IPAddress.Parse("1:2::3:4") },
				new { a = System.Net.IPAddress.Parse("11AA:22BB::33CC:44DD") },
				new { a = (System.Net.IPAddress)null });
			failed += SerializeValues(sb, buffer, A(Guid.Empty, new List<System.Net.IPAddress> { null, System.Net.IPAddress.Any, System.Net.IPAddress.Broadcast }));
			failed += SerializeValues(sb, buffer, A(true, Enumerable.Range(0, 16).ToDictionary(i => i.ToString())));
			failed += SerializeValues(sb, buffer, new byte[] { }, new byte[] { 255 }, new byte[] { 255, 254 }, new byte[] { 255, 254, 253 }, null);
			failed += SerializeValues(sb, buffer, A(new byte[] { }), A(new byte[] { 255 }), A(new byte[] { 255, 254 }), A(new byte[] { 255, 254, 253 }));

			ApprovalTests.Approvals.Verify(sb.ToString());
			Assert.True(failed == 0, "Look at the approval " + failed + " tests failed");
		}

		[Fact]
		public void BadUsage()
		{
			var badSchema = new Schema { JsonType = (JsonType)int.MaxValue };
			Assert.Throws<ArgumentException>(() => DelegateBuilder.CreateStream<int>(badSchema));

			badSchema = new Schema { JsonType = JsonType.Boolean, NetType = this.GetType() };
			Assert.Throws<ArgumentException>(() => DelegateBuilder.CreateStream<int>(badSchema));

			badSchema = new Schema
			{
				JsonType = JsonType.Array,
				NetType = typeof(int[]),
				Items = new Schema
				{
					JsonType = (JsonType)int.MaxValue,
					NetType = typeof(int),
				}
			};
			Assert.Throws<ArgumentException>(() => DelegateBuilder.CreateStream<int[]>(badSchema));

			badSchema = new Schema
			{
				JsonType = JsonType.Array,
				NetType = typeof(int[]),
				Items = new Schema
				{
					JsonType = JsonType.Integer,
					NetType = this.GetType(),
				}
			};
			Assert.Throws<ArgumentException>(() => DelegateBuilder.CreateStream<int[]>(badSchema));
		}

		[Fact]
		public void ValueWriterBasics()
		{
			var defaultInt = ValueWriter.Lookup.Default<int>();
			var defaultLong = ValueWriter.Lookup.Default<long>();

			Assert.False(defaultInt.Equals(defaultLong));
		}

		/// <summary>Return count of failed values</summary>
		static int SerializeValues<T>(StringBuilder sb, byte[] buffer, params T[] values)
		{
			return SerializeValues(sb, buffer, values, false);
		}

		internal static int SerializeValues<T>(StringBuilder sb, byte[] buffer, T[] values, bool approveIL)
		{
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("# " + SchemaTest.HumanName(typeof(T)));

			Cameronism.Json.Serializer.WriteToPointer<T> writer;
			string instructions;
			try
			{
				var emit = Cameronism.Json.DelegateBuilder.CreatePointer<T>(Cameronism.Json.Schema.Reflect(typeof(T)));
				writer = emit.CreateDelegate<Serializer.WriteToPointer<T>>(out instructions);
			}
			catch (Sigil.SigilVerificationException sve)
			{
				sb.AppendLine("## failed");
				sb.AppendLine(sve.ToString());

				return values.Length;
			}

			if (approveIL)
			{
				sb.AppendLine();
				sb.AppendLine("## il");
				sb.AppendLine(instructions);
				sb.AppendLine();
			}

			int failed = 0;
			for (int i = 0; i < values.Length; i++)
			{
				for (int j = 0; j < buffer.Length; j++) buffer[j] = 0;

				var value = values[i];
				int myResult;
				var mine = GetBytes(value, writer, buffer, out myResult);
				var newtonsoft = GetNewtonsoft(value);

				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("## Newtonsoft " + i);
				Hex.Dump(sb, newtonsoft);

				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("## Cameronism.Json " + i);
				Hex.Dump(sb, mine);

				sb.AppendLine();
				sb.AppendLine();
				bool equal = mine != null && Enumerable.SequenceEqual(newtonsoft, mine);
				if (equal) AssertStreamVersion(value, newtonsoft);
				sb.AppendFormat("### Equal: {0}", equal);
				if (mine != null)
				{
					int diff = CompareNewtonsoft.IndexOfDiff(newtonsoft, mine);
					if (diff != -1)
					{
						sb.AppendFormat("### Difference at: {0:x}", diff);
					}
				}
				else
				{

					sb.AppendFormat("### result: {0}", myResult);
				}

				failed += equal ? 0 : 1;
			}

			if (failed > 0)
			{
				sb.AppendLine();
				sb.AppendLine(instructions);
			}

			return failed;
		}

		unsafe static IEnumerable<byte> GetBytes<T>(T value, Cameronism.Json.Serializer.WriteToPointer<T> writer, byte[] buffer, out int result)
		{
			fixed (byte* bs = buffer)
			{
				result = writer.Invoke(ref value, bs, buffer.Length);
			}

			return result < 0 ? null : buffer.Take(result);
		}

		static IEnumerable<byte> GetNewtonsoft<T>(T value)
		{
			var ms = new MemoryStream();
			int len;
			using (var writer = new StreamWriter(ms, CompareNewtonsoft._utf8))
			{
				CompareNewtonsoft._serializer.Serialize(writer, value, typeof(T));
				writer.Flush();
				len = (int)ms.Position;
			}
			return ms.GetBuffer().Take(len);
		}

		static byte[] _Buffer = new byte[1024];
		static IEnumerable<byte> GetStreamVersion<T>(T value)
		{
			var ms = new MemoryStream();
			Serializer.Serialize(value, ms, _Buffer);
			return ms.GetBuffer().Take((int)ms.Position);
		}

		static void AssertStreamVersion<T>(T value, IEnumerable<byte> other)
		{
			Assert.Equal(other, GetStreamVersion(value));
		}

		static readonly MethodInfo DescribeMethod = typeof(DelegateBuilderTest).GetMethod("Describe", BindingFlags.Static | BindingFlags.NonPublic);

		static bool Describe<T>(StringBuilder sb)
		{
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("# " + SchemaTest.HumanName(typeof(T)));

			var emit = Cameronism.Json.DelegateBuilder.CreatePointer<T>(Cameronism.Json.Schema.Reflect(typeof(T)));

			string instructions;
			try
			{
				var writer = emit.CreateDelegate<Serializer.WriteToPointer<T>>(out instructions);
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
