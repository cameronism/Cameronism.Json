using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cameronism.Json;
using Xunit;

namespace Cameronism.Json.Tests
{
	public class EnumerableTest
	{
		class ExplicitEnumerableBadCount : IEnumerable<bool>
		{
			IEnumerator<bool> IEnumerable<bool>.GetEnumerator() { throw new NotImplementedException(); } 
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new NotImplementedException(); } 
			/// <summary>
			/// This should not be picked up since neither ICollection<> or IReadOnlyCollection are implemented
			/// </summary>
			public int Count
			{
				get
				{
					return 0;
				}
			}
		}

		class ExplicitReadOnlyCollection : IReadOnlyCollection<Guid>
		{
			int IReadOnlyCollection<Guid>.Count { get { throw new NotImplementedException(); } } 
			IEnumerator<Guid> IEnumerable<Guid>.GetEnumerator() { throw new NotImplementedException(); } 
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
		}
		class ExplicitReadOnlyCollectionImplicitCount : IReadOnlyCollection<byte>
		{
			public int Count { get { throw new NotImplementedException(); } }
			IEnumerator<byte> IEnumerable<byte>.GetEnumerator() { throw new NotImplementedException(); }
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
		}
		class ExplicitReadOnlyCollectionPublicEnumerator : IReadOnlyCollection<sbyte>
		{
			public struct Enumerator
			{
				public bool MoveNext() { return false; }
				public sbyte Current { get { return 0; } }
			}

			public Enumerator GetEnumerator() { return new Enumerator(); }
			int IReadOnlyCollection<sbyte>.Count { get { throw new NotImplementedException(); } } 
			IEnumerator<sbyte> IEnumerable<sbyte>.GetEnumerator() { throw new NotImplementedException(); } 
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
		}

		class MyReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
		{
			KeyValuePair<TKey,TValue>[] _Pairs;

			private MyReadOnlyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
			{
				_Pairs = pairs.ToArray();
			}

			public bool ContainsKey(TKey key)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<TKey> Keys
			{
				get { throw new NotImplementedException(); }
			}

			public bool TryGetValue(TKey key, out TValue value)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<TValue> Values
			{
				get { throw new NotImplementedException(); }
			}

			public TValue this[TKey key]
			{
				get { throw new NotImplementedException(); }
			}

			public int Count
			{
				get { throw new NotImplementedException(); }
			}

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				return ((IEnumerable<KeyValuePair<TKey, TValue>>)_Pairs).GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _Pairs.GetEnumerator();
			}

			public static MyReadOnlyDictionary<TKey, TValue> Create(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
			{
				return new MyReadOnlyDictionary<TKey, TValue>(pairs);
			}
		}



