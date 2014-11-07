using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnsafeJson;
using Xunit;

namespace Tests
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



	}
}
