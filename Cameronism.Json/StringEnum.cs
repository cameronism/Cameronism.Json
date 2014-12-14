using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cameronism.Json
{
	unsafe internal class StringEnum
	{
		public enum LookupTypes
		{
			Unsupported,
			/// <summary>
			/// Not a lookup, just use the numeric value
			/// </summary>
			Numeric,
			Indexed,
			Sorted,
			Verbose,
		}

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

			public void FindSequential(ulong longValue, out byte* str, out int length)
			{
				uint value = (uint)longValue;
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

			public void FindVerbose(ulong value, out byte* str, out int length)
			{
				byte* lookup = _TableStart;
				byte* stop = _StringStart;

				while (lookup < stop)
				{
					var verbose = (VerboseEnum*)lookup;
					if (verbose->Value == value)
					{
						verbose->GetString(stop, out length, out str);
						return;
					}

					lookup += 16;
				}

				str = null;
				length = 0;
				return;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		struct VerboseEnum
		{
			[FieldOffset(0)]
			ulong _Value;

			[FieldOffset(8)]
			fixed byte _String[8];

			[FieldOffset(8)]
			byte _Kind;

			[FieldOffset(9)]
			byte _ZeroMe; // to zero every byte of the struct

			[FieldOffset(10)]
			ushort _Length;
			
			[FieldOffset(12)]
			uint _Offset;

			public ulong Value { get { return _Value; } }

			public void GetString(byte* regionStart, out int length, out byte* start)
			{
				if ((_Kind & 0x80) == 0)
				{
					fixed (byte* ptr = _String)
					{
						for (int i = 0; i < 8; i++)
						{
							if (*(ptr + i) != 0)
							{
								// this is generally a very bad idea 
								// this struct must not be on the .NET heap
								start = ptr + i;
								length = 8 - i;
								return;
							}
						}
					}

					// this shouldn't happen
					start = null;
					length = 0;
				}
				else
				{
					start = regionStart + _Offset;
					length = _Length;
				}
			}

			/// <summary>
			/// Returns true if string was inlined
			/// </summary>
			public bool Init(byte* regionStart, ulong value, byte* stringStart, int stringLength)
			{
				_Value = value;

				// try to inline
				if (stringLength < 8 || (stringLength == 8 && (*stringStart & 0x80) == 0))
				{
					fixed (byte* ptr = _String)
					{
						// zero out leading (if any)
						int i;
						for (i = 0; i < 8 - stringLength; i++)
						{
							*(ptr + i) = 0;
						}

						// finally copy it in
						for (int j = 0; j < stringLength; j++)
						{
							*(ptr + i++) = *(stringStart + j);
						}
					}

					return true;
				}

				// not inlined
				_Kind = 0x80;
				_ZeroMe = 0;
				_Length = (ushort)stringLength;
				_Offset = (uint)(stringStart - regionStart);
				return false;
			}
		}

		public static LookupTypes GetLookupType(KeyValuePair<ulong, string>[] values)
		{
			if (values.Length == 0) return LookupTypes.Numeric;

			ulong max = values.Last().Key;

			if (max > uint.MaxValue) return LookupTypes.Verbose;

			var sortedSize = (ulong)(values.Length * 8);
			var indexedSize = (max + 1) * 4;

			return indexedSize <= sortedSize ?
				LookupTypes.Indexed :
				LookupTypes.Sorted;
		}

		public static Lookup? GenerateIndexedLookup(KeyValuePair<ulong, string>[] values, byte* destination, int available, out int used)
		{
			used = 0;

			int valueCount = values.Length;
			if (valueCount == 0) return null;

			var maxValue = values.Last().Key + 1;

			byte* endStrings = destination + available;
			byte* stringPtr = (byte*)(destination + (maxValue * 4));
			var stringsStart = stringPtr;

			if (stringPtr >= destination + available) return null;

			int valueIndex = 0;
			for (ulong i = 0; i < maxValue; i++)
			{
				long offset = stringPtr - stringsStart;
				if (offset > ushort.MaxValue) return null;

				var enumName = GetString(values, (int)i, ref valueIndex);
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

		public static Lookup? GenerateSortedLookup(KeyValuePair<ulong, string>[] values, byte* destination, int available, out int used)
		{
			used = 0;

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

		public static Lookup? GenerateVerboseLookup(KeyValuePair<ulong, string>[] values, byte* destination, int available, out int used)
		{
			used = 0;

			if (values.Length == 0) return null;

			byte* endStrings = destination + available;
			byte* stringPtr = (byte*)(destination + (values.Length * 16));
			var stringsStart = stringPtr;

			if (stringPtr >= destination + available) return null;

			int ix = 0;
			foreach (var value in values)
			{
				long offset = stringPtr - stringsStart;
				if (offset > ushort.MaxValue) return null;

				int result = ConvertUTF.WriteStringUtf8(value.Value, stringPtr, (int)(endStrings - stringPtr), useQuote: false);
				if (result <= 0 || result >= ushort.MaxValue) return null;

				var verbose = (VerboseEnum*)(destination + ix);
				bool inlined = verbose->Init(stringsStart, value.Key, stringPtr, result);

				if (!inlined) stringPtr += result;
				ix += 16;
			}

			used = (int)(stringPtr - destination);
			return new Lookup(destination, stringsStart);
		}

		public static KeyValuePair<ulong, string>[] SortValues(Type type)
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

		static string GetString(KeyValuePair<ulong, string>[] values, int value, ref int ix)
		{
			for (int i = ix; i <= value && i < values.Length; i++)
			{
				if ((int)values[i].Key == value)
				{
					ix = i;
					return values[i].Value;
				}
			}

			return null;
		}
	}
}
