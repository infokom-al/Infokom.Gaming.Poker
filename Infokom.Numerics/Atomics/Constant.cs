namespace Infokom.Numerics.Extensions
{
#pragma warning disable IDE1006 // Naming Styles

	public readonly struct Constant : IConvertible
	{
		private readonly decimal _value;

		private Constant(decimal value)
		{
			_value = value;
		}

		#region IConvertible Implementation
		public TypeCode GetTypeCode() => this._value.GetTypeCode();
		public bool ToBoolean(IFormatProvider provider) => ((IConvertible)this._value).ToBoolean(provider);
		public byte ToByte(IFormatProvider provider) => ((IConvertible)this._value).ToByte(provider);
		public char ToChar(IFormatProvider provider) => ((IConvertible)this._value).ToChar(provider);
		public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)this._value).ToDateTime(provider);
		public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)this._value).ToDecimal(provider);
		public double ToDouble(IFormatProvider provider) => ((IConvertible)this._value).ToDouble(provider);
		public short ToInt16(IFormatProvider provider) => ((IConvertible)this._value).ToInt16(provider);
		public int ToInt32(IFormatProvider provider) => ((IConvertible)this._value).ToInt32(provider);
		public long ToInt64(IFormatProvider provider) => ((IConvertible)this._value).ToInt64(provider);
		public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)this._value).ToSByte(provider);
		public float ToSingle(IFormatProvider provider) => ((IConvertible)this._value).ToSingle(provider);
		public string ToString(IFormatProvider provider) => this._value.ToString(provider);
		public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)this._value).ToType(conversionType, provider);
		public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)this._value).ToUInt16(provider);
		public uint ToUInt32(IFormatProvider provider) => ((IConvertible)this._value).ToUInt32(provider);
		public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)this._value).ToUInt64(provider);
		#endregion


		public static implicit operator Half(Constant source) => (Half)source._value;
		public static implicit operator float(Constant source) => (float)source._value;
		public static implicit operator double(Constant source) => (double)source._value;
		public static implicit operator decimal(Constant source) => source._value;


		public static decimal operator +(Constant c) => c._value;
		public static decimal operator -(Constant c) => -c._value;

		public static Half operator +(Constant c, Half x) => (Half)c._value + x;
		public static float operator +(Constant c, float x) => (float)c._value + x;
		public static double operator +(Constant c, double x) => (double)c._value + x;
		public static decimal operator +(Constant c, decimal x) => c._value + x;

		public static Half operator +(Half x, Constant c) => x + (Half)c._value;
		public static float operator +(float x, Constant c) => x + (float)c._value;
		public static double operator +(double x, Constant c) => x + (double)c._value;
		public static decimal operator +(decimal x, Constant c) => x + c._value;


		public static Half operator -(Constant c, Half x) => (Half)c._value - x;
		public static float operator -(Constant c, float x) => (float)c._value - x;
		public static double operator -(Constant c, double x) => (double)c._value - x;
		public static decimal operator -(Constant c, decimal x) => c._value - x;

		public static Half operator -(Half x, Constant c) => x - (Half)c._value;
		public static float operator -(float x, Constant c) => x - (float)c._value;
		public static double operator -(double x, Constant c) => x - (double)c._value;
		public static decimal operator -(decimal x, Constant c) => x - c._value;

		public static Half operator *(Constant c, Half x) => (Half)c._value * x;
		public static float operator *(Constant c, float x) => (float)c._value * x;
		public static double operator *(Constant c, double x) => (double)c._value * x;
		public static decimal operator *(Constant c, decimal x) => c._value * x;

		public static Half operator *(Half x, Constant c) => x * (Half)c._value;
		public static float operator *(float x, Constant c) => x * (float)c._value;
		public static double operator *(double x, Constant c) => x * (double)c._value;
		public static decimal operator *(decimal x, Constant c) => x * c._value;

		public static Half operator /(Constant c, Half x) => (Half)c._value / x;
		public static float operator /(Constant c, float x) => (float)c._value / x;
		public static double operator /(Constant c, double x) => (double)c._value / x;
		public static decimal operator /(Constant c, decimal x) => c._value / x;


		public static Half operator /(Half x, Constant c) => x / (Half)c._value;
		public static float operator /(float x, Constant c) => x / (float)c._value;
		public static double operator /(double x, Constant c) => x / (double)c._value;
		public static decimal operator /(decimal x, Constant c) => x / c._value;





		public static decimal operator *(Constant c, int k) => c._value * k;
		public static decimal operator /(Constant c, int k) => c._value / k;
		public static decimal operator *(int k, Constant c) => k * c._value;
		public static decimal operator /(int k, Constant c) => k / c._value;



		/// <summary>
		/// The Napiers Number, which is the base of the natural logarithm.
		/// </summary>
		/// <remarks>
		/// - N.J.A.Sloane; <b>Decimal expansion of e.</b>; Entry <see href="https://oeis.org/A001113">A001113</see>  in The On-Line Encyclopedia of Integer Sequences; <see href="https://oeis.org/A001113/constant"/>
		/// </remarks>
		public static readonly Constant e = new(Constants.e);

		/// <summary>
		/// The Pi Number, wich is  the ratio of a circle's circumference to its diameter.
		/// </summary>
		/// <remarks>
		/// N.J.A.Sloane; <b>Decimal expansion of π.</b>; Entry <see href="https://oeis.org/A000796">A000796</see>  in The On-Line Encyclopedia of Integer Sequences; <see href="https://oeis.org/A000796/constant"/>
		/// </remarks>
		public static readonly Constant π = new(Constants.π);

		/// <summary>
		/// Tau constant, which is equal to 2π.
		/// </summary>
		/// <remarks>
		/// N.J.A.Sloane; <b>Decimal expansion of 2π.</b>; Entry <see href="https://oeis.org/A019692">A019692</see>  in The On-Line Encyclopedia of Integer Sequences; <see href="https://oeis.org/A019692/constant"/>
		/// </remarks>
		public static readonly Constant τ = new(Constants.τ);


		/// <summary>
		/// The Golden Ratio.
		/// </summary>
		/// <remarks>
		/// N.J.A.Sloane; <b>Decimal expansion of the golden ratio, φ.</b>; Entry <see href="https://oeis.org/A001622">A001622</see>  in The On-Line Encyclopedia of Integer Sequences; <see href="https://oeis.org/A001622/constant"/>
		/// </remarks>
		public static readonly Constant φ = new(Constants.φ);


		/// <summary>
		/// The Euler-Mascheroni Constant.
		/// </summary>
		/// <remarks>
		/// N.J.A.Sloane; <b>Decimal expansion of Euler's constant (or the Euler-Mascheroni constant), gamma.</b>; Entry <see href="https://oeis.org/A001620">A001620</see>  in The On-Line Encyclopedia of Integer Sequences; <see href="https://oeis.org/A001620/constant"/>
		/// </remarks>
		public static readonly Constant γ = new(Constants.γ);

		
	}
}