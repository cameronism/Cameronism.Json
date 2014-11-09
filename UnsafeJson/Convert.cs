using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UnsafeJson
{
	public unsafe static class Convert
	{
		public delegate int LowWriter<T>(ref T value, byte* dst, int avail);

		#region static lookups
		internal const int SIZEOF_DIGIT_PAIRS = 100 * 2;
		internal const int SIZEOF_HEX_PAIRS = 256 * 2;
		internal const int MAX_JSON_ESCAPE_CODEPOINT = 0x90;
		internal const int MAX_JSON_ESCAPE_LENGTH = 6;
		internal const int SIZEOF_JSON_ESCAPES = MAX_JSON_ESCAPE_CODEPOINT * MAX_JSON_ESCAPE_LENGTH;
		internal const int SIZEOF_STATIC_LOOKUPS = SIZEOF_JSON_ESCAPES + SIZEOF_DIGIT_PAIRS + SIZEOF_HEX_PAIRS;

		static byte LowerHex(byte number)
		{
			return (byte)(number < 10 ?
				'0' + number :
				'a' + number - 10);
		}

		internal static void CreateStaticLookups(byte* dst, out ushort* digitPairs, out byte* jsonEscapes, out ushort* hexPairsLower)
		{
			hexPairsLower = (ushort*)dst;

			for (byte i = 0; i < 16 ; i++)
			{
				for (byte j = 0; j < 16; j++)
				{
					*dst++ = LowerHex(i);
					*dst++ = LowerHex(j);
				}
			}

			digitPairs = (ushort*)dst;
			
			for (byte i = (byte)'0'; i <= (byte)'9'; i++)
			{
				for (byte j = (byte)'0'; j <= (byte)'9'; j++)
				{
					*dst++ = i;
					*dst++ = j;
				}
			}
			
			jsonEscapes = dst;
			
			// write 6 bytes for every character [0,128)
			for (byte i = 0; i < MAX_JSON_ESCAPE_CODEPOINT; i++)
			{
				char? single = null;
				switch (i)
				{
					case 0x08: single = 'b'; break;
					case 0x09: single = 't'; break;
					case 0x0C: single = 'f'; break;
					case 0x0A: single = 'n'; break;
					case 0x0D: single = 'r'; break;
					case 0x22: single = '"'; break;
					case 0x5C: single = '\\'; break;
				}
				
				if (single.HasValue)
				{
					// 2 char escape sequence
					*dst++ = (byte)'\\';
					*dst++ = (byte)single.Value;
					*dst++ = 0;
					*dst++ = 0;
					*dst++ = 0;
					*dst++ = 0;
				}
				else if (i < 0x20 || i == 0x85)
				{
					var s = i.ToString("x2");

					// 6 char escape sequence
					*dst++ = (byte)'\\';
					*dst++ = (byte)'u';
					*dst++ = (byte)'0';
					*dst++ = (byte)'0';
					*dst++ = (byte)s[0];
					*dst++ = (byte)s[1];
				}
				else if (i >= 0x80)
				{
					// 2 bytes in utf8
					*dst++ = 0xC2;
					*dst++ = i;
					*dst++ = 0;
					*dst++ = 0;
					*dst++ = 0;
					*dst++ = 0;
				}
				else
				{
					// single char no escape
					*dst++ = i;
					*dst++ = 0;
					*dst++ = 0;
					*dst++ = 0;
					*dst++ = 0;
					*dst++ = 0;
				}
			}
		}
		#endregion

		internal static readonly ushort* DIGIT_PAIRS;
		internal static readonly ushort* HEX_PAIRS_LOWER;
		internal static readonly byte* JSON_ESCAPES;
		static Convert()
		{
			// Hold onto < 1K until the process exits
			IntPtr ptr = Marshal.AllocHGlobal(SIZEOF_STATIC_LOOKUPS);
			CreateStaticLookups((byte*)ptr.ToPointer(), out DIGIT_PAIRS, out JSON_ESCAPES, out HEX_PAIRS_LOWER);
		}

		#region writers
		internal static int WriteNull(byte* dst, int avail)
		{
			if (avail < 4) return -4;
			
			*(dst + 0) = (byte)'n';
			*(dst + 1) = (byte)'u';
			*(dst + 2) = (byte)'l';
			*(dst + 3) = (byte)'l';
			
			return 4;
		}

		internal const int SIZEOF_GUID_D = 38; // 32 digits, 4 hyphens, 2 quotes

		internal static int WriteGuidFormatD(Guid g, byte* dst, int avail)
		{
			if (avail < SIZEOF_GUID_D) return -SIZEOF_GUID_D;
			//00000000-0000-0000-0000-000000000000


			byte* hex;
			byte* ptr = (byte*)&g;

			*dst++ = (byte)'"';
			// 3 - 0
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 3));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 2));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 1));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 0));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			*dst++ = (byte)'-';
			// 5 - 4
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 5));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 4));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			*dst++ = (byte)'-';
			// 7 - 6
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 7));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 6));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			*dst++ = (byte)'-';
			// 8 - 9
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 8));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 9));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			*dst++ = (byte)'-';
			// 10 - 15
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 10));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 11));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 12));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 13));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 14));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			hex = (byte*)(HEX_PAIRS_LOWER + *(ptr + 15));
			*dst++ = *(hex + 0);
			*dst++ = *(hex + 1);
			*dst   = (byte)'"';

			return SIZEOF_GUID_D;
		}

		internal static int WriteBoolean(bool b, byte* dst, int avail)
		{
			if (avail < 5) return -5;

			if (b)
			{
				*dst++ = (byte)'t';
				*dst++ = (byte)'r';
				*dst++ = (byte)'u';
				*dst   = (byte)'e';
				return 4;
			}
			else
			{
				*dst++ = (byte)'f';
				*dst++ = (byte)'a';
				*dst++ = (byte)'l';
				*dst++ = (byte)'s';
				*dst   = (byte)'e';
				return 5;
			}
		}

		// from http://stackoverflow.com/a/4351484
		internal static int WriteInt32(int n, byte* c, int avail)
		{
			if(n==0)
			{
				*c = (byte)'0';
				return 1;
			}

			int sign = n<0 ? -1 : 0;
			uint val = (uint)((n ^ sign) - sign);

			int size;
			if(val>=10000)
			{
				if(val>=10000000)
				{
					if(val>=1000000000)
						size=10;
					else if(val>=100000000)
						size=9;
					else 
						size=8;
				}
				else
				{
					if(val>=1000000)
						size=7;
					else if(val>=100000)
						size=6;
					else
						size=5;
				}
			}
			else 
			{
				if(val>=100)
				{
					if(val>=1000)
						size=4;
					else
						size=3;
				}
				else
				{
					if(val>=10)
						size=2;
					else
						size=1;
				}
			}
			size -= sign;
			if (size > avail) return -size;

			if(sign != 0)
				*c=(byte)'-';

			c += size-1;

			var digitPairs = DIGIT_PAIRS;
			while(val>=100)
			{
				int pos = (int)(val % 100);
				val /= 100;
				*(ushort*)(c-1) = *(digitPairs + pos);
				c-=2;
			}

			while(val>0)
			{
				*c--=(byte)('0' + (val % 10));
				val /= 10;
			}
			return size;
		}

		// from http://stackoverflow.com/a/4351484
		internal static int WriteUInt32(uint val, byte* c, int avail)
		{
			if(val==0)
			{
				*c = (byte)'0';
				return 1;
			}

			int size;
			if(val>=10000)
			{
				if(val>=10000000)
				{
					if(val>=1000000000)
						size=10;
					else if(val>=100000000)
						size=9;
					else 
						size=8;
				}
				else
				{
					if(val>=1000000)
						size=7;
					else if(val>=100000)
						size=6;
					else
						size=5;
				}
			}
			else 
			{
				if(val>=100)
				{
					if(val>=1000)
						size=4;
					else
						size=3;
				}
				else
				{
					if(val>=10)
						size=2;
					else
						size=1;
				}
			}

			if (size > avail) return -size;
			c += size - 1;

			var digitPairs = DIGIT_PAIRS;
			while(val>=100)
			{
				int pos = (int)(val % 100);
				val /= 100;
				*(ushort*)(c - 1) = *(digitPairs + pos);
				c-=2;
			}

			while(val>0)
			{
				*c--=(byte)('0' + (val % 10));
				val /= 10;
			}
			return size;
		}
		internal static int WriteUInt64(ulong val, byte* c, int avail)
		{
			if (val <= 0xFFFFFFFFul) return WriteUInt32((uint)val, c, avail);

			// max length is 20
			// min length is 10 (called WriteUInt32 for small enough values)

			int size;
			if (val >= 100000000000000ul)
			{
				if (val >= 100000000000000000ul)
				{
					if (val >= 10000000000000000000ul)
						size = 20;
					else if (val >= 1000000000000000000ul)
						size = 19;
					else 
						size = 18;
				}
				else 
				{
					if (val >= 10000000000000000ul)
						size = 17;
					else if (val >= 1000000000000000ul)
						size = 16;
					else 
						size = 15;
				}
			}
			else 
			{
				if (val >= 100000000000ul)
				{
					if (val >= 10000000000000ul)
						size = 14;
					else if (val >= 1000000000000ul)
						size = 13;
					else 
						size = 12;
				}
				else 
				{
					if (val >= 10000000000ul)
						size = 11;
					else 
						size = 10;
				}
			}

			if (size > avail) return -size;
			c += size - 1;

			var digitPairs = DIGIT_PAIRS;
			while(val>=100)
			{
				int pos = (int)(val % 100);
				val /= 100;
				*(ushort*)(c - 1) = *(digitPairs + pos);
				c-=2;
			}

			while(val>0)
			{
				*c--=(byte)('0' + (val % 10));
				val /= 10;
			}
			return size;
		}
		internal static int WriteInt64(long n, byte* c, int avail)
		{
			if (n <= 2147483647 && n >= -2147483648) return WriteInt32((int)n, c, avail);

			// long.MaxValue length is 19
			// min length is 10 (small enough values go to WriteInt32

			int sign = n < 0 ? -1 : 0;
			ulong val = (ulong)((n ^ sign) - sign);

			int size;
			if(val>=100000000000000L)
			{
				if(val>=10000000000000000L)
				{
					if(val>=1000000000000000000L)
						size=19;
					else if(val>=100000000000000000L)
						size=18;
					else 
						size=17;
				}
				else
				{
					if(val>=1000000000000000L)
						size=16;
					else
						size=15;
				}
			}
			else 
			{
				if(val>=1000000000000L)
				{
					if(val>=10000000000000L)
						size=14;
					else
						size=13;
				}
				else
				{
					if(val>=100000000000L)
						size=12;
					else if(val>=10000000000L)
						size=11;
					else
						size=10;
				}
			}

			size -= sign;
			if (size > avail) return -size;

			if(sign != 0)
				*c=(byte)'-';

			c += size-1;

			var digitPairs = DIGIT_PAIRS;
			while(val>=100)
			{
				int pos = (int)(val % 100);
				val /= 100;
				*(ushort*)(c-1) = *(digitPairs + pos);
				c-=2;
			}

			while(val>0)
			{
				*c--=(byte)('0' + (val % 10));
				val /= 10;
			}
			return size;
		}

		const int TICKS_PER_SECOND = 10000000;
		//2014-09-28T14:58:35.8439067Z // 28
		internal const int SIZEOF_DATETIME_8601 = 28 + 2; // 28, 2 quotes
		internal static int WriteDateTime8601(DateTime val, byte* c, int avail)
		{
			if (avail < SIZEOF_DATETIME_8601) return -SIZEOF_DATETIME_8601;

			int part;
			byte* start = c;
			var digitPairs = DIGIT_PAIRS;
			*c++ = (byte)'"';

			part = val.Year;
			*(ushort*)c = *(digitPairs + part / 100);
			c += 2;
			*(ushort*)c = *(digitPairs + part % 100);
			c += 2;
			*c++ = (byte)'-';

			part = val.Month;
			*(ushort*)c = *(digitPairs + part);
			c += 2;
			*c++ = (byte)'-';

			part = val.Day;
			*(ushort*)c = *(digitPairs + part);
			c += 2;
			*c++ = (byte)'T';

			part = val.Hour;
			*(ushort*)c = *(digitPairs + part);
			c += 2;
			*c++ = (byte)':';

			part = val.Minute;
			*(ushort*)c = *(digitPairs + part);
			c += 2;
			*c++ = (byte)':';

			part = val.Second;
			*(ushort*)c = *(digitPairs + part);
			c += 2;

			part = (int)(val.Ticks % TICKS_PER_SECOND);

			if (part != 0)
			{
				*c = (byte)'.';
				*(ushort*)(c + 6) = *(digitPairs + part % 100);
				part /= 100;
				*(ushort*)(c + 4) = *(digitPairs + part % 100);
				part /= 100;
				*(ushort*)(c + 2) = *(digitPairs + part % 100);
				part /= 100;
				*(c + 1) = (byte)('0' + part);

				// omit trailing zeroes
				int used = 7;
				while (*(c + used) == (byte)'0') used--;
				c += used + 1;
			}

			if (val.Kind == DateTimeKind.Utc)
			{
				*c++ = (byte)'Z';
			}

			*c++ = (byte)'"';

			return (int)(c - start);
		}

		#region floating point
		static bool IsFinite(double d)
		{
			ulong u = *(ulong*)&d;
			return (u & 0x7ff0000000000000ul) != 0x7ff0000000000000ul;
		}

		static bool IsFinite(float f)
		{
			uint u = *(uint*)&f;
			return (u & 0x7f800000u) != 0x7f800000u;
		}

		internal const string NAN = "\"NaN\"";
		internal const string POSITIVE_INFINITY = "\"Infinity\"";
		internal const string NEGATIVE_INFINITY = "\"-Infinity\"";

		static int WriteASCII(string s, byte* c, int avail)
		{
			if (s.Length > avail) return -s.Length;

			int ix;
			for (ix = 0; ix < s.Length; ix++)
			{
				*(c + ix) = (byte)s[ix];
			}
			return ix;
		}

		static int WriteGeneralFloat(string floatNumber, byte* c, int avail)
		{
			int requiredLength = floatNumber.Length + 2; // +2 in case we need to append ".0"
			if (requiredLength > avail) return -requiredLength;

			bool needsDot = true;
			int ix;
			for (ix = 0; ix < floatNumber.Length; ix++)
			{
				int ch = floatNumber[ix];
				*(c + ix) = (byte)ch;
				if (ch == (int)'.')
				{
					needsDot = false;
				}
			}

			if (needsDot)
			{
				*(c + ix++) = (byte)'.';
				*(c + ix++) = (byte)'0';
			}

			return ix;
		}

		internal static int WriteDouble(double val, byte* c, int avail)
		{
			string s;
			if (IsFinite(val))
			{
				// need an allocation free alternative
				s = val.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
				return WriteGeneralFloat(s, c, avail);
			}
			else
			{
				s = 
					double.IsNaN(val) ? NAN : 
					double.IsPositiveInfinity(val) ? POSITIVE_INFINITY :
					NEGATIVE_INFINITY;
				return WriteASCII(s, c, avail);
			}
		}

		internal static int WriteSingle(float val, byte* c, int avail)
		{
			string s;
			if (IsFinite(val))
			{
				// need an allocation free alternative
				s = val.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
				return WriteGeneralFloat(s, c, avail);
			}
			else
			{
				s = 
					float.IsNaN(val) ? NAN : 
					float.IsPositiveInfinity(val) ? POSITIVE_INFINITY :
					NEGATIVE_INFINITY;
				return WriteASCII(s, c, avail);
			}
		}

		internal static int WriteDecimal(decimal val, byte* c, int avail)
		{
			// need an allocation free alternative
			var s = val.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
			return WriteGeneralFloat(s, c, avail);
		}
		#endregion
		#region IPAddress
		[DllImport("ntdll.dll"), System.Security.SuppressUnmanagedCodeSecurity]
		static extern byte* RtlIpv4AddressToStringA(byte* addr, byte* dst);

		[DllImport("ntdll.dll"), System.Security.SuppressUnmanagedCodeSecurity]
		static extern byte* RtlIpv6AddressToStringA(byte* addr, byte* dst);

		internal static int WriteIPv4(long val, byte* dst, int avail)
		{
			// need at least 18 (16 for ip and 2 for quotes)
			if (avail < 18) return -18;

			*dst++ = (byte)'"';

			byte* ip = ((byte*)&val); // little endian

			byte* nullChar = RtlIpv4AddressToStringA(ip, dst);
			long len = nullChar - dst;

			if (len > 0 && len <= 16)
			{
				*nullChar = (byte)'"';
				return 2 + (int)len;
			}

			return -18;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void Swap2(byte* src, byte* dst, int offset)
		{
			*(dst + offset + 0) = *(src + offset + 1);
			*(dst + offset + 1) = *(src + offset + 0);
		}

		internal static int WriteIPv6(ushort[] val, byte* dst, int avail)
		{
			// need at least 48 (46 for ip and 2 for quotes)
			if (avail < 48) return -48;

			*dst++ = (byte)'"';

			// Guid is also 16 bytes
			Guid fakeIP;
			byte* ip;
			fixed (ushort* uptr = val)
			{
				byte* src = (byte*)uptr;
				ip = (byte*)&fakeIP;

				// swap the bytes -- no idea why .NET keeps these in host order instead of network order
				Swap2(src, ip, 0);
				Swap2(src, ip, 2);
				Swap2(src, ip, 4);
				Swap2(src, ip, 6);
				Swap2(src, ip, 8);
				Swap2(src, ip, 10);
				Swap2(src, ip, 12);
				Swap2(src, ip, 14);
			}

			byte* nullChar = RtlIpv6AddressToStringA((byte*)ip, dst);
			long len = nullChar - dst;
			if (len > 0 && len <= 46)
			{
				*nullChar = (byte)'"';
				return 2 + (int)len;
			}

			return -48;
		}
		#endregion
		#endregion
	}
}
