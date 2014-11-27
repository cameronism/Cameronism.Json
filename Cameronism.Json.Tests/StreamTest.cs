using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cameronism.Json.Tests
{
	public class StreamTest : IDisposable
	{
		public class StreamSpy : Stream
		{
			readonly private Stream that;
			readonly private TextWriter log;

			public StreamSpy(Stream other, TextWriter log)
			{
				this.that = other;
				this.log = log;
			}

			public Stream Underlying { get { return that; } }
			public TextWriter Log { get { return log; } }

			object[] Args(params object[] args)
			{
				return args;
			}
			void Spy([CallerMemberName]string member = null)
			{
				log.WriteLine("Stream." + member + "()");
			}
			void Spy(object[] arguments, [CallerMemberName]string member = null)
			{
				var args = String.Join(", ", arguments.Select(arg =>
				{
					var buffer = arg as byte[];
					if (buffer != null)
					{
						return new { Type = "byte[]", Length = buffer.Length }.ToString();
					}
					if (arg == null)
					{
						return "`null`";
					}

					return arg.ToString();
				}));

				log.WriteLine("Stream." + member + "(" + args + ")");
			}

			public override bool CanRead { get { Spy(); return that.CanRead; } } 
			public override bool CanSeek { get { Spy(); return that.CanSeek; } }
			public override bool CanWrite { get { Spy(); return that.CanWrite; } }
			public override void Flush() { Spy(); that.Flush(); }
			public override long Length { get { Spy(); return that.Length; } }

			public override long Position
			{
				get { Spy(); return that.Position; }
				set { Spy(Args(value)); that.Position = value; }
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				Spy(Args(buffer, offset, count));
				return that.Read(buffer, offset, count);
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				Spy(Args(offset, origin));
				return that.Seek(offset, origin);
			}

			public override void SetLength(long value)
			{
				Spy(Args(value));
				that.SetLength(value);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				Spy(Args(buffer, offset, count));
				that.Write(buffer, offset, count);
			}
		}

		byte[] _Buffer = new byte[64];

		static DelegateBuilderTest.B<T> A<T>(T i)
		{
			return new DelegateBuilderTest.B<T> { i = i };
		}

		static DelegateBuilderTest.B<T1, T2> A<T1, T2>(T1 i, T2 j)
		{
			return new DelegateBuilderTest.B<T1, T2> { i = i, j = j };
		}

		[Fact]
		public void SimpleValues()
		{
			var sw = new StringWriter();
			var ms = new StreamSpy(new MemoryStream(), sw);

			AssertNullableValue(1, "1", ms);
			AssertNullableValue(true, "true", ms);
			AssertNullableValue(false, "false", ms);

			var g = Guid.Parse("171d7051-0c40-4475-9568-e8df9da1fb53");
			AssertNullableValue(g, "\"" + g + "\"", ms);

			var d = new DateTime(2014, 11, 18, 7, 42, 41, 111, DateTimeKind.Utc);
			AssertNullableValue(d, Newtonsoft.Json.JsonConvert.SerializeObject(d), ms);

			AssertEqual("\"1.2.3.4\"", System.Net.IPAddress.Parse("1.2.3.4"), ms);

			ApprovalTests.Approvals.Verify(sw.ToString());
		}

		[Fact]
		public void EnumerableValues()
		{
			var sw = new StringWriter();
			var ss = new StreamSpy(new MemoryStream(), sw);

			AssertEqual("[1]", new List<int> { 1 }, ss);
			AssertEqual("[12]", new List<int> { 12 }, ss);

			AssertEqual("[1,null]", new List<int?> { 1, null }, ss);

			AssertEqual("[1]", new[] { 1 }, ss);
			AssertEqual("[1,null]", new int?[] { 1, null }, ss);

			AssertEqual("{\"i\":1}", A(1), ss);
			AssertEqual("{\"i\":0.5,\"j\":\"00000000-0000-0000-0000-000000000000\"}", A(0.5, Guid.Empty), ss);

			AssertEqual("{\"i\":1,\"j\":2}", new Dictionary<char, int> { 
				{ 'i', 1 },
				{ 'j', 2 },
			}, ss);

			AssertEqual("[{\"i\":1}]", new[] { A(1) }, ss);
			AssertEqual("{\"i\":[1]}", A(new[] { 1 }), ss);

			ApprovalTests.Approvals.Verify(sw.ToString());
		}

		void AssertEqual<T>(string expected, T value, StreamSpy ss)
		{
			ss.Log.WriteLine("# " + expected);

			string instructions;
			Assert.Equal(expected, ToJson(value, ss, out instructions));

			ss.Log.WriteLine();
		}

		[Fact]
		public void CourtesyFlush()
		{
			var sw = new StringWriter();
			var ss = new StreamSpy(new MemoryStream(), sw);

			string instructions;

			var xs = Enumerable.Repeat(1, 64);
			var json = "[" + String.Join(",", xs) + "]";
			sw.WriteLine("# " + @"[ + String.Join(',', Enumerable.Repeat(1, 64)) + ]");
			Assert.Equal(json, ToJson(xs, ss, out instructions));

			sw.WriteLine();

			xs = Enumerable.Repeat(int.MinValue, 64);
			json = "[" + String.Join(",", xs) + "]";
			sw.WriteLine("# " + @"[ + String.Join(',', Enumerable.Repeat(int.MinValue, 64)) + ]");
			Assert.Equal(json, ToJson(xs, ss, out instructions));

			// poke around the boundaries of filling up the 64 byte _Buffer
			foreach (int len in new[] { 0, 1, 44, 45, 46, 89, 90, 91 })
			{
				sw.WriteLine();

				var bs = new byte[len];
				json = "\"" + Convert.ToBase64String(bs) + "\"";
				sw.WriteLine("# [0,...] -> base 64 -- " + len);
				Assert.Equal(json, ToJson(bs, ss, out instructions));
			}

			ApprovalTests.Approvals.Verify(sw.ToString());
		}

		[Fact]
		public void StreamInstructions()
		{
			var sb = new StringBuilder();

			// not trying to approve the result of this stream spy since we're only interested in IL here
			var sw = new StringWriter();
			var ms = new StreamSpy(new MemoryStream(), sw);

			string instructions;

			sb.AppendLine("# IEnumerable<int>");
			sb.AppendLine();
			ToJson<IEnumerable<int>>(null, ms, out instructions);
			sb.AppendLine(instructions);

			sb.AppendLine();
			sb.AppendLine("# string");
			sb.AppendLine();
			ToJson<string>(null, ms, out instructions);
			sb.AppendLine(instructions);

			sb.AppendLine();
			sb.AppendLine("# string[]");
			sb.AppendLine();
			ToJson<string[]>(null, ms, out instructions);
			sb.AppendLine(instructions);

			sb.AppendLine();
			sb.AppendLine("# List<string>");
			sb.AppendLine();
			ToJson<List<string>>(null, ms, out instructions);
			sb.AppendLine(instructions);

			ApprovalTests.Approvals.Verify(sb.ToString());
		}

		[Fact]
		public void DirectStrings()
		{
			var sw = new StringWriter();
			var ss = new StreamSpy(new MemoryStream(), sw);

			string theValue;

			theValue = null;
			AssertString(theValue, "null", ss);

			theValue = "";
			AssertString(theValue, "empty string", ss);

			theValue = "a";
			AssertString(theValue, "a", ss);

			theValue = String.Concat(Enumerable.Repeat("a", 62));
			AssertString(theValue, "a * 62", ss);

			theValue = String.Concat(Enumerable.Repeat("a", 63));
			AssertString(theValue, "a * 63", ss);

			theValue = String.Concat(Enumerable.Repeat("a", 124));
			AssertString(theValue, "a * 124", ss);

			theValue = String.Concat(Enumerable.Repeat("a", 125));
			AssertString(theValue, "a * 125", ss);

			theValue = String.Concat(Enumerable.Repeat("a", 126));
			AssertString(theValue, "a * 126", ss);

			theValue = ((char)0).ToString();
			AssertString(theValue, "(char)0", ss);

			theValue = String.Concat(Enumerable.Repeat(((char)0).ToString(), 10));
			AssertString(theValue, "(char)0 * 10", ss);

			theValue = String.Concat(Enumerable.Repeat(((char)0).ToString(), 11));
			AssertString(theValue, "(char)0 * 11", ss);

			theValue = String.Concat(Enumerable.Repeat("\"", 31));
			AssertString(theValue, "\" * 31", ss);

			theValue = String.Concat(Enumerable.Repeat("\"", 32));
			AssertString(theValue, "\" * 32", ss);

			theValue = String.Concat(Enumerable.Repeat("\u00EE", 32));
			AssertString(theValue, "\\u00EE * 32", ss);

			theValue = String.Concat(Enumerable.Repeat("𝕡𝕣𝕚𝕫𝕖", 8));
			AssertString(theValue, "unicode 'prize' * 8", ss);

			AssertValue(new[] { "one" }, "[ one ]", ss);

			List<string> xs;

			xs = Enumerable.Range(0, 26).Select(i => i.ToString()).ToList();
			AssertValue(xs, "[ \"0\", ... , \"25\" ]", ss);

			xs = Enumerable.Range(0, 27).Select(i => i.ToString()).ToList();
			AssertValue(xs, "[ \"0\", ... , \"26\" ]", ss);

			xs = Enumerable.Range(0, 192).Select(i => ((char)i).ToString()).ToList();
			AssertValue(xs, "[ \"\\u0000\", ... , \"\\u00bf\" ]", ss);

			xs = Enumerable.Range(0, 26).Select(i => (string)null).ToList();
			AssertValue(xs, "[ null * 25 ]", ss);

			AssertValue(A("b", "c"), "{ i: b, j: c }", ss);

			ApprovalTests.Approvals.Verify(sw.ToString());
		}

		[Fact]
		public unsafe void PublicStreamCall()
		{
			var ms = new MemoryStream();
			var g = Guid.NewGuid();
			string json;
			var buffer = new byte[1024];

			Serializer.Serialize(g, ms, buffer);
			ms.Position = 0;
			json = LoggerTest.Read(ms);
			var n1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Guid>(json);
			Assert.Equal(g, n1);

			ms.SetLength(0);
			Serializer.Serialize(g, ms, buffer);
			ms.Position = 0;
			json = LoggerTest.Read(ms);
			var n2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Guid>(json);
			Assert.Equal(g, n2);

			Assert.Throws<ArgumentNullException>(() => Serializer.Serialize(g, ms, null));
			Assert.Throws<ArgumentOutOfRangeException>(() => Serializer.Serialize(g, ms, new byte[1023]));

			// zero
			for (int i = 0; i < buffer.Length; i++) buffer[i] = 0;

			fixed (byte* ptr = buffer)
			{
				var ums = new UnmanagedMemoryStream(ptr, buffer.Length);
				Serializer.Serialize(g, ums);

				// just sanity check the length - ascii
				Assert.Equal(json.Length, ums.Position);
			}
		}

		void AssertString(string value, string description, StreamSpy ss)
		{
			string instructions;
			var json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
			ss.Log.WriteLine("# " + description);
			Assert.Equal(json, ToJson(value, ss, out instructions));
			ss.Log.WriteLine();
		}

		void AssertValue<T>(T value, string description, StreamSpy ss)
		{
			string instructions;
			var json = Newtonsoft.Json.JsonConvert.SerializeObject(value, new NewtonsoftConverters.IPAddressConverter());
			ss.Log.WriteLine("# " + description);
			Assert.Equal(json, ToJson(value, ss, out instructions));
			ss.Log.WriteLine();
		}

		void AssertNullableValue<T>(T value, string expected, StreamSpy ms) where T : struct
		{
			ms.Log.WriteLine("# " + expected);
			Assert.Equal(expected, ToJson(value, ms));

			// FIXME submit a sigil bug with testcase for this
			DelegateBuilder.UseSigilVerify = false;

			ms.Log.WriteLine();
			ms.Log.WriteLine("# " + expected);
			Assert.Equal(expected, ToJson((T?)value, ms));

			ms.Log.WriteLine();
			ms.Log.WriteLine("# null");
			Assert.Equal("null", ToJson((T?)null, ms));
			DelegateBuilder.UseSigilVerify = true;

			ms.Log.WriteLine();
		}

		unsafe string ToJson<T>(T value, StreamSpy ms)
		{
			string instructions;
			return ToJson(value, ms, out instructions);
		}

		unsafe string ToJson<T>(T value, StreamSpy ss, out string instructions)
		{
			var ms = ss.Underlying as MemoryStream;
			ms.Position = 0;
			try
			{
				var schema = Schema.Reflect(typeof(T));
				var emit = DelegateBuilder.CreateStream<T>(schema);
				var del = emit.CreateDelegate<Serializer.WriteToStream<T>>(out instructions);
				fixed (byte* ptr = _Buffer)
				{
					del.Invoke(ref value, ptr, ss, _Buffer);
				}
			}
			catch (Sigil.SigilVerificationException sve)
			{
				Trace.WriteLine(sve);
				instructions = sve.GetDebugInfo();
				throw;
			}
			return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Position);
		}

		public void Dispose()
		{
			_Buffer = null;
		}
	}
}
