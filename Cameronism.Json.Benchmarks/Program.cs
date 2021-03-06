﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewtonsoftJson = Newtonsoft.Json;
using JilJson = Jil.JSON;
using ProtoBufSerializer = ProtoBuf.Serializer;
using SimpleSpeedTester.Core;
using System.Runtime.InteropServices;
#if FPBENCH
using BrianarySerializer = FP.IO.Serialization.Serializer;
#endif

namespace Cameronism.Json.Benchmarks
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

		static Encoding _Utf8 = new UTF8Encoding(false);

		static NewtonsoftJson.JsonSerializer NewtonsoftSerializer = new NewtonsoftJson.JsonSerializer();
		void Newtonsoft<T>(T item, UnmanagedMemoryStream ms)
		{
			using (var tw = new StreamWriter(ms, _Utf8, DefaultStreamWriterBufferSize, true))
			{
				NewtonsoftSerializer.Serialize(tw, item, typeof(T));
			}
		}

		const int DefaultStreamWriterBufferSize = 1024;

		void Jil<T>(T item, UnmanagedMemoryStream ms)
		{
			using (var tw = new StreamWriter(ms, _Utf8, DefaultStreamWriterBufferSize, true))
			{
				JilJson.Serialize(item, tw);
			}
		}

		void ProtoBuf<T>(T item, UnmanagedMemoryStream ms)
		{
			ProtoBufSerializer.Serialize(ms, item);
		}

#if FPBENCH
		void Brianary<T>(T item, UnmanagedMemoryStream ms)
		{
			BrianarySerializer.Serialize(ms, item);
		}
#endif

		unsafe void CameronismPointer<T>(T item, UnmanagedMemoryStream ms)
		{
			int resul = Cameronism.Json.Serializer.Serialize(item, ms);
			if (resul <= 0) throw new InsufficientMemoryException();
		}

		static byte[] _Buffer;
		unsafe void CameronismStream<T>(T item, UnmanagedMemoryStream ms)
		{
			Cameronism.Json.Serializer.Serialize(item, ms, _Buffer);
		}

		unsafe void CameronismFakeStream<T>(T item, UnmanagedMemoryStream ms)
		{
			var buffer = _Buffer;
			fixed (byte* ptr = _Buffer)
			{
				int result = Cameronism.Json.Serializer.Serialize(item, ptr, buffer.Length);
				if (result <= 0) throw new InsufficientMemoryException();

				ms.Write(buffer, 0, result);

				//// slow this down on purpose because it's freaking fast right now
				//for (int i = 0; i < 15; i++)
				//{
				//	ms.Position = 0;
				//	ms.Write(buffer, 0, result);
				//}
			}
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
#if FPBENCH
				Brianary,
#endif
				CameronismPointer,
				CameronismStream,
				CameronismFakeStream,
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
			_Buffer = new byte[length];

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
