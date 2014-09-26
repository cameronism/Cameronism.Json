// Based on http://llvm.org/svn/llvm-project/llvm/trunk/lib/Support/ConvertUTF.c downloaded 2014-09-24

/*===--- ConvertUTF.c - Universal Character Names conversions ---------------===
 *
 *                     The LLVM Compiler Infrastructure
 *
 * This file is distributed under the University of Illinois Open Source
 * License. See LICENSE.TXT for details.
 *
 *===------------------------------------------------------------------------=*/
/*
 * Copyright 2001-2004 Unicode, Inc.
 * 
 * Disclaimer
 * 
 * This source code is provided as is by Unicode, Inc. No claims are
 * made as to fitness for any particular purpose. No warranties of any
 * kind are expressed or implied. The recipient agrees to determine
 * applicability of information provided. If this file has been
 * purchased on magnetic or optical media from Unicode, Inc., the
 * sole remedy for any claim will be exchange of defective media
 * within 90 days of receipt.
 * 
 * Limitations on Rights to Redistribute This Code
 * 
 * Unicode, Inc. hereby grants the right to freely use the information
 * supplied in this file in the creation of products supporting the
 * Unicode Standard, and to make copies of this file in any form
 * for internal or external distribution as long as this notice
 * remains attached.
 */

