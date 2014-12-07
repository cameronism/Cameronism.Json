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

			var values = SortValues(type);
			if (values.Length == 0) return null;
			var maxValue = values[values.Length - 1].Key;
			if (maxValue > uint.MaxValue) return null;

			byte* endStrings = destination + available;
			byte* stringPtr = (byte*)(destination + (values.Length * 8));
			var stringsStart = stringPtr;

			if (stringPtr >= destination + available) return null;

			int ix = 0;
			foreach (var value in values)
			{
				long offset = stringPtr - stringsStart;
				if (offset > ushort.MaxValue) return null;

				*(uint*)(destination + ix) = (uint)value.Key;
				*(ushort*)(destination + ix + 4) = (ushort)offset;

				int result = ConvertUTF.WriteStringUtf8(value.Value, stringPtr, (int)(endStrings - stringPtr), useQuote: false);
				if (result <= 0 || result >= ushort.MaxValue) return null;

				*(ushort*)(destination + ix + 6) = (ushort)result;

				stringPtr += result;
				ix += 8;
			}

			used = (int)(stringPtr - destination);
			return new Lookup(destination, stringsStart);
		}

		static KeyValuePair<ulong, string>[] SortValues(Type type)
		{
			var values = Enum.GetValues(type);
			bool signed;
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
					signed = true;
					break;
				default:
					signed = false;
					break;
			}

			var sorted = new KeyValuePair<ulong, string>[values.Length];

			for (int i = 0; i < sorted.Length; i++)
			{
				var value = values.GetValue(i);
				var num = signed ? (ulong)Convert.ToInt64(value) : Convert.ToUInt64(value);
				sorted[i] = new KeyValuePair<ulong, string>(num, value.ToString());
			}

			// insertion sort -- probably already sorted
			for (int i = 1; i < sorted.Length; i++)
			{
				int j = i;
				var temp = sorted[i];
				var exchanged = false;
 
				while (sorted[j - 1].Key > temp.Key)
				{
					sorted[j] = sorted[j - 1];
					j--;
					exchanged = true;
					if (j == 0) break;
				}
 
				if (exchanged) sorted[j] = temp;
			}

			return sorted;
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
