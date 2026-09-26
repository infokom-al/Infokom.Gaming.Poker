//using static Infokom.Numerics.Atomics.FloatingPointConstant;
using static Infokom.Numerics.Extensions.Constants;

namespace Infokom.Numerics.Atomics
{
	public readonly struct Angle
	{
		private readonly sbyte _data;

		private const decimal Scale = π / 128;

		private Angle(sbyte data)
		{
			_data = data;
		}

		public decimal Radians => _data * Scale;

		public static readonly Angle Zero = new(0);

		public static readonly Angle Pi = new(sbyte.MinValue);

		public static readonly Angle MaxValue = new(sbyte.MaxValue);

		public static readonly Angle MinValue = new(sbyte.MinValue);


		public static Angle FromRadians(decimal radians)
		{
			radians %= 2 * π; // Normalize to [-π, π)
			if (radians >= π)
				radians -= 2 * π;
			else if (radians < -π)
				radians += 2 * π;
			return new Angle((sbyte)Math.Round(128 * radians / π));
		}

		public static implicit operator Angle(decimal x)
		{
			
			x %= 2 * π;// Normalize to [-π, π)

			if (x >= π)
				x -= 2 * π;
			else if (x < -π)
				x += 2 * π;

			return new Angle((sbyte)Math.Round(128 * x / π));
		}
	}
}
