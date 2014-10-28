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
		[Fact]
		public void FindMethods()
		{
			var sb = new StringBuilder();

			foreach (var type in new[]
			{
				typeof(List<int>),
				typeof(IEnumerable<int>),
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
