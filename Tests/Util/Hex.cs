using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Util
{
	public static class Hex
	{
		private static void PrintSafe(StringBuilder builder, char[] printable, int start, int count)
		{
			for (int i = 0; i < count; i++)
			{
				//string s = null;
				char ch = printable[start + i];
				//switch (ch)
				//{
				//	case '<': s = "&lt;"; break;
				//	case '>': s = "&gt;"; break;
				//	case '"': s = "&quot;"; break;
				//	case '&': s = "&amp;"; break;
				//}
				
				//if (s != null) 
				//	builder.Append(s);
				//else 
					builder.Append(ch);
			}
		}

		private static void ShowPrintable(StringBuilder builder, char[] printable, int count)
		{
			PrintSafe(builder, printable, 0, Math.Min(8, count));
			if (count < 9) return;
			builder.Append(' ');
			PrintSafe(builder, printable, 8, count - 8);
		}

		public static StringBuilder Dump(IEnumerable<byte> bytes)
		{
			var sb = new StringBuilder();
			Dump(sb, bytes);
			return sb;
		}

		public static void Dump(StringBuilder builder, IEnumerable<byte> bytes)
		{
			char[] printable = Enumerable.Repeat(' ', 16).ToArray();
			
			builder.AppendLine();
			
		
			int count = 0;
			int index = 0;
			foreach (var b in bytes)
			{
				index = count % 16;
				
				if (index == 0)
				{
					if (count > 0)
					{
						builder.Append(' ', 3);
						ShowPrintable(builder, printable, 16);
					}
					
					builder.AppendLine();
					builder.Append(count.ToString("x4"));
				}
				
				printable[index] = b >= 0x20 && b <= 0x7e ? (char)b : '.';
				
				if (count % 8 == 0)
				{
					builder.Append(' ', 3);
				}
				else
				{
					builder.Append(' ');
				}
				
				builder.Append(b.ToString("x2"));

				count++;
			}
			
			if (count > 0)
			{
				builder.Append(' ', ((16 - index) * 3) + (index < 8 ? 2 : 0));
				ShowPrintable(builder, printable, index + 1);
			}
			else
			{
				builder.Append("zero bytes dumped");
				return;
			}
		}
	}
}