		[Fact]
		public void FindMethods()
		{
			var sb = new StringBuilder();

			foreach (var type in new[]
			{
				typeof(List<int>),
				typeof(IEnumerable<int>),
				typeof(ExplicitEnumerableBadCount),
				typeof(ExplicitReadOnlyCollection),
				typeof(ExplicitReadOnlyCollectionImplicitCount),
				typeof(ExplicitReadOnlyCollectionPublicEnumerator),
				typeof(ICollection<short>),
				typeof(IReadOnlyCollection<ushort>),
				typeof(IReadOnlyList<long>),
				typeof(IList<ulong>),
				this.GetType(), // throw in something non enumerable
			})
			{
				var methods = EnumerableInfo.FindMethods(type);
				sb.AppendLine("# " + SchemaTest.HumanName(type));
				DescribeMethod(sb, () => methods.GetEnumerator);
				DescribeMethod(sb, () => methods.MoveNext);
				DescribeMethod(sb, () => methods.get_Current);
				DescribeMethod(sb, () => methods.get_Count);
				DescribeMethod(sb, () => methods.Dispose);

				sb.AppendLine();
			}

			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		static void DescribeMethod(StringBuilder sb, Expression<Func<MethodInfo>> fmi)
		{
			var prop = fmi.Body as MemberExpression;
			sb.AppendLine("## " + prop.Member.Name);

			var mi = fmi.Compile().Invoke();
			if (mi == null)
			{
				sb.AppendLine("`null`");
			}
			else
			{
				sb.AppendLine("Return Type: " + SchemaTest.HumanName(mi.ReturnType));
				sb.AppendLine("IsVirtual: " + mi.IsVirtual);
				sb.AppendLine("Declaring Type: " + SchemaTest.HumanName(mi.DeclaringType));
				foreach (var arg in mi.GetParameters())
				{
					sb.AppendLine(String.Format("{0}: {1}", arg.Name, SchemaTest.HumanName(arg.ParameterType)));
				}
			}

			sb.AppendLine();
		}

		[Fact]
		public void SerializeEnumerable()
		{
			var buffer = new byte[4096];
			var sb = new StringBuilder();
			int failed = 0;

			failed += SerializeValues<IEnumerable<int>>(sb, buffer, true,
				null, 
				new List<int> { },
				new List<int> { 1 },
				new List<int> { 1, 2 },
				new int[] { },
				new int[] { 2 },
				new int[] { 2, 4 },
				Enumerable.Empty<int>(),
				Enumerable.Repeat(3, 0),
				Enumerable.Repeat(3, 1),
				Enumerable.Repeat(3, 2),
				Enumerable.Repeat(3, 3),
				null);

			failed += SerializeValues(sb, buffer, true,
				new List<int> { },
				new List<int> { 1 },
				new List<int> { 1, 2 },
				new List<int> { 1, 2, 3 });

			failed += SerializeValues<IList<int?>>(sb, buffer, false,
				new int?[] { },
				new List<int?> { null },
				new int?[] { 1 },
				new List<int?> { 1, null },
				new int?[] { 1, 2 },
				new List<int?> { 1, 2, null },
				new int?[] { 1, 2, 3 },
				new List<int?> { 1, 2, 3, null });

			failed += SerializeValues<IReadOnlyCollection<int?>>(sb, buffer, false,
				new int?[] { },
				new List<int?> { null }.AsReadOnly(),
				new int?[] { 1 },
				new List<int?> { 1, null },
				new int?[] { 1, 2 },
				new List<int?> { 1, 2, null },
				new int?[] { 1, 2, 3 },
				new List<int?> { 1, 2, 3, null });


			failed += SerializeValues(sb, buffer, false,
				new List<int?> { null }.AsReadOnly());

			// Newtonsoft thinks this should be an object not an array :(
			//failed += SerializeValues<IEnumerable<KeyValuePair<string, int>>>(sb, buffer, true,
			//	new Dictionary<string, int> { },
			//	new Dictionary<string, int> { { "0", 0 } },
			//	new Dictionary<string, int> { { "0", 0 }, { "1", 1 } },
			//	null);

			failed += SerializeValues(sb, buffer, true,
				new Dictionary<string, int> { },
				new Dictionary<string, int> { { "0", 0 } },
				new Dictionary<string, int> { { "0", 0 }, { "1", 1 } },
				null);

			failed += SerializeValues(sb, buffer, false,
				MyReadOnlyDictionary<string, int>.Create(new Dictionary<string, int> { }),
				MyReadOnlyDictionary<string, int>.Create(new Dictionary<string, int> { { "0", 0 } }),
				MyReadOnlyDictionary<string, int>.Create(new Dictionary<string, int> { { "0", 0 }, { "1", 1 } }),
				null);

			failed += SerializeValues(sb, buffer, false,
				new Dictionary<char, int> { },
				new Dictionary<char, int> { { '0', 0 } },
				new Dictionary<char, int> { { '0', 0 }, { '1', 1 } },
				null);

			failed += SerializeValues(sb, buffer, false,
				new Dictionary<Guid, int[]> { },
				new Dictionary<Guid, int[]> { { Guid.Empty, null } },
				new Dictionary<Guid, int[]> { { Guid.Empty, new[] { 0 } }, { Guid.Parse("838f5c03-ed56-41e1-9264-686bb18d0a77"), new[] { 1, 2 } } },
				null);

			ApprovalTests.Approvals.Verify(sb.ToString());
			Assert.True(failed == 0, "Look at the approval " + failed + " tests failed");
		}

		static int SerializeValues<T>(StringBuilder sb, byte[] buffer, bool approveIL, params T[] values)
		{
			return DelegateBuilderTest.SerializeValues(sb, buffer, values, approveIL);
		}

		class MethodBucket
		{
			public int _1 () { return 0; }
			public string _2 () { return null; }
			public IDisposable _3() { return null; }
			private int _4() { return 0; }
			private IDisposable _5() { return null; }
		}

		[Fact]
		public void VerifyGetEnumeratorOrder()
		{
			// repeat it several times so that we try both directions of all comparisons
			var methods = Enumerable.Repeat(typeof(MethodBucket).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly), 16)
				.SelectMany(xs => xs)
				.ToArray();

			var expectedOrder = methods.OrderBy(mi => int.Parse(mi.Name.Substring(1))).ToList();
			Array.Sort(methods, EnumerableInfo.SortEnumeratorCandidates);

			Assert.Equal(expectedOrder.Select(mi => mi.Name), methods.Select(mi => mi.Name), StringComparer.Ordinal);
		}
	}
}
