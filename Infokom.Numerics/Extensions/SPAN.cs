using System.Runtime.CompilerServices;

namespace Infokom.Numerics.Extensions
{
	public static class SPAN
	{
		extension<T>(in InlineArray2<T> source)
		{
			public ReadOnlySpan<T> AsReadOnlySpan()
			{
				
				return source;
			}
		}
	}

}
