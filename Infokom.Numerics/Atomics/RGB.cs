//using static Infokom.Numerics.Atomics.FloatingPointConstant;
using System.Runtime.InteropServices;
using System.Xml.XPath;

using static Infokom.Numerics.Extensions.Constant;

namespace Infokom.Numerics.Atomics
{


	/// <summary>
	/// Represents a color in the RGBA byte-order scheme where each channel is represented by a byte (0-255).
	/// </summary>
	/// <remarks>
	/// In OpenGL and Portable Network Graphics (PNG), the RGBA byte order is used, where the colors are stored in memory such that R is at the 
	/// lowest address, G after it, B after that, and A last. On a little endian architecture this is equivalent to ABGR32. 
	/// </remarks>
	[StructLayout(LayoutKind.Explicit)]
	public readonly struct RGBA
	{
		[FieldOffset(0)] private readonly uint _data;

		private RGBA(uint data) => _data = data;


		/// <summary>
		/// Red channel of this RGBA encoded color.
		/// </summary>
		[FieldOffset(0)] public readonly byte R;

		/// <summary>
		/// Green channel of this RGBA encoded color.
		/// </summary>
		[FieldOffset(1)] public readonly byte G;

		/// <summary>
		/// Blue channel of this RGBA encoded color.
		/// </summary>
		[FieldOffset(2)] public readonly byte B;

		/// <summary>
		/// Alpha channel of this RGBA encoded color.
		/// </summary>
		[FieldOffset(3)] public readonly byte A;

		private RGBA(byte r, byte g, byte b, byte a) => (R, G, B, A) = (r, g, b, a);



		public void Deconstruct(out byte r, out byte g, out byte b, out byte a) => (r, g, b, a) = (R, G, B, A);


		public static RGBA operator *(RGBA x, float k)
		{	
		
			var a = x.A / 255f;
			var b = x.B / 255f;
			var g = x.G / 255f;
			var r = x.R / 255f;

			b *= a;
			g *= a;
			r *= a;

			b /= (r + g + b);
			g /= (r + g + b);
			r /= (r + g + b);


			//ka = b+g+r
			//a = (b+g+r)/k

			a = (b + g + r) / k;

			a /= (b + g + r + a);
			b /= (b + g + r + a);
			g /= (b + g + r + a);
			r /= (b + g + r + a);

			return new RGBA((byte)(r*255), (byte)(g*255), (byte)(b*255), (byte)(a * 255));
		}

		public static RGBA operator +(RGBA x, RGBA y)
		{
			var A = x.A + y.A * (1 - x.A);
			var B = x.B * x.A + y.B * y.A * (1 - x.A);
			var G = x.G * x.A + y.G * y.A * (1 - x.A);
			var R = x.R * x.A + y.R * y.A * (1 - x.A);

			var (r, g, b, a) = ((byte)(R * 255), (byte)(G * 255), (byte)(B * 255), (byte)(A * 255));

			return new RGBA(r, g, b, a);
		}




		public static readonly RGBA Zero = new(0x00000000);
		public static readonly RGBA UnitR = new(0x000000FF);
		public static readonly RGBA UnitG = new(0x0000FF00);
		public static readonly RGBA UnitB = new(0x00FF0000);
		public static readonly RGBA UnitA = new(0xFF000000);


		public static readonly RGBA Transparent = new(0x00000000);

		public static readonly RGBA Black	= new(0xFF000000);
		public static readonly RGBA Red	= new(0xFF0000FF);
		public static readonly RGBA Green	= new(0xFF00FF00);
		public static readonly RGBA Blue	= new(0xFFFF0000);


		public static readonly RGBA Orange = Red + Green * 0.5f;
		public static readonly RGBA Yellow = Red + Green;
		public static readonly RGBA Cyan = Green + Blue;
		public static readonly RGBA Magenta = Red + Blue;
		public static readonly RGBA White = Red + Green + Blue;




		/// <summary>
		/// Color from the heat map gradient based on the given weight (0.0 to 1.0).
		/// </summary>
		/// <param name="x">The weight value between 0.0 and 1.0. </param>
		/// <returns>
		/// red: hot
		/// green: warm
		/// blue: cold
		/// </returns>
		public static RGBA Heat(float x)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
			ArgumentOutOfRangeException.ThrowIfGreaterThan(x, 1f, nameof(x));


			var p = Point<double, double, double>.Zeros;
			p.X = x < 0.5f ? 0     : -1 + 2 * x;
			p.Y = x < 0.5f ? x * 2 :  1 - 2 * x;	
			p.Z = x > 0.5f ? 0     :  1 - 2 * x;

			return Red * (float)p.X + Green * (float)p.Y + Blue * (float)p.Z;
		}
	}




	/// <summary>
	/// Represents a color in the BGRA byte-order scheme where each channel is represented by a byte (0-255).
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct BGRA
	{
		[FieldOffset(0)] private readonly uint _data;

		/// <summary>
		/// Blue channel of this BGRA encoded color.
		/// </summary>
		[FieldOffset(0)] public byte B;
		/// <summary>
		/// Green channel of this BGRA encoded color.
		/// </summary>
		[FieldOffset(1)] public byte G;
		/// <summary>
		/// Red channel of this BGRA encoded color.
		/// </summary>
		[FieldOffset(2)] public byte R;
		/// <summary>
		/// Alpha channel of this BGRA encoded color.
		/// </summary>
		[FieldOffset(3)] public byte A;
	}
}