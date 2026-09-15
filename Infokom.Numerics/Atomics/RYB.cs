using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Infokom.Numerics.Atomics
{
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	public readonly struct RYB
	{
		[FieldOffset(0)] private readonly byte _r;
		[FieldOffset(1)] private readonly byte _y;
		[FieldOffset(2)] private readonly byte _b;
		[FieldOffset(3)] private readonly byte __;	


		public float R => _r / 255f;
		public float Y => _y / 255f;
		public float B => _b / 255f;


		private RYB(byte red, byte yellow, byte blue)
		{
			_r = red;
			_y = yellow;
			_b = blue;
		}






		public override string ToString() => $"RYB Byte: ({_r}, {_y}, {_b}) | Float: ({R:F2}, {Y:F2}, {B:F2})";





		public static readonly RYB UnitR = new(0xFF, 0x00, 0x00);
		public static readonly RYB UnitY = new(0x00, 0xFF, 0x00);
		public static readonly RYB UnitB = new(0x00, 0x00, 0xFF);













		/// <summary>
		/// Konverton një ngjyrë standarde RGB në RYB (0-255)
		/// </summary>
		public static RYB From(Color rgbColor)
		{
			float r = rgbColor.R / 255f;
			float g = rgbColor.G / 255f;
			float b = rgbColor.B / 255f;

			// 1. Hiqet pjesa e bardhë
			float w = Math.Min(r, Math.Min(g, b));
			r -= w;
			g -= w;
			b -= w;

			float maxGreen = Math.Max(r, Math.Max(g, b));

			// 2. Nxirret e verdha
			float y = Math.Min(r, g);
			r -= y;
			g -= y;

			if (b > 0 && g > 0)
			{
				b /= 2f;
				g /= 2f;
			}

			y += g;
			b += g;

			// 3. Normalizimi
			float maxRyb = Math.Max(r, Math.Max(y, b));
			if (maxRyb > 0 && maxGreen > 0)
			{
				float factor = maxGreen / maxRyb;
				r *= factor;
				y *= factor;
				b *= factor;
			}

			// 4. Rikthehet pjesa e bardhë
			r += w;
			y += w;
			b += w;

			// Kthehen në byte (0-255) duke i rrumbullakosur saktë
			return new RYB(
			    (byte)Math.Clamp(Math.Round(r * 255f), 0, 255),
			    (byte)Math.Clamp(Math.Round(y * 255f), 0, 255),
			    (byte)Math.Clamp(Math.Round(b * 255f), 0, 255)
			);
		}

		/// <summary>
		/// Konverton ngjyrën RYB mbrapsht në RGB standarde
		/// </summary>
		public Color ToRgb()
		{
			// Përdorim vetitë float për llogaritjen
			float r = R;
			float y = Y;
			float b = B;

			float w = Math.Min(r, Math.Min(y, b));
			r -= w;
			y -= w;
			b -= w;

			float maxRyb = Math.Max(r, Math.Max(y, b));

			float g = Math.Min(y, b);
			y -= g;
			b -= g;

			if (b > 0 && g > 0)
			{
				b *= 2.0f;
				g *= 2.0f;
			}

			r += y;
			g += y;

			float maxRgb = Math.Max(r, Math.Max(g, b));
			if (maxRgb > 0 && maxRyb > 0)
			{
				float factor = maxRyb / maxRgb;
				r *= factor;
				g *= factor;
				b *= factor;
			}

			r += w;
			g += w;
			b += w;

			return Color.FromArgb(
			    (int)Math.Clamp(Math.Round(r * 255f), 0, 255),
			    (int)Math.Clamp(Math.Round(g * 255f), 0, 255),
			    (int)Math.Clamp(Math.Round(b * 255f), 0, 255)
			);
		}

	}
}