/* ---------------------------------------------------------------------

    Conversions between UInt32, UTF-16, and UTF-8. Source code file.
    Author: Mark E. Davis, 1994.
    Rev History: Rick McGowan, fixes & updates May 2001.
    Sept 2001: fixed const & error conditions per
        mods suggested by S. Parent & A. Lillich.
    June 2002: Tim Dodd added detection and handling of incomplete
        source sequences, enhanced error detection, added casts
        to eliminate compiler warnings.
    July 2003: slight mods to back out aggressive FFFE detection.
    Jan 2004: updated switches in from-Byte conversions.
    Oct 2004: updated to use UNI_MAX_LEGAL_UTF32 in UTF-32 conversions.

    See the header file "ConvertUTF.h" for complete documentation.

------------------------------------------------------------------------ */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UnsafeJson
{
	public unsafe static class ConvertUTF
	{
		const Int32 UNI_REPLACEMENT_CHAR = 0x0000FFFD;

		public enum ConversionFlags {
			strictConversion = 0,
			lenientConversion
		}

		const ConversionFlags strictConversion = ConversionFlags.strictConversion;
		const ConversionFlags lenientConversion = ConversionFlags.lenientConversion;

		public enum ConversionResult {
			/// <summary>conversion successful</summary>
			conversionOK,
			/// <summary>partial character in source, but hit end</summary>
			sourceExhausted,
			/// <summary>insuff. room in target for conversion</summary>
			targetExhausted,
			/// <summary>source sequence is illegal/malformed</summary>
			sourceIllegal
		}

		const ConversionResult conversionOK = ConversionResult.conversionOK;
		const ConversionResult sourceExhausted = ConversionResult.sourceExhausted;
		const ConversionResult targetExhausted = ConversionResult.targetExhausted;
		const ConversionResult sourceIllegal = ConversionResult.sourceIllegal;


		const Int32 UNI_SUR_HIGH_START = (Int32)0xD800;
		const Int32 UNI_SUR_HIGH_END   = (Int32)0xDBFF;
		const Int32 UNI_SUR_LOW_START  = (Int32)0xDC00;
		const Int32 UNI_SUR_LOW_END    = (Int32)0xDFFF;

		const int halfShift  = 10; /* used for shifting by 10 bits */
		const Int32 halfBase = 0x0010000;

		static Byte[] firstByteMark = { 0x00, 0x00, 0xC0, 0xE0, 0xF0, 0xF8, 0xFC };
		const int firstByteMark_1 = 0;
		const int firstByteMark_2 = 0xC0;
		const int firstByteMark_3 = 0xE0;
		const int firstByteMark_4 = 0xF0;

		
		const int JSON_ESCAPE_MAX_LENGTH = 6;
		/// <summary>Writes 768 bytes to dst with JSON escapes.  6 bytes for each char 0-127</summary>
		public unsafe static void WriteJsonEscapes(byte* dst)
		{
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
					var s = i.ToString("X2");
					*dst++ = (byte)'\\';
					*dst++ = (byte)'u';
					*dst++ = (byte)'0';
					*dst++ = (byte)'0';
					*dst++ = (byte)s[0];
					*dst++ = (byte)s[1];
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
		
		static readonly byte* JsonEscapes;
		static ConvertUTF()
		{
			IntPtr ptr = Marshal.AllocHGlobal(0x80 * JSON_ESCAPE_MAX_LENGTH);
			byte* start = (byte*)ptr.ToPointer();
			WriteJsonEscapes(start);
			JsonEscapes = start;
		}
		
		/// <summary>Returns the number of bytes written</summary>
		public static int EscapeJson(
				string s, 
				byte[] b)
		{
			if (String.IsNullOrEmpty(s)) return 0;
			if (b == null) return 0;
			
			fixed (char* cptr = s)
			{
				fixed (byte* bptr = b)
				{
					ushort* sourceStart = (ushort*)cptr;
					ushort* sourceEnd = (ushort*)(cptr + s.Length);
					byte* targetStart = bptr;
					byte* targetEnd = bptr + b.Length;
					var result = ConvertUTF16toJson(ref sourceStart, sourceEnd, ref targetStart, targetEnd);
					return (int)(targetStart - bptr);
				}
			}
		}
		
		/// <summary>JSON = UTF-8 with the following escaped: quotation mark, reverse solidus, and the control characters (U+0000 through U+001F)</summary>
		public static ConversionResult ConvertUTF16toJson (
				ref UInt16* sourceStart, UInt16* sourceEnd, 
				ref Byte* targetStart, Byte* targetEnd) {
				
			const Int32 byteMask = 0xBF;
			const Int32 byteMark = 0x80; 
			
			ConversionResult result = conversionOK;
			UInt16* source = sourceStart;
			Byte* target = targetStart;
			
			while (source < sourceEnd) {
				Int32 ch;
				
				UInt16* oldSource = source; /* In case we have to back up because of target overflow. */
				ch = *source++;
				/* If we have a surrogate pair, convert to UInt32 first. */
				if (ch >= UNI_SUR_HIGH_START && ch <= UNI_SUR_HIGH_END) {
					/* If the 16 bits following the high surrogate are in the source buffer... */
					if (source < sourceEnd) {
						Int32 ch2 = *source;
						/* If it's a low surrogate, convert to UInt32. */
						if (ch2 >= UNI_SUR_LOW_START && ch2 <= UNI_SUR_LOW_END) {
							ch = (int)(((ch - UNI_SUR_HIGH_START) << halfShift)
								+ (ch2 - UNI_SUR_LOW_START) + halfBase);
							++source;
						}
					} else { /* We don't have the 16 bits following the high surrogate. */
						--source; /* return to the high surrogate */
						result = sourceExhausted;
						break;
					}
				}

				/* Figure out how many bytes the result will require */
				Int32 bytesToWrite;
				Int32 byteMark_len = 0;
				byte* escaped = null;
				if (ch < 0x80) {
					escaped = JsonEscapes + ch * JSON_ESCAPE_MAX_LENGTH;
					bytesToWrite = 1;
					if (*escaped == '\\') {
						bytesToWrite = 2;
						if (*(escaped + 1) == 'u') {
							bytesToWrite = 6;
						}
					}
				} else if (ch < 0x800) {
					bytesToWrite = 2;
					byteMark_len = firstByteMark_2;
				} else if (ch < 0x10000) {
					bytesToWrite = 3;
					byteMark_len = firstByteMark_3;
				} else if (ch < 0x110000) {
					bytesToWrite = 4;
					byteMark_len = firstByteMark_4;
				} else {
					bytesToWrite = 3;
					ch = UNI_REPLACEMENT_CHAR;
					byteMark_len = firstByteMark_3;
				}

				if (target + bytesToWrite > targetEnd) {
					source = oldSource; /* Back up source pointer! */
					result = targetExhausted; break;
				}
				
				if (escaped != null) {
					switch (bytesToWrite) { /* note: everything falls through. */
						case 6:
							*target++ = *escaped++;
							*target++ = *escaped++;
							*target++ = *escaped++;
							*target++ = *escaped++;
							goto case 2;
						case 2:
							*target++ = *escaped++;
							goto case 1;
						case 1:
							*target++ = *escaped++;
							break;
					}
				} else {
					target += bytesToWrite;
					switch (bytesToWrite) { /* note: everything falls through. */
						case 4: *--target = (Byte)((ch | byteMark) & byteMask); ch >>= 6; goto case 3;
						case 3: *--target = (Byte)((ch | byteMark) & byteMask); ch >>= 6; goto case 2;
						case 2: *--target = (Byte)((ch | byteMark) & byteMask); ch >>= 6; goto case 1;
						case 1: *--target =  (Byte)(ch | byteMark_len);    break;
					}
					target += bytesToWrite;
				}
			}
			sourceStart = source;
			targetStart = target;
			return result;
		}
	}
}
