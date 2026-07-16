using Holdem.Core.Internal;

using Infokom.Numerics;

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text.RegularExpressions;


namespace Holdem.Core
{

	public static partial class GTO
	{
		public static Matrix<GTO.Cell> CELLS { get; } = Matrix.Create(13, 13, (i, j) => Cell.FromIndex(i, j));





	}



}
