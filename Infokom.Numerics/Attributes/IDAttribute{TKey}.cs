using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text;

namespace Infokom.Numerics.Attributes
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public class IDAttribute<TKey>(TKey id) : Attribute where TKey : unmanaged, IBinaryInteger<TKey>, IUnsignedNumber<TKey>
	{
		public TKey ID { get; } = id;
	}
}
