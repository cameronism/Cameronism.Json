using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cameronism.Json
{
	public static class Logger
	{
		public static Logger<T> Create<T>(Action<FileStream> send, int maxFileSize, int? flushRecordCount = null, string logNameFormat = null, string logDirectory = null)
		{
			var writer = Serializer.GetDelegate<T>();
			return new Logger<T>(send, writer, maxFileSize, flushRecordCount, logNameFormat, logDirectory);
		}
	}

	public unsafe sealed class Logger<T> : IDisposable
	{
		#region properties
		public int MaximumFileSize { get; set; }
		public int ReservedRecordSize { get; set; }
		public int? FlushRecordCount { get; set; }
		public string LogDirectory { get; set; }
		public string LogNameFormat { get; set; }
		public bool SendOnWorkerThread { get; set; }
		public Action<Exception, string> ErrorLogger { get; set; }
		#endregion
		
		#region fields
		readonly object _Gate = new object();
		readonly Serializer.LowWriter<T> _Writer;
		Action<FileStream> _Sender;
		FileStream _Stream;
		MemoryMappedFile _Mapped;
		MemoryMappedViewAccessor _Accessor;
		int _RecordCount;
		bool _Disposed;
		long _Position;
		long _Length;
		#endregion fields
		
		internal Logger(Action<FileStream> sender, Serializer.LowWriter<T> writer, int maxFileSize, int? flushRecordCount, string logNameFormat, string logDirectory)
		{
			_Sender = sender;
			_Writer = writer;
			MaximumFileSize = maxFileSize;
			FlushRecordCount = flushRecordCount;
			LogNameFormat = logNameFormat ?? "{0:yyyy'-'MM'-'dd'T'HH'-'mm'-'ss'.'fff}.log.json";
			LogDirectory = logDirectory;
			SendOnWorkerThread = true;
			ReservedRecordSize = 64;
			
			InitFile();
		}

		/// <summary>Queue a record</summary>
		public void Write(ref T value)
		{
			lock (_Gate)
			{
				if (_Disposed) throw new ObjectDisposedException("Logger");
		
				int attempt = 0;
				
				do
				{
					var position = _Position;
					byte* destination = (byte*)_Accessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + position;
					var result = _Writer(ref value, destination, (int)(_Length - (position + 1))); // + 1 for trailing comma (or ']')
						
					if (result > 0)
					{
						*(destination + result) = (byte)',';
						_Position += result + 1;
						_RecordCount++;
						TrySend();
						return;
					}
					
					if (!Rollover(-1 * result)) throw new Exception("Failed to serialize to new file");
				} while (++attempt < 4);
				
				throw new Exception("Failed to serialize.  Attempts: " + attempt);
			}
		}
		
		void InitFile()
		{
			long length = MaximumFileSize;
			
			var fs = File.Open(GetFilename(), FileMode.CreateNew, FileAccess.ReadWrite);
			var mmf = MemoryMappedFile.CreateFromFile(fs, Guid.NewGuid().ToString(), length, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.None, leaveOpen: true);
			var accessor = mmf.CreateViewAccessor();
			
			_Stream = fs;
			_Mapped = mmf;
			_Accessor = accessor;
			_Length = length;

			// open the array
			WriteByte(0, (byte)'[');
			_Position = 1;
		}
		
		string GetFilename()
		{
			var dir = LogDirectory ?? Path.GetTempPath();
			var name = String.Format(LogNameFormat, DateTime.UtcNow);
			return Path.Combine(dir, name);
		}
		
		// send if appropriate
		void TrySend()
		{
			if (_RecordCount >= FlushRecordCount || _Position + ReservedRecordSize >= _Length)
			{
				SendInternal();
				InitFile();
			}
		}
		
		bool Rollover(int minimumRequired)
		{
			if (_RecordCount == 0) return false;
			
			SendInternal();
			InitFile();
			return _Length >= minimumRequired;
		}
		
		void SendInternal()
		{
			long position = _Position;

			// handle empty array
			if (_RecordCount == 0) position++;
			
			// close the array
			WriteByte(position - 1, (byte)']');
			
			_Accessor.Dispose();
			_Mapped.Dispose();
			_Accessor = null;
			_Mapped = null;
			
			var fs = _Stream;
			_Stream = null;
			fs.SetLength(position);
			
			if (SendOnWorkerThread)
			{
				ThreadPool.QueueUserWorkItem(InvokeSend, fs);
			}
			else
			{
				try
				{
					_Sender.Invoke(fs);
				}
				catch (Exception ex)
				{
					LogError(ex);
				}
			}
		}
		
		void InvokeSend(object state)
		{
			var fs = (FileStream)state;
			try
			{
				_Sender.Invoke(fs);
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}
		
		void WriteByte(long position, byte value)
		{
			byte* destination = (byte*)_Accessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + position;
			*destination = value;
		}
		
		void LogError(Exception ex, [CallerMemberName]string caller = null)
		{
			var handler = ErrorLogger;
			if (handler != null)
			{
				handler.Invoke(ex, caller);
				return;
			}

			Trace.TraceError(caller + Environment.NewLine + ex.ToString());
		}
		
		/// <summary>Send pending records</summary>
		public void Send()
		{
			lock (_Gate)
			{
				if (_Disposed) throw new ObjectDisposedException("Logger");
				
				SendInternal();
				InitFile();
			}
		}
			
		/// <summary>Send pending records and stop accepting more</summary>
		public void Dispose()
		{
			lock (_Gate)
			{
				_Disposed = true;
				if (_RecordCount > 0) SendInternal();
				
				TryDispose(_Accessor);
				TryDispose(_Mapped);
				TryDispose(_Stream);
				
				_RecordCount = 0;
				_Accessor = null;
				_Mapped = null;
				_Stream = null;
				//_Sender = null;
			}
		}
		
		void TryDispose(IDisposable d)
		{
			if (d != null)
			{
				try
				{
					d.Dispose();
				}
				catch (Exception ex)
				{
					LogError(ex);
				}
			}
		}
	}
}
