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
			readonly uint _StringLength;
			readonly LookupTypes _Type;

			public byte* TableStart { get { return _TableStart; } }
			public byte* StringStart { get { return _StringStart; } }
			public uint StringLength { get { return _StringLength; } }
			public LookupTypes Type { get { return _Type; } }

			public Lookup(byte* tableStart, byte* stringStart, byte* stringStop, LookupTypes type)
			{
				_TableStart = tableStart;
				_StringStart = stringStart;
				_StringLength = stringStop == null ? 0 : (uint)(stringStop - stringStart);
				_Type = type;
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

		#region cache
		public static int PagesAllocated { get; private set; }
		public static Lookup[] CachedLookups
		{
			get
			{
				lock (_Cache)
				{
					return _Cache.Values.ToArray();
				}
			}
		}

		const int PAGE_SIZE = 0x10000;
		static byte* _Page = null;
		static int _Offset = 0;
		static Dictionary<Type, Lookup> _Cache = new Dictionary<Type, Lookup>();

		public static Lookup GetCachedLookup(Type type)
		{
			Lookup cached;
			lock (_Cache)
			{
				if (!_Cache.TryGetValue(type, out cached))
				{
					cached = GenerateLookup(type);
					_Cache[type] = cached;
				}
			}
			return cached;
		}

		static Lookup GenerateLookup(Type type)
		{
			if (_Page == null)
			{
				ResetPage();
				if (_Page == null)
				{
					return new Lookup(null, null, null, LookupTypes.Unsupported);
				}
			}

			KeyValuePair<ulong, string>[] values;
			LookupTypes lookupType;
			var lookup = GetLookup(type, _Page + _Offset, PAGE_SIZE - _Offset, out lookupType, out values);
			if (lookup.Type != LookupTypes.Unsupported)
			{
				if (lookup.Type != LookupTypes.Numeric)
				{
					SetOffset(lookup);
				}
				return lookup;
			}

			// try again
			Lookup? maybeLookup = null;
			if (lookupType != LookupTypes.Unsupported && _Offset != 0)
			{
				ResetPage();
				if (_Page != null)
				{
					byte* dst = _Page + _Offset;
					int available = PAGE_SIZE - _Offset;

					switch (lookupType)
					{
						case LookupTypes.Indexed:
							maybeLookup = GenerateIndexedLookup(values, dst, available);
							break;
						case LookupTypes.Sorted:
							maybeLookup = GenerateSortedLookup(values, dst, available);
							break;
						case LookupTypes.Verbose:
							maybeLookup = GenerateVerboseLookup(values, dst, available);
							break;
					}
				}
			}

			if (maybeLookup.HasValue)
			{
				lookup = maybeLookup.GetValueOrDefault();
				SetOffset(lookup);
				return lookup;
			}

			return new Lookup(null, null, null, LookupTypes.Unsupported);
		}

		static void ResetPage()
		{
			IntPtr ptr = Marshal.AllocHGlobal(PAGE_SIZE);
			if (ptr == IntPtr.Zero)
			{
				_Page = null;
				return;
			}
			PagesAllocated++;
			_Page = (byte*)ptr.ToPointer();
			_Offset = 0;
		}

		static void SetOffset(Lookup lookup)
		{
			byte* boundary = lookup.StringStart + lookup.StringLength;

			// round up to next 64 byte boundary
			ulong value = ((ulong)boundary + 63UL) & 0xffffffffffffffc0;
			boundary = (byte*)value;

			var offset = boundary - _Page;

			if (offset + 64 >= PAGE_SIZE)
			{
				// this page is nearly full
				// just allocate a new one next time
				_Page = null;
				return;
			}

			_Offset = (int)offset;
		}

		static int[] GetFlagIterationOrder(byte* tableStart)
		{
			throw new NotImplementedException();
		}
		#endregion

		static Lookup GetLookup(Type type, byte* destination, int available, out LookupTypes lookupType, out KeyValuePair<ulong, string>[] values)
		{
			values = SortValues(type);
			// this out parameter is so caller can differentiate numeric from unsupported
			lookupType = GetLookupType(values);

			Lookup? maybeLookup = null;
			switch (lookupType)
			{
				case LookupTypes.Indexed:
					maybeLookup = GenerateIndexedLookup(values, destination, available);
					break;
				case LookupTypes.Sorted:
					maybeLookup = GenerateSortedLookup(values, destination, available);
					break;
				case LookupTypes.Verbose:
					maybeLookup = GenerateVerboseLookup(values, destination, available);
					break;
				case LookupTypes.Numeric:
					return new Lookup(null, null, null, LookupTypes.Numeric);
			}

			return maybeLookup.HasValue ?
				maybeLookup.GetValueOrDefault() :
				new Lookup(null, null, null, LookupTypes.Unsupported);
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

		public static Lookup? GenerateIndexedLookup(KeyValuePair<ulong, string>[] values, byte* destination, int available)
		{
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

			return new Lookup(destination, stringsStart, stringPtr, LookupTypes.Indexed);
		}

		public static Lookup? GenerateSortedLookup(KeyValuePair<ulong, string>[] values, byte* destination, int available)
		{
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

			return new Lookup(destination, stringsStart, stringPtr, LookupTypes.Sorted);
		}

		public static Lookup? GenerateVerboseLookup(KeyValuePair<ulong, string>[] values, byte* destination, int available)
		{
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

			return new Lookup(destination, stringsStart, stringPtr, LookupTypes.Verbose);
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

		#region writers
		static int WriteString(byte* str, int len, byte* destination, int available)
		{
			int required = len + 2;
			if (available < required) return -required;

			*destination++ = (byte)'"';
			for (int i = 0; i < len; i++)
			{
				*destination++ = *str++;
			}
			*destination = (byte)'"';
			return required;
		}
		public static int WriteIndexed(long number, byte* lookupStart, byte* stringStart, byte* destination, int available)
		{
			var lookup = new Lookup(lookupStart, stringStart, null, LookupTypes.Indexed);
			byte* str;
			int len;
			lookup.FindIndexed((ulong)number, out str, out len);
			if (len > 0)
			{
				return WriteString(str, len, destination, available);
			}
			return Serializer.WriteInt64(number, destination, available);
		}
		public static int WriteIndexed(ulong number, byte* lookupStart, byte* stringStart, byte* destination, int available)
		{
			var lookup = new Lookup(lookupStart, stringStart, null, LookupTypes.Indexed);
			byte* str;
			int len;
			lookup.FindIndexed(number, out str, out len);
			if (len > 0)
			{
				return WriteString(str, len, destination, available);
			}
			return Serializer.WriteUInt64(number, destination, available);
		}
		public static int WriteSorted(long number, byte* lookupStart, byte* stringStart, byte* destination, int available)
		{
			var lookup = new Lookup(lookupStart, stringStart, null, LookupTypes.Sorted);
			byte* str;
			int len;
			lookup.FindSequential((ulong)number, out str, out len);
			if (len > 0)
			{
				return WriteString(str, len, destination, available);
			}
			return Serializer.WriteInt64(number, destination, available);
		}
		public static int WriteSorted(ulong number, byte* lookupStart, byte* stringStart, byte* destination, int available)
		{
			var lookup = new Lookup(lookupStart, stringStart, null, LookupTypes.Sorted);
			byte* str;
			int len;
			lookup.FindSequential(number, out str, out len);
			if (len > 0)
			{
				return WriteString(str, len, destination, available);
			}
			return Serializer.WriteUInt64(number, destination, available);
		}
		public static int WriteVerbose(long number, byte* lookupStart, byte* stringStart, byte* destination, int available)
		{
			var lookup = new Lookup(lookupStart, stringStart, null, LookupTypes.Verbose);
			byte* str;
			int len;
			lookup.FindVerbose((ulong)number, out str, out len);
			if (len > 0)
			{
				return WriteString(str, len, destination, available);
			}
			return Serializer.WriteInt64(number, destination, available);
		}
		public static int WriteVerbose(ulong number, byte* lookupStart, byte* stringStart, byte* destination, int available)
		{
			var lookup = new Lookup(lookupStart, stringStart, null, LookupTypes.Verbose);
			byte* str;
			int len;
			lookup.FindVerbose(number, out str, out len);
			if (len > 0)
			{
				return WriteString(str, len, destination, available);
			}
			return Serializer.WriteUInt64(number, destination, available);
		}
		#region flags
		static int WriteInnerString(byte* str, int len, byte* destination, int available, bool firstValue)
		{
			int required = len;
			if (!firstValue) required += 2;

			if (available < required) return -required;

			if (!firstValue)
			{
				*destination++ = (byte)',';
				*destination++ = (byte)' ';
			}

			for (int i = 0; i < len; i++)
			{
				*destination++ = *str++;
			}
			return required;
		}
		static int GetFlagCount(long number, int @sizeof)
		{
			// need to try a lookup version of this to avoid branches
			var unum = (ulong)number;
			if (number < 0)
			{
				if (@sizeof == 4)
				{
					unum = unum & 0xffffffff;
				}
				else
				{
					if (@sizeof == 1)
					{
						unum = unum & 0xff;
					}
					else if (@sizeof == 2)
					{
						unum = unum & 0xffff;
					}
				}
			}
			return GetFlagCount(unum);
		}
		static int GetFlagCount(ulong x)
		{
			// http://stackoverflow.com/a/109025
			x = x - ((x >> 1) & 0x5555555555555555);
			x = (x & 0x3333333333333333) + ((x >> 2) & 0x3333333333333333);
			x = (x + (x >> 4)) & 0x0F0F0F0F0F0F0F0F;
			x = x + (x >> 8);
			x = x + (x >> 16);
			x = x + (x >> 32); 
			return (int)(x & 0xFF);
		}
		static bool WriteIndexedFlagImp(ulong unum, byte* lookupStart, byte* stringStart, byte* destination, int available, out int result)
		{
			result = 0;
			var order = GetFlagIterationOrder(lookupStart);
			if (order == null || available < 3) return false;

			int number = (int)unum; // we know it fits in a ushort since Indexed
			var lookup = new Lookup(lookupStart, stringStart, null, LookupTypes.Indexed);
			var ptr = destination;

			available -= 2;
			*ptr++ = (byte)'"';

			bool firstValue = true;
			foreach (var ix in order)
			{
				if ((ix & number) == ix)
				{
					number -= ix;
					byte* str = stringStart + *(ushort*)(lookupStart + ix);
					int length = *(ushort*)(lookupStart + ix + 2);
					int attempt = WriteInnerString(str, length, destination, available, firstValue);
					if (attempt > 0)
					{
						firstValue = false;
						ptr += attempt;
						available -= attempt;
					}
					else
					{
						result = attempt - (int)(ptr - destination);
						return false;
					}
				}
			}

			*ptr++ = (byte)'"';
			result = (int)(ptr - destination);
			return true;
		}
		static bool WriteSortedFlagImp(ulong number, byte* lookupStart, byte* stringStart, byte* destination, int available, out int result)
		{
			throw new NotImplementedException();
		}
		static bool WriteVerboseFlagImp(ulong number, byte* lookupStart, byte* stringStart, byte* destination, int available, out int result)
		{
			throw new NotImplementedException();
		}
		public static int WriteIndexedFlag(long number, byte* lookupStart, byte* stringStart, byte* destination, int available, int @sizeof)
		{
			if (GetFlagCount(number, @sizeof) <= 1) return WriteIndexed(number, lookupStart, stringStart, destination, available);
			int result;
			if (WriteIndexedFlagImp((ulong)number, lookupStart, stringStart, destination, available, out result))
			{
				return result;
			}
			return Serializer.WriteInt64(number, destination, available);
		}
		public static int WriteIndexedFlag(ulong number, byte* lookupStart, byte* stringStart, byte* destination, int available)
		{
			if (GetFlagCount(number) <= 1) return WriteIndexed(number, lookupStart, stringStart, destination, available);
			int result;
			if (WriteIndexedFlagImp(number, lookupStart, stringStart, destination, available, out result))
			{
				return result;
			}
			return Serializer.WriteUInt64(number, destination, available);
		}
		public static int WriteSortedFlag(long number, byte* lookupStart, byte* stringStart, byte* destination, int available, int @sizeof)
		{
			if (GetFlagCount(number, @sizeof) <= 1) return WriteSorted(number, lookupStart, stringStart, destination, available);
			int result;
			if (WriteSortedFlagImp((ulong)number, lookupStart, stringStart, destination, available, out result))
			{
				return result;
			}
			return Serializer.WriteInt64(number, destination, available);
		}
		public static int WriteSortedFlag(ulong number, byte* lookupStart, byte* stringStart, byte* destination, int available)
		{
			if (GetFlagCount(number) <= 1) return WriteSorted(number, lookupStart, stringStart, destination, available);
			int result;
			if (WriteSortedFlagImp(number, lookupStart, stringStart, destination, available, out result))
			{
				return result;
			}
			return Serializer.WriteUInt64(number, destination, available);
		}
		public static int WriteVerboseFlag(long number, byte* lookupStart, byte* stringStart, byte* destination, int available, int @sizeof)
		{
			if (GetFlagCount(number, @sizeof) <= 1) return WriteVerbose(number, lookupStart, stringStart, destination, available);
			int result;
			if (WriteVerboseFlagImp((ulong)number, lookupStart, stringStart, destination, available, out result))
			{
				return result;
			}
			return Serializer.WriteInt64(number, destination, available);
		}
		public static int WriteVerboseFlag(ulong number, byte* lookupStart, byte* stringStart, byte* destination, int available)
		{
			if (GetFlagCount(number) <= 1) return WriteVerbose(number, lookupStart, stringStart, destination, available);
			int result;
			if (WriteVerboseFlagImp(number, lookupStart, stringStart, destination, available, out result))
			{
				return result;
			}
			return Serializer.WriteUInt64(number, destination, available);
		}
		#endregion
		#endregion
	}
}
