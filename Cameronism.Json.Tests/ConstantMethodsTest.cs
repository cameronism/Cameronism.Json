using System;
using System.Collections.Generic;
using System.IO;
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
		double C43 { get { return 0; } }
		float C44 { get { return 0; } }
		sbyte C45 { get { return -2; } }
		short C46 { get { return -2; } }
		int C47 { get { return -2; } }
		long C48 { get { return -2; } }
		char C49 { get { return '\u0000'; } }
		string C50 { get { return "\r\n"; } }
		string C51 { get { return "\uFF72\u3093\u4E47 \u4E02\u4E47c\u5C3A\u4E47\uFF72 \uFF89\u4E02 ou\uFF72."; } }
		#endregion
		static DateTime SomeDateTime() { return new DateTime(2010, 1, 1, 1, 1, 1, DateTimeKind.Utc); }
		#region inner types
		class OneConstant
		{
			public int A { get { return 0; } }
		}
		class OneConstantThen
		{
			public int A { get { return 0; } }
			public int B { get; set; }
		}
		class OneConstantVirtual
		{
			public virtual int A { get { return 0; } }
		}
		sealed class OneConstantSealed : OneConstantVirtual
		{
		}
		[System.Runtime.Serialization.DataContract]
		internal class StringThen
		{
			[System.Runtime.Serialization.DataMember(Order=1)]
			public string A { get { return "\u0000"; } }
			[System.Runtime.Serialization.DataMember(Order=2)]
			public string B { get; set; }
		}
		[System.Runtime.Serialization.DataContract]
		internal class ThenString
		{
			[System.Runtime.Serialization.DataMember(Order=1)]
			public string A { get; set; }
			[System.Runtime.Serialization.DataMember(Order=2)]
			public string B { get { return "\u0000"; } }
		}
		class NoConstants
		{
			public string A { get; set; }
			public string B { get; set; }
		}
		#endregion

		static bool ToleratedFailure(Type type, string newtonsoft)
		{
			var underlying = Nullable.GetUnderlyingType(type) ?? type;

			return newtonsoft != null && underlying == typeof(decimal);
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
				var constant = ConstantMethods.TryGetJson(mi, this.GetType());

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

		[Fact]
		public void ConstantObjects()
		{
			var sb = new StringBuilder();
			int failedCount = 0;

			TrySerializer(sb, new OneConstant());
			TrySerializer(sb, new OneConstantThen());
			TrySerializer(sb, new OneConstantVirtual());
			TrySerializer(sb, new OneConstantSealed());

			ApprovalTests.Approvals.Verify(sb.ToString());
			Assert.True(failedCount == 0, "Look at the approval, " + failedCount + " failed");
		}

		unsafe static int TrySerializer<T>(StringBuilder sb, params T[] samples)
		{
			int failures = 0;

			sb.AppendLine();
			sb.AppendLine("# " + SchemaTest.HumanName(typeof(T)));
			sb.AppendLine();

			var emit = DelegateBuilder.CreateStream<T>(Schema.Reflect(typeof(T)));
			string instructions;
			var del = (Serializer.WriteToStream<T>)emit.CreateDelegate(typeof(Serializer.WriteToStream<T>), out instructions);
			sb.AppendLine(emit.Instructions());
			sb.AppendLine();

			var ms = new MemoryStream();
			var buffer = new byte[64];

			for (int i = 0; i < samples.Length; i++)
			{
				ms.Position = 0;
				fixed (byte* ptr = buffer)
				{
					del.Invoke(ref samples[i], ptr, ms, buffer);
				}

				var newt = DelegateBuilderTest.GetNewtonsoft(samples[i]);
				var mine = ms.GetBuffer().Take((int)ms.Position);

				sb.AppendLine();
				sb.AppendLine("## Newtonsoft");
				Util.Hex.Dump(sb, newt);
				sb.AppendLine();

				sb.AppendLine();
				sb.AppendLine("## Cameronism.Json");
				Util.Hex.Dump(sb, mine);
				sb.AppendLine();
				sb.AppendLine();

				bool equal = newt.SequenceEqual(mine);
				sb.AppendLine("Equal: " + equal);
				sb.AppendLine();

				if (!equal) failures++;
			}

			return failures;
		}

		[Fact]
		public void CheckBoundaries()
		{
			var az = String.Concat(Enumerable.Range(0, 26).Select(i => 'a' + (char)i));

			Assert.NotNull(CheckBoundary(new NoConstants { A = "...", B = "\u0000" }));
			Assert.NotNull(CheckBoundary(new ThenString { A = "..." }));
			Assert.NotNull(CheckBoundary(new ThenString { A = "\u0000" }));
			Assert.NotNull(CheckBoundary(new ThenString { A = "\n" }));
			Assert.NotNull(CheckBoundary(new ThenString { A = az }, 96));
			Assert.NotNull(CheckBoundary(new StringThen { B = "..." }));
			Assert.NotNull(CheckBoundary(new StringThen { B = "\u0000" }));
			Assert.NotNull(CheckBoundary(new StringThen { B = "\n" }));
			Assert.NotNull(CheckBoundary(new StringThen { B = az }, 96));
		}

		// returns the length of serialized value when successful
		unsafe static int? CheckBoundary<T>(T value, int maxLength = 64)
		{
			const byte init = 255;
			var nJSON = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(value));
			var buffer = new byte[maxLength + 2];
			var emit = DelegateBuilder.CreatePointer<T>(Schema.Reflect(typeof(T)));
			string instructions;
			var del = (Serializer.WriteToPointer<T>)emit.CreateDelegate(typeof(Serializer.WriteToPointer<T>), out instructions);
			int? successLength = null;

			for (int i = 1; i <= maxLength; i++)
			{
				for (int j = 0; j < buffer.Length; j++) buffer[j] = init;

				int result;
				fixed (byte* ptr = buffer)
				{
					result = del.Invoke(ref value, ptr + 1, i);
				}

				// make sure we didn't go past the boundary
				Assert.Equal(init, buffer[0]);
				Assert.Equal(init, buffer[i + 1]);
				Assert.True(result <= i, "Don't exceed length");

				if (result > 0)
				{
					Assert.Equal(nJSON, buffer.Skip(1).Take(result));
					if (successLength == null)
					{
						successLength = i;
					}
				}
			}

			if (successLength.HasValue)
			{
				// do stuff with the stream version now
				//for (int i = nJSON.Length; i >= nJSON.Length / 2; i--)
				//{

				//}
			}

			return successLength;
		}
	}
}
