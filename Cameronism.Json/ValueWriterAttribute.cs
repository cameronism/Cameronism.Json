using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cameronism.Json
{
	[AttributeUsage(AttributeTargets.Method)]
	class ValueWriterAttribute : Attribute
	{
		public int MaxLength { get; set; }
		public int MinLength { get; set; }

		public ValueWriterAttribute()
		{
			MinLength = 1;
		}
	}
}
