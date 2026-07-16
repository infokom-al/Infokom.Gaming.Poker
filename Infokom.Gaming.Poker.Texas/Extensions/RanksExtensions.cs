using Infokom.Gaming.Poker.Atomics;
using Infokom.Numerics.Extensions;

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Holdem.Core.Extensions
{
	public static class RanksExtensions
	{	

		extension(Cards)
		{
			public static bool HasAnyStraight(Cards source)
			{
				var mask = source.Ranks.ID;

				if ((mask & 0b1000000001111) == 0b1000000001111)
				{
					return true;
				}

				mask &= mask << 1;
				mask &= mask << 2;
				mask &= mask << 1;



				return mask != 0;
			}


			private static Ranks FilterStraight(Cards source)
			{
				var ranks = source.Ranks;

				if (((uint)ranks & 0x1F00) == 0x1F00) return (Ranks)0x1F00; // AKQJT
				if (((uint)ranks & 0x0F80) == 0x0F80) return (Ranks)0x0F80; // KQJT9
				if (((uint)ranks & 0x07C0) == 0x07C0) return (Ranks)0x07C0; // QJT98
				if (((uint)ranks & 0x03E0) == 0x03E0) return (Ranks)0x03E0; // JT987
				if (((uint)ranks & 0x01F0) == 0x01F0) return (Ranks)0x01F0; // T9876
				if (((uint)ranks & 0x00F8) == 0x00F8) return (Ranks)0x00F8; // 98765
				if (((uint)ranks & 0x007C) == 0x007C) return (Ranks)0x007C; // 87654
				if (((uint)ranks & 0x003E) == 0x003E) return (Ranks)0x003E; // 76543
				if (((uint)ranks & 0x001F) == 0x001F) return (Ranks)0x001F; // 65432
				if (((uint)ranks & 0x100F) == 0x100F) return (Ranks)0x100F; // 5432A (Ace-low straight)

				return 0;
			}

			public static bool TryGetStraight(Cards source, out Rank rank)
			{
				var mask = source.Ranks.ID;

				if ((mask & 0b1000000001111) == 0b1000000001111)
				{
					rank = Rank.Five;
					return true;
				}

				mask = mask & (mask << 1) & (mask << 2) & (mask << 3) & (mask << 4);

				if (mask == 0)
				{
					rank = default;
					return false;
				}

				var start = mask.UppBit;

				rank = Rank.Values[start + 4];

				return true;
			}


			/// <summary>
			/// <see cref=get"/>
			/// </summary>
			/// <param name="source"></param>
			/// <returns></returns>
			public static bool HasAnyFlush(Cards source)
			{
				var (c, d, h, s) = source;

				var (nc, nd, nh, ns) = (c.Count, d.Count, h.Count, s.Count);

				if (nc >= 5 || nd >= 5 || nh >= 5 || ns >= 5)
				{
					
					


					var r = Ranks.None;
					if (nc >= 5)
					{
						r = (Ranks)Math.Max((ulong)r, (ulong)c);
					}

					if (nd >= 5)
					{
						r = (Ranks)Math.Max((ulong)r, (ulong)d);
					}

					if (nh >= 5)
					{
						r = (Ranks)Math.Max((ulong)r, (ulong)h);
					}

					if (ns >= 5)
					{
						r = (Ranks)Math.Max((ulong)r, (ulong)s);
					}



				}

					var ss = (c & d & h & s).Count;
				return ;
			}
		}


		extension(Cards source)
		{
			public bool HasStraight => Cards.HasAnyStraight(source);

			public bool HasFlush => Cards.HasAnyFlush(source);
		}
	}
}
