using System;

namespace Eto.Parse
{
	static class TypeExtensions
	{
		#if net40
		public static Type GetTypeInfo(this Type type)
		{
			return type;
		}
		#endif

	}
}

