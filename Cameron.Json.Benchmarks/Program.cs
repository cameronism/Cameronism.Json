using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewtonsoftJson = Newtonsoft.Json;
using JilJson = Jil.JSON;
using ProtoBufSerializer = ProtoBuf.Serializer;
using BrianarySerializer = FP.IO.Serialization.Serializer;
using SimpleSpeedTester.Core;
using System.Runtime.InteropServices;

namespace Json.Benchmarks
{
	class Program
	{
		class MyTestResult
		{
			public string Serializer;
			public string Case;
			public SimpleSpeedTester.Interfaces.ITestResultSummary Summary;
			public IReadOnlyList<long?> Lengths;
		}

		delegate void Benchy<T>(T item, UnmanagedMemoryStream stream);

		static NewtonsoftJson.JsonSerializer NewtonsoftSerializer = new NewtonsoftJson.JsonSerializer();
		void Newtonsoft<T>(T item, UnmanagedMemoryStream ms)
		{
			using (var tw = new StreamWriter(ms, Encoding.UTF8, DefaultStreamWriterBufferSize, true))
			{
				NewtonsoftSerializer.Serialize(tw, item, typeof(T));
			}
		}

		const int DefaultStreamWriterBufferSize = 1024;

		void Jil<T>(T item, UnmanagedMemoryStream ms)
		{
			using (var tw = new StreamWriter(ms, Encoding.UTF8, DefaultStreamWriterBufferSize, true))
			{
				JilJson.Serialize(item, tw);
			}
		}

		void ProtoBuf<T>(T item, UnmanagedMemoryStream ms)
		{
			ProtoBufSerializer.Serialize(ms, item);
		}

		void Brianary<T>(T item, UnmanagedMemoryStream ms)
		{
			BrianarySerializer.Serialize(ms, item);
		}

		unsafe void Cameronism<T>(T item, UnmanagedMemoryStream ms)
		{
			int resul = Cameronism.Json.Convert.Serialize(item, ms);
			if (resul <= 0) throw new InsufficientMemoryException();
		}

		unsafe void BenchAll<T>(T item, IntPtr ptr, int length, Random random, int testRuns = 16, string name = null)
		{
			name = name ?? typeof(T).Name;

			ConsoleDump.Extensions.Dump(
				BenchAll(name, item, ptr, length, random, testRuns).Select(r =>
				new {
					r.Serializer,
					Length = r.Lengths.FirstOrDefault(),
					AverageExecutionTime = (int)Math.Round(r.Summary.AverageExecutionTime),
					r.Summary.Failures,
				}).OrderBy(r => r.Failures == 0 ? r.AverageExecutionTime : int.MaxValue),
				name);
		}

		unsafe List<MyTestResult> BenchAll<T>(string name, T item, IntPtr ptr, int length, Random random, int testRuns)
		{
			var methods = new Benchy<T>[] {
				Newtonsoft,
				Jil,
				ProtoBuf,
				Brianary,
				Cameronism,
			};

			var testGroup = new TestGroup(name);

			return methods
				.OrderBy(_ => random.Next())
				.Select(method =>
				{
					var positions = new List<long?>();
					var result = new MyTestResult
					{
						Case = testGroup.Name,
						Serializer = method.Method.Name,
						Lengths = positions,
					};

					Action plan = () =>
					{
						using (var ms = new UnmanagedMemoryStream((byte*)ptr, 0, length, FileAccess.Write))
						{
							method.Invoke(item, ms);
							positions.Add(ms.Position);
						}
					};

					var serializeResult =
						testGroup
							.Plan(method.Method.Name, plan, testRuns)
							.GetResult();

					var filter = new SimpleSpeedTester.Core.OutcomeFilters.ExcludeMinAndMaxTestOutcomeFilter();
					var summary = serializeResult.GetSummary(filter);

					result.Summary = summary;
					positions.Sort();
					if (positions.Count > 1 && positions.First() != positions.Last())
					{
						throw new Exception("Inconsistent results from " + method.Method.Name);
					}

					return result;
				})
				.ToList();
		}

		static IEnumerable<T> Repeat<T>(IEnumerable<T> xs, int count)
		{
			for (int i = 0; i < count; i++)
			{
				foreach (var x in xs)
				{
					yield return x;
				}
			}
		}

		static void Main(string[] args)
		{
			var rand = new Random(42);
			int length = 256 * 1024 * 1024;
			var instance = new Program();

			IntPtr ptr = IntPtr.Zero;
			try
			{
				ptr = Marshal.AllocHGlobal(length);

				var first1024 = Repeat(Enumerable.Range(0, 1024), 1024).ToArray();
				instance.BenchAll(first1024, ptr, length, rand, name: "[0, 1024) * 1024");

				instance.BenchAll(first1024.Select(i => i.ToString()).ToList(), ptr, length, rand, name: "[0, 1024) * 1024 - as strings");

				instance.BenchAll(first1024.Select(i =>
				{
					return new ImpressionDetail
					{
						FPSessionId= Guid.NewGuid(),
						Timestamp = DateTime.UtcNow,
						UserAgent = i + "/" + i,
						MessageId = (uint)i,
						SessionValues = new Dictionary<string,string>(),
						Groups = new List<ImpressionDetail.GroupInfo>(),
					};
				}).ToDictionary(id => id.FPSessionId.ToString()), ptr, length, rand, name: "dictionary of Impression Detail", testRuns: 4);
			}
			finally
			{
				if (ptr != IntPtr.Zero) Marshal.FreeHGlobal(ptr);
			}
		}
	}
}
