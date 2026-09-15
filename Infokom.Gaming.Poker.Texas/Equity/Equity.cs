using Infokom.Numerics.Atomics;

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Gaming.Poker.Texas
{
	public partial class Equity
	{
		
	


		/// <summary>
		/// Range of all possible pocket hands (suited, offsuit, and paired).
		/// </summary>
		public static GTO.Range Range() => GTO.Range.XYo | GTO.Range.XYs | GTO.Range.XX;


	}
}
