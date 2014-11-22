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

		static void Run<T>(Action<Logger<T>> act, Action<List<FileStream>, DirectoryInfo> assert, int? flushRecordCount = null, long? maxFileSize = null, Action<FileStream> afterSend = null)
		{
			var files = new List<FileStream>();

			using (var dir = new DisposableDirectory())
			{
				using (var logger = Logger.Create<T>(
					fs =>
					{
						files.Add(fs);
						if (afterSend != null) afterSend(fs);
					}, 
					maxFileSize ?? short.MaxValue,
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

		public static string Read(Stream fs)
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

		[Fact]
		public void UseAfterDispose()
		{
			Logger<int> theLogger = null;

			Run<int>(
				flushRecordCount: 3,
				maxFileSize: ushort.MaxValue,
				act: (logger) =>
				{
					theLogger = logger;
				},
				assert: (files, dir) =>
				{
					Assert.InRange(theLogger.Position, 0, ushort.MaxValue);
					Assert.Equal(0, theLogger.RecordCount);
					Assert.Equal(true, theLogger.Disposed);

					Assert.Throws<ObjectDisposedException>(() =>
					{
						var i = 0;
						theLogger.Write(ref i);
					});

					Assert.Throws<ObjectDisposedException>(() =>
					{
						theLogger.Send();
					});
				});
		}

		[Fact]
		public void Rollover()
		{
			Run<Guid>(
				maxFileSize: 128,
				act: (logger) =>
				{
					// force an unreasonably small reserve to test rollver
					logger.ReservedRecordSize = 1;

					for (int i = 0; i < 10; i++)
					{
						var g = Guid.NewGuid();
						logger.Write(ref g);
					}
				},
				assert: (files, dir) =>
				{
					Assert.Equal(4, files.Count);
				});
		}

		[Fact]
		public void SendError()
		{
			var errors = new List<Tuple<Exception, string>>();
			var myError = new NotImplementedException();

			Run<Guid>(
				afterSend: fs => { throw myError; },
				act: (logger) =>
				{
					logger.ErrorLogger = (ex, msg) => errors.Add(Tuple.Create(ex, msg));

					for (int i = 0; i < 10; i++)
					{
						var g = Guid.NewGuid();
						logger.Write(ref g);
					}
				},
				assert: (files, dir) =>
				{
					Assert.Equal(1, errors.Count);
					Assert.ReferenceEquals(errors[0].Item1, myError);
				});

			int afterSendCount = 0;
			// do it again without custom error logger
			Run<Guid>(
				afterSend: fs => { afterSendCount++; throw myError; },
				act: (logger) =>
				{
					for (int i = 0; i < 10; i++)
					{
						var g = Guid.NewGuid();
						logger.Write(ref g);
					}
				},
				assert: (files, dir) =>
				{
					Assert.Equal(1, errors.Count);
					Assert.Equal(1, afterSendCount);
				});
		}

		[Fact]
		public void BasicTimer()
		{
			Logger theLogger = null;
			Run<Guid>(
				act: (logger) =>
				{
					theLogger = logger;
					logger.Interval = null;
					for (int i = 0; i < 10; i++)
					{
						var g = Guid.NewGuid();
						logger.Write(ref g);
					}
				},
				assert: (files, dir) =>
				{
					Assert.Null(theLogger.Interval);

					var ts = TimeSpan.FromHours(1);
					theLogger.Interval = ts;
					Assert.Equal(ts, theLogger.Interval);
				});
		}

		[Fact(Timeout=1000)] // real filesystem :(
		public void ActualTimer()
		{
			using (var ss = new SemaphoreSlim(1))
			{
				// acquire here and release when the timer goes
				ss.Wait();

				using (var dir = new DisposableDirectory())
				{
					var files = new List<FileStream>();

					var myException = new IndexOutOfRangeException();
					var errors = new List<Exception>();

					using (var logger = Logger.Create<Guid>(
						fs =>
						{
							files.Add(fs);

							ss.Release();
							if (files.Count == 1)
							{
								throw myException;
							}
						},
						ushort.MaxValue,
						logDirectory: dir.Info.FullName))
					{
						logger.ErrorLogger = (ex, msg) => errors.Add(ex);
						logger.Interval = TimeSpan.FromMilliseconds(1);
						var g = Guid.NewGuid();

						logger.Write(ref g);

						// wait for send to be invoked
						ss.Wait();

						// push another
						logger.Write(ref g);

						// wait again
						ss.Wait();
					}

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

					Assert.Equal(2, files.Count);
					Assert.Equal(1, errors.Count);
					Assert.Same(myException, errors[0]);
				}
			}
		}
	}
}
