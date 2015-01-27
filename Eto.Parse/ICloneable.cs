using System;
namespace Eto.Parse
{
	#if PCL
	interface ICloneable
	{
		object Clone();
	}
	#endif
}

