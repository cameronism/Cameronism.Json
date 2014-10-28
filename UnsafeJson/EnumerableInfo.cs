using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnsafeJson
{
	internal class EnumerableInfo
	{
		public MethodInfo GetEnumerator { get; private set; }
		public MethodInfo MoveNext { get; private set; }
		public MethodInfo Dispose { get; private set; }
		public MethodInfo get_Current { get; private set; }
		public MethodInfo get_Count { get; private set; }

		static readonly MethodInfo _MoveNext = typeof(System.Collections.IEnumerator).GetMethod("MoveNext");
		static readonly MethodInfo _Dispose = typeof(System.IDisposable).GetMethod("Dispose");

		public static EnumerableInfo FindMethods(Type t)
		{
			var ie = new EnumerableInfo();

			var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			var rand = new Random();
			Array.Sort(methods, (a, b) => rand.NextDouble() >= 0.5 ? 1 : -1);

			// FIXME need to get more selective about which get enumerator to pick 
			ie.GetEnumerator = methods.FirstOrDefault(mi => mi.Name == "GetEnumerator" && mi.GetParameters().Length == 0);
			ie.get_Count = methods.FirstOrDefault(mi => mi.IsSpecialName && mi.Name == "get_Count" && mi.GetParameters().Length == 0);

			if (ie.GetEnumerator == null) return ie;

			var enumeratorType = ie.GetEnumerator.ReturnType;
			methods = enumeratorType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			Array.Sort(methods, (a, b) => rand.NextDouble() >= 0.5 ? 1 : -1);

			ie.MoveNext = methods.FirstOrDefault(mi => mi.Name == "MoveNext");
			ie.Dispose = methods.FirstOrDefault(mi => mi.Name == "Dispose");
			ie.get_Current = methods.FirstOrDefault(mi => mi.IsSpecialName && mi.Name == "get_Current");

			if (ie.MoveNext == null && typeof(System.Collections.IEnumerator).IsAssignableFrom(enumeratorType))
			{
				ie.MoveNext = _MoveNext;
			}

			if (ie.Dispose == null && typeof(System.IDisposable).IsAssignableFrom(enumeratorType))
			{
				ie.Dispose = _Dispose;
			}

			return ie;
		}
	}
}
