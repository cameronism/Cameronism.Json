using System;
using System.Collections.Generic;
using System.Linq;
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
		internal const int MAX_JSON_ESCAPE_CODEPOINT = 0x80;
		internal const int MAX_JSON_ESCAPE_LENGTH = 6;
		internal const int SIZEOF_JSON_ESCAPES = MAX_JSON_ESCAPE_CODEPOINT * MAX_JSON_ESCAPE_LENGTH;
		internal const int SIZEOF_STATIC_LOOKUPS = SIZEOF_JSON_ESCAPES + SIZEOF_DIGIT_PAIRS;

		internal static void CreateStaticLookups(byte* dst, out ushort* digitPairs, out byte* jsonEscapes)
		{
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
			for (byte i = 0; i < 0x80; i++)
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
				else if (i < 0x20)
				{
					// 6 char escape sequence
					*dst++ = (byte)'\\';
					*dst++ = (byte)'u';
					*dst++ = (byte)'0';
					*dst++ = (byte)'0';
					
					*(ushort*)dst = *(digitPairs + i);
					dst += 2;
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
		internal static readonly byte* JSON_ESCAPES;
		static Convert()
		{
			IntPtr ptr = Marshal.AllocHGlobal(SIZEOF_STATIC_LOOKUPS);
			CreateStaticLookups((byte*)ptr.ToPointer(), out DIGIT_PAIRS, out JSON_ESCAPES);
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
				*(short*)(c-1)=*(short*)(digitPairs+2*pos); 
				c-=2;
			}

			while(val>0)
			{
				*c--=(byte)('0' + (val % 10));
				val /= 10;
			}
			return size;
		}

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

			var digitPairs = DIGIT_PAIRS;
			while(val>=100)
			{
				int pos = (int)(val % 100);
				val /= 100;
				*(short*)(c-1)=*(short*)(digitPairs+2*pos); 
				c-=2;
			}

			while(val>0)
			{
				*c--=(byte)('0' + (val % 10));
				val /= 10;
			}
			return size;
		}
		#endregion
	}
}
