using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cameronism.Json
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

			var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public);

			var getEnumeratorCandidates = methods
				.Where(mi => mi.Name == "GetEnumerator" && mi.GetParameters().Length == 0 && mi.ReturnType != typeof(object) && mi.ReturnType != typeof(void))
				.ToList();

			var genericEnumerable = GetGenericEnumerable(t);
			if (genericEnumerable != null)
			{
				getEnumeratorCandidates.Add(genericEnumerable.GetMethod("GetEnumerator"));
			}

			getEnumeratorCandidates.Sort((m1, m2) =>
			{
				// prefer the public method
				if (m1.IsPublic != m2.IsPublic) return m1.IsPublic ? -1 : 1;

				var r1 = m1.ReturnType;
				var r2 = m2.ReturnType;
				// prefer the concrete type
				if (r1.IsInterface != r2.IsInterface) return r1.IsInterface ? 1 : -1;

				// prefer the struct
				if (r1.IsValueType != r2.IsValueType) return r1.IsValueType ? 1 : -1;

				return 0;
			});

			ie.GetEnumerator = getEnumeratorCandidates.FirstOrDefault();

			var genericCollection = GetGenericCollection(t);

			// only use get_Count if ICollection<> or IReadOnlyCollection<> are implemented
			if (genericCollection != null)
			{
				ie.get_Count = methods
					.FirstOrDefault(mi => mi.IsSpecialName && mi.ReturnType == typeof(int) && mi.Name == "get_Count" && mi.GetParameters().Length == 0) ??
					genericCollection.GetMethod("get_Count");
			}

			if (ie.GetEnumerator == null) return ie;

			var enumeratorType = ie.GetEnumerator.ReturnType;
			methods = enumeratorType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

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

		static Type GetGenericEnumerable(Type t)
		{
			if (t.IsInterface && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				return t;
			}

			return t.GetInterfaces().Select(GetGenericEnumerable).FirstOrDefault(ti => ti != null);
		}

		static Type GetGenericCollection(Type t)
		{
			var genericDef = t.IsInterface && t.IsGenericType ? t.GetGenericTypeDefinition() : null;
			if (genericDef != null && (genericDef == typeof(ICollection<>) || genericDef == typeof(IReadOnlyCollection<>)))
			{
				return t;
			}

			return t.GetInterfaces().Select(GetGenericCollection).FirstOrDefault(ti => ti != null);
		}
	}
}
