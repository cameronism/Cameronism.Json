using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cameronism.Json.Tests
{
	public class LoggerTest
	{
		class DisposableDirectory : IDisposable
		{
			public DisposableDirectory(string startDir = null)
			{
				var name = Path.Combine(startDir ?? Path.GetTempPath(), "cj-" + Guid.NewGuid().ToString());
				var di = new DirectoryInfo(name);
				di.Create();
				Info = di;
			}

			public DirectoryInfo Info { get; private set; }

			public void Dispose()
			{
				int attempts = 0;
				do {
					try
					{
						Info.Delete(recursive: true);
						return;
					}
					catch (DirectoryNotFoundException)
					{
						return;
					}
					catch (Exception)
					{
						Thread.Sleep(10);
					}
				} while(++attempts < 42);

				Trace.TraceWarning("temp directory could not be deleted " + Info.FullName);
			}
		}
		static string GetLogName(DateTime dt)
		{
			var g = Guid.NewGuid();
			return String.Format("{0:HH'-'mm'-'ss'.'fffffff}.{1}.log.json", dt, g.ToString().Substring(0, 8));
		}

		static void Run<T>(Action<Logger<T>> act, Action<List<FileStream>, DirectoryInfo> assert, int? flushRecordCount = null)
		{
			var files = new List<FileStream>();

			using (var dir = new DisposableDirectory())
			{
				using (var logger = Logger.Create<T>(fs => files.Add(fs), short.MaxValue, 
					logDirectory: dir.Info.FullName, 
					flushRecordCount: flushRecordCount,
					logNamer: GetLogName))
				{
					logger.SendOnWorkerThread = false;

					act(logger);
				}

				assert(files, dir.Info);

				foreach (var fs in files)
				{
					try
					{
						fs.Dispose();
					}
					catch (Exception)
					{
						// don't care
					}
				}
			}
		}

		static string Read(FileStream fs)
		{
			using (var rd = new StreamReader(fs, Encoding.UTF8, false, 4096, leaveOpen: true))
			{
				return rd.ReadToEnd();
			}
		}

		[Fact]
		public void EmptyDir()
		{
			Run<int>(
				act: (logger) =>
				{
					// do nothing
				},
				assert: (files, dir) =>
				{
					Assert.Equal(0, files.Count); // there should be nothing
				});
		}

		[Fact]
		public void SendOnce()
		{
			Run<int>(
				act: (logger) =>
				{
					logger.Send();
				},
				assert: (files, dir) =>
				{
					Assert.Equal(1, files.Count); // there should be one
					Assert.Equal("[]", Read(files[0])); // empty
				});
		}

		[Fact]
		public void One()
		{
			Run<int>(
				act: (logger) =>
				{
					int i = 1;
					logger.Write(ref i);
				},
				assert: (files, dir) =>
				{
					Assert.Equal(1, files.Count); // there should be one
					Assert.Equal("[1]", Read(files[0]));
				});
		}

		[Fact]
		public void Two()
		{
			Run<int>(
				act: (logger) =>
				{
					int i;
					i = 1;
					logger.Write(ref i);
					i = 2;
					logger.Write(ref i);
				},
				assert: (files, dir) =>
				{
					Assert.Equal(1, files.Count); // there should be one
					Assert.Equal("[1,2]", Read(files[0]));
				});
		}

		[Fact]
		public void TwoSplit()
		{
			Run<int>(
				act: (logger) =>
				{
					int i;
					i = 1;
					logger.Write(ref i);
					logger.Send();
					i = 2;
					logger.Write(ref i);
				},
				assert: (files, dir) =>
				{
					Assert.Equal(2, files.Count);
					Assert.Equal("[1]", Read(files[0]));
					Assert.Equal("[2]", Read(files[1]));
				});
		}

		[Fact]
		public void Limit1()
		{
			const int count = 10;

			Run<int>(
				flushRecordCount: 1,
				act: (logger) =>
				{
					for (int i = 0; i < count; i++)
					{
						logger.Write(ref i);
					}
				},
				assert: (files, dir) =>
				{
					Assert.Equal(count, files.Count);

					for (int i = 0; i < count; i++)
					{
						Assert.Equal("[" + i + "]", Read(files[i]));
					}
				});
		}

		[Fact]
		public void Limit2()
		{
			Run<int>(
				flushRecordCount: 2,
				act: (logger) =>
				{
					for (int i = 0; i < 10; i++)
					{
						logger.Write(ref i);
					}
				},
				assert: (files, dir) =>
				{
					Assert.Equal(5, files.Count);

					Assert.Equal("[0,1]", Read(files[0]));
					Assert.Equal("[2,3]", Read(files[1]));
					Assert.Equal("[4,5]", Read(files[2]));
					Assert.Equal("[6,7]", Read(files[3]));
					Assert.Equal("[8,9]", Read(files[4]));
				});
		}

		[Fact]
		public void Limit3()
		{
			Run<int>(
				flushRecordCount: 3,
				act: (logger) =>
				{
					for (int i = 0; i < 10; i++)
					{
						logger.Write(ref i);
					}
				},
				assert: (files, dir) =>
				{
					Assert.Equal(4, files.Count);

					Assert.Equal("[0,1,2]", Read(files[0]));
					Assert.Equal("[3,4,5]", Read(files[1]));
					Assert.Equal("[6,7,8]", Read(files[2]));
					Assert.Equal("[9]", Read(files[3]));
				});
		}

		[Fact]
		public void Limit3Send()
		{
			Run<int>(
				flushRecordCount: 3,
				act: (logger) =>
				{
					for (int i = 0; i < 10; i++)
					{
						logger.Write(ref i);
					}
					logger.Send();
					logger.Send();
				},
				assert: (files, dir) =>
				{
					Assert.Equal(5, files.Count);

					Assert.Equal("[0,1,2]", Read(files[0]));
					Assert.Equal("[3,4,5]", Read(files[1]));
					Assert.Equal("[6,7,8]", Read(files[2]));
					Assert.Equal("[9]", Read(files[3]));
					Assert.Equal("[]", Read(files[4]));
				});
		}
	}
}
