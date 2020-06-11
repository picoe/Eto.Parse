using System;
namespace Eto.Parse
{
#if NETSTANDARD1_0
    interface ICloneable
	{
		object Clone();
	}
#endif
}

