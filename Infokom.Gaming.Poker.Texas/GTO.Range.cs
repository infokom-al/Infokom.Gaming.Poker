using Holdem.Core.Extensions;

using Infokom.Gaming.Poker.Texas;
using Infokom.Numerics;

using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;


namespace Holdem.Core
{

	public static partial class GTO
	{

		// language=regex
		private const string PAIR_PATTERN = @"^([2-9TJQKA])\1(\+|(\-([2-9TJQKA])\4))?$";

		// language=regex
		private const string SUITED_PATTERN = @"^([2-9TJQKA])(?!\\1)([2-9TJQKA])s(\\+|(\\-([2-9TJQKA])(?!\\5)([2-9TJQKA])s))?$";

		// language=regex
		private const string OFFSUITED_PATTERN = @"^([2-9TJQKA])(?!\1)([2-9TJQKA])o(\+|(\-([2-9TJQKA])(?!\5)([2-9TJQKA])o))?$";




		public partial class Range
		{
			private readonly string _expression;
			private readonly Mask _mask;
			private readonly Dictionary<Mask, List<Pocket>> _pockets;

			private Range(string expression, Mask mask)
			{
				_expression = expression;
				_mask = mask;
			}

			internal Mask ToMask() => this._mask;

			public override string ToString() => _expression;

			public static Range Parse(string expression)
			{
				if(!GTO.Range.Mask.TryParse(expression, out var mask))
				{
					throw new FormatException();
				}

				return new(expression, mask);
			}			



			public static Range Empty {  get; } = new Range("", Mask.Min);
			public static Range Full { get; } = new Range("Full", Mask.Max);


			public static Range Of(GTO.Cell cell)
			{
				return new Range(cell.Symbol, Mask.Of(cell));
			}


		}
	}



}
