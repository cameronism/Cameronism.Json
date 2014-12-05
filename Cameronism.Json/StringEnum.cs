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

			public void FindIndexed(ulong value, out byte* str, out int length)
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

			public void FindSequential(uint value, out byte* str, out int length)
			{
				byte* lookup = _TableStart;
				byte* stop = _StringStart;

				while (lookup < stop)
				{
					if (*(uint*)lookup == value)
					{
						str = _StringStart + *(ushort*)(lookup + 4);
						length = *(ushort*)(lookup + 6);
						return;
					}

					lookup += 8;
				}

				str = null;
				length = 0;
				return;
			}
		}

		public static Lookup? GenerateIndexedLookup(Type type, byte* destination, int available, out int used)
		{
			used = 0;

			int valueCount;
			ulong maxValue;
			DescribeEnum(type, out valueCount, out maxValue);
			if (valueCount == 0) return null;
			maxValue++;

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

		public static Lookup? GenerateSortedLookup(Type type, byte* destination, int available, out int used)
		{
			used = 0;

			int valueCount;
			ulong maxValue;
			DescribeEnum(type, out valueCount, out maxValue);
			if (valueCount == 0 || maxValue > uint.MaxValue) return null;
			maxValue++;

			byte* endStrings = destination + available;
			byte* stringPtr = (byte*)(destination + (valueCount * 8));
			var stringsStart = stringPtr;

			if (stringPtr >= destination + available) return null;

			int ix = 0;
			for (ulong i = 0; i < maxValue; i++)
			{
				long offset = stringPtr - stringsStart;
				if (offset > ushort.MaxValue) return null;

				string enumName = Enum.GetName(type, i);
				if (enumName == null) continue;

				*(uint*)(destination + ix) = (uint)i;
				*(ushort*)(destination + ix + 4) = (ushort)offset;

				int result = ConvertUTF.WriteStringUtf8(enumName, stringPtr, (int)(endStrings - stringPtr), useQuote: false);
				if (result <= 0 || result >= ushort.MaxValue) return null;

				*(ushort*)(destination + ix + 6) = (ushort)result;

				stringPtr += result;
				ix += 8;
			}

			used = (int)(stringPtr - destination);
			return new Lookup(destination, stringsStart);
		}

		public static void DescribeEnum(Type type, out int valueCount, out ulong maxValue)
		{
			maxValue = 0;
			var allValues = Enum.GetValues(type);
			valueCount = allValues.Length;
			foreach (var val in allValues)
			{
				var num = Convert.ToUInt64(val);
				if (num > maxValue) maxValue = num;
			}
		}
	}
}
