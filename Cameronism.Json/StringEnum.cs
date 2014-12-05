using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cameronism.Json
{
	unsafe internal class StringEnum
	{
		public struct Lookup
		{
			readonly byte* _TableStart;
			readonly byte* _StringStart;

			public byte* TableStart { get { return _TableStart; } }
			public byte* StringStart { get { return _StringStart; } }

			public Lookup(byte* tableStart, byte* stringStart)
			{
				_TableStart = tableStart;
				_StringStart = stringStart;
			}

			public void FindSequential(ulong value, out byte* str, out int length)
			{
				byte* key = _TableStart + value * 4;
				if (key >= _StringStart)
				{
					str = null;
					length = 0;
					return;
				}

				str = _StringStart + *(ushort*)key;
				length = *(ushort*)(key + 2);
			}
		}

		public static Lookup? GenerateSequentialLookup(Type type, byte* destination, int available, out int used)
		{
			used = 0;

			var maybeMaxValue = GetMaxValue(type);
			if (!maybeMaxValue.HasValue) return null;
			var maxValue = maybeMaxValue.Value + 1;

			byte* endStrings = destination + available;
			byte* stringPtr = (byte*)(destination + (maxValue * 4));
			var stringsStart = stringPtr;

			if (stringPtr >= destination + available) return null;

			for (ulong i = 0; i < maxValue; i++)
			{
				long offset = stringPtr - stringsStart;
				if (offset > ushort.MaxValue) return null;

				string enumName = Enum.GetName(type, i);
				if (enumName == null)
				{
					*(uint*)(destination + (i * 4)) = 0;
					continue;
				}

				*(ushort*)(destination + (i * 4)) = (ushort)offset;

				int result = ConvertUTF.WriteStringUtf8(enumName, stringPtr, (int)(endStrings - stringPtr), useQuote: false);
				if (result <= 0 || result >= ushort.MaxValue) return null;

				*(ushort*)(destination + (i * 4) + 2) = (ushort)result;
				stringPtr += result;
			}

			used = (int)(stringPtr - destination);
			return new Lookup(destination, stringsStart);
		}

		public static ulong? GetMaxValue(Type type)
		{
			bool any = false;
			ulong maxValue = 0;
			foreach (var val in Enum.GetValues(type))
			{
				any = true;
				var num = Convert.ToUInt64(val);
				if (num > maxValue) maxValue = num;
			}

			return any ? maxValue : (ulong?)null;
		}
	}
}
