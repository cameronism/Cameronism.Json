using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cameronism.Json.Tests
{
	public class ConstantMethodsTest
	{
		#region properties
		string C2 { get { return null; } }
		string C3 { get { return "a"; } }
		int C4 { get { return 1; } }
		int? C5 { get { return 1; } }
		int? C6 { get { return null; } }
		char C7 { get { return 'a'; } }
		char? C8 { get { return 'a'; } }
		char? C9 { get { return null; } }
		DateTime C10 { get { return default(DateTime); } }
		int C11 { get { return -1; } }
		int C12 { get { return unchecked((int)0x80000000); } }
		long C13 { get { return 0xffffffffff; } }
		double C14 { get { return 0.5; } }
		float C15 { get { return 0.5f; } }
		sbyte C16 { get { return sbyte.MaxValue; } }
		byte C17 { get { return byte.MaxValue; } }
		short C18 { get { return short.MaxValue; } }
		ushort C19 { get { return ushort.MaxValue; } }
		uint C20 { get { return uint.MaxValue; } }
		ulong C21 { get { return ulong.MaxValue; } }
		long? C22 { get { return 0xffffffffff; } }
		double? C23 { get { return 0.5; } }
		float? C24 { get { return 0.5f; } }
		sbyte? C25 { get { return sbyte.MaxValue; } }
		byte? C26 { get { return byte.MaxValue; } }
		short? C27 { get { return short.MaxValue; } }
		ushort? C28 { get { return ushort.MaxValue; } }
		uint? C29 { get { return uint.MaxValue; } }
		ulong? C30 { get { return ulong.MaxValue; } }
		Guid C31 { get { return default(Guid); } }
		Guid? C32 { get { return default(Guid); } }
		bool C33 { get { return true; } }
		bool? C34 { get { return false; } }
		bool? C35 { get { return null; } }
		decimal C36 { get { return 1.1M; } }
		decimal? C37 { get { return 1.1M; } }
		decimal? C38 { get { return null; } }
		DateTime D39 { get { return SomeDateTime(); } }
		DateTime D40 { get { return new DateTime(2010, 1, 1, 1, 1, 1, DateTimeKind.Utc); } } // TODO
		Guid D41 { get { return new Guid("3350c137-a6c7-4e3f-aa4d-182f068899c5"); } } // TODO
		DelegateBuilderTest.B<int> D42 { get { return new DelegateBuilderTest.B<int>(); } }
		#endregion
		static DateTime SomeDateTime() { return new DateTime(2010, 1, 1, 1, 1, 1, DateTimeKind.Utc); }

		static bool ToleratedFailure(Type type, string newtonsoft)
		{
			var underlying = Nullable.GetUnderlyingType(type) ?? type;

			return newtonsoft != null &&
				(underlying == typeof(float) || underlying == typeof(decimal));
		}

		[Fact]
		public void ApproveConstants()
		{
			var methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

			var sb = new StringBuilder();
			int failedEqualCount = 0;

			foreach (var mi in methods)
			{
				if (!mi.IsSpecialName || !mi.Name.StartsWith("get_")) continue;

				var shouldBeConstant = mi.Name[4] == 'C';
				var actual = mi.Invoke(this, null);
				var nJSON = Newtonsoft.Json.JsonConvert.SerializeObject(actual);
				var constant = ConstantMethods.TryGetJson(mi);

				var passed = shouldBeConstant ?
					String.Equals(nJSON, constant, StringComparison.Ordinal) :
					constant == null;

				sb.AppendLine("# " + mi.Name.Substring(4) + " " + SchemaTest.HumanName(mi.ReturnType));
				sb.AppendLine();
				sb.AppendLine("Expected constant: " + shouldBeConstant);
				if (!passed)
				{
					if (ToleratedFailure(mi.ReturnType, nJSON))
					{
						sb.AppendLine("** SKIPPED ** - fix this");
					}
					else
					{
						failedEqualCount++;
						sb.AppendLine("** FAILED **");
					}
				}

				sb.AppendLine();
				sb.AppendLine("## Newtonsoft");
				Util.Hex.Dump(sb, Encoding.UTF8.GetBytes(nJSON));
				sb.AppendLine();

				sb.AppendLine();
				sb.AppendLine("## Cameronism.Json");
				if (constant != null)
				{
					Util.Hex.Dump(sb, Encoding.UTF8.GetBytes(constant));
				}
				else
				{
					sb.AppendLine();
					sb.AppendLine("`null`");
				}
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine();
			}

			ApprovalTests.Approvals.Verify(sb.ToString());
			Assert.True(failedEqualCount == 0, "Look at the approval, " + failedEqualCount + " failed");
		}
	}
}
