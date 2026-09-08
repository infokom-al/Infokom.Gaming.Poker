namespace Poker.Calc;

public static class PreflopRangeHoldemLiteral
{
	public static string GetDefaultLiteral(this PocketRangeHoldem pocketsRange)
	{
		PocketRangeHoldem tail;
		return $"{pocketsRange.GetPreflopRangeHoldem(out tail).GetDefaultLiteral()} {tail}".Trim();
	}

	public static string GetDefaultLiteral(this PreflopRangeHoldem preflopRange)
	{
		if (preflopRange.IsEmpty)
		{
			return string.Empty;
		}
		PreflopRangeHoldem preflopRangeHoldem = preflopRange.PairCells.ToPreflopRangeHoldem();
		PreflopRangeHoldem preflopRangeHoldem2 = preflopRange.GetUnpairedRange().ToPreflopRangeHoldem();
		PreflopRangeHoldem preflopRangeHoldem3 = preflopRange.Except(preflopRangeHoldem2).SuitedCells.ToPreflopRangeHoldem();
		PreflopRangeHoldem preflopRangeHoldem4 = preflopRange.Except(preflopRangeHoldem2).Except(preflopRangeHoldem).OffsuitedCells.ToPreflopRangeHoldem();
		string text = preflopRangeHoldem.GetPairsRangeLiteral();
		if (preflopRangeHoldem2.IsNotEmpty)
		{
			text = text + " " + preflopRangeHoldem2.SuitedCells.ToPreflopRangeHoldem().GetSuitedRangeLiteral().Remove("s");
		}
		if (preflopRangeHoldem3.IsNotEmpty)
		{
			text = text + " " + preflopRangeHoldem3.GetSuitedRangeLiteral();
		}
		if (preflopRangeHoldem4.IsNotEmpty)
		{
			text = text + " " + preflopRangeHoldem4.Cells.Select((PreflopRangeHoldemCell x) => x.GetAlternativeCell()).ToPreflopRangeHoldem().GetSuitedRangeLiteral()
				.Replace("s", "o");
		}
		return text.Trim();
	}

	public static string GetDefaultLiteral(this PocketsRangeWeightedHoldem rangeWeightedHoldem)
	{
		return (from item in rangeWeightedHoldem.Range
			   group item by item.weightUnitInterval into @group
			   select (weight: @group.Key, individualLiterals: @group.Select(((PocketCardsHoldem pocket, double weightUnitInterval) item) => item.pocket).ToPocketsRangeHoldem().GetDefaultLiteral()
				   .Split(' ')) into item
			   select item.individualLiterals.Select(delegate (string literal)
			   {
				   object obj;
				   if (item.weight == 1.0)
				   {
					   obj = literal;
					   if (obj == null)
					   {
						   return "";
					   }
				   }
				   else
				   {
					   obj = $"{literal} {item.weight * 100.0:0.#}%";
				   }
				   return (string)obj;
			   }).JoinStrings()).JoinStrings();
	}

	private static string GetSuitedRangeLiteral(this PreflopRangeHoldem range)
	{
		HashSet<Point> pointHashSet = range.Cells.Where((PreflopRangeHoldemCell x) => x.IsSuited).GetPointHashSet();
		string text = string.Empty;
		foreach (Point item in pointHashSet.OrderByDescending((Point x) => x, new LambdaComparer<Point>((Point x, Point y) => x.GetCell().CompareByRanks(y.GetCell()))).ToList())
		{
			if (pointHashSet.Contains(item))
			{
				Line horizontalLine = pointHashSet.GetHorizontalLine(item);
				Line verticalLine = pointHashSet.GetVerticalLine(item);
				Line diagonal = pointHashSet.GetDiagonal(item);
				int maxLength = Math.Max(verticalLine.Length, Math.Max(horizontalLine.Length, diagonal.Length));
				Line line = new Line[3] { verticalLine, horizontalLine, diagonal }.First((Line x) => x.Length == maxLength);
				text = text.AddWord(line.GetSuitedLineLiteral());
				pointHashSet.RemoveAll(line.GetAllPoints());
			}
		}
		return text.Trim();
	}

	public static IEnumerable<PreflopRangeHoldemCell> GetUnpairedRange(this PreflopRangeHoldem range)
	{
		HashSet<PreflopRangeHoldemCell> cells = range.Cells.ToHashSet();
		foreach (PreflopRangeHoldemCell cell in range.Cells)
		{
			if (!cell.IsPair && cells.Contains(cell.GetAlternativeCell()))
			{
				yield return cell;
			}
		}
	}

	public static string GetPairsRangeLiteral(this PreflopRangeHoldem range)
	{
		HashSet<Point> points = range.Cells.Where((PreflopRangeHoldemCell x) => x.IsPair).GetPointHashSet();
		string text = string.Empty;
		foreach (Point item in points.OrderBy((Point x) => x.X).ToList())
		{
			if (points.Contains(item))
			{
				Line diagonal = points.GetDiagonal(item);
				text = text + " " + GetPairsDiagonalLiteral(diagonal);
				diagonal.GetAllPoints().ForEach(delegate (Point x)
				{
					points.Remove(x);
				});
			}
		}
		return text.Trim();
		static string GetPairsDiagonalLiteral(Line line)
		{
			line.VerifyIsDiagonal();
			PreflopRangeHoldemCell value = line.TopLeft.GetCell().VerifyIsPair();
			PreflopRangeHoldemCell value2 = line.BottomRight.GetCell().VerifyIsPair();
			if (line.IsSinglePoint)
			{
				return value.Abbreviation;
			}
			if (value.HighRank.IsAce())
			{
				return $"{value2}+";
			}
			if (line.Length != 2)
			{
				return $"{value2}-{value}";
			}
			return value.Abbreviation.AddWord(value2.Abbreviation);
		}
	}

	private static PreflopRangeHoldemCell GetAlternativeCell(this PreflopRangeHoldemCell cell)
	{
		if (!cell.IsPair)
		{
			return new PreflopRangeHoldemCell(cell.HighRank, cell.LowRank, cell.Suitness.Switch());
		}
		return cell;
	}

	private static string GetSuitedLineLiteral(this Line line)
	{
		if (line.IsSinglePoint)
		{
			return line.Start.GetCell().VerifyIsSuited().Abbreviation;
		}
		PreflopRangeHoldemCell preflopRangeHoldemCell = line.TopLeft.GetCell().VerifyIsSuited();
		PreflopRangeHoldemCell preflopRangeHoldemCell2 = line.BottomRight.GetCell().VerifyIsSuited();
		if (preflopRangeHoldemCell.HighRank == preflopRangeHoldemCell2.HighRank && preflopRangeHoldemCell.HighRank - preflopRangeHoldemCell.LowRank == 1 && !line.IsVerticalLine)
		{
			return preflopRangeHoldemCell2.Abbreviation + "+";
		}
		if (line.Length == 2)
		{
			return preflopRangeHoldemCell.Abbreviation + " " + preflopRangeHoldemCell2.Abbreviation;
		}
		return preflopRangeHoldemCell2.Abbreviation.Substring(0, 2) + "-" + preflopRangeHoldemCell.Abbreviation;
	}

	private static string GetSuitedDiagonalLiteral(this Line line)
	{
		line.VerifyIsDiagonal();
		if (line.IsSinglePoint)
		{
			return line.Start.GetCell().VerifyIsSuited().Abbreviation;
		}
		PreflopRangeHoldemCell preflopRangeHoldemCell = line.TopLeft.GetCell().VerifyIsSuited();
		PreflopRangeHoldemCell preflopRangeHoldemCell2 = line.BottomRight.GetCell().VerifyIsSuited();
		if (preflopRangeHoldemCell.HighRank.IsAce())
		{
			return preflopRangeHoldemCell2.Abbreviation + "+";
		}
		if (line.Length == 2)
		{
			return preflopRangeHoldemCell.Abbreviation + " " + preflopRangeHoldemCell2.Abbreviation;
		}
		return preflopRangeHoldemCell2.Abbreviation.Substring(2) + "-" + preflopRangeHoldemCell.Abbreviation;
	}

	private static Line GetDiagonal(this HashSet<Point> points, Point startingPoint)
	{
		Line result = startingPoint.ToLine(startingPoint);
		for (int i = 0; i < int.MaxValue; i++)
		{
			if (points.Contains(result.End.Shift(1, 1)))
			{
				result = new Line(result.Start, result.End.Shift(1, 1));
				continue;
			}
			return result;
		}
		return result;
	}

	private static Line GetHorizontalLine(this HashSet<Point> points, Point startingPoint)
	{
		Line line = startingPoint.ToLine(startingPoint);
		for (int i = 0; i < int.MaxValue; i++)
		{
			if (points.Contains(line.End.ShiftRight()))
			{
				line = line.ExtendRight();
				continue;
			}
			return line;
		}
		return line;
	}

	private static Line GetVerticalLine(this HashSet<Point> points, Point startingPoint)
	{
		Line line = startingPoint.ToLine(startingPoint);
		for (int i = 0; i < int.MaxValue; i++)
		{
			if (points.Contains(line.End.ShiftBottom()))
			{
				line = line.ExtendBottom();
				continue;
			}
			return line;
		}
		return line;
	}

	private static HashSet<Point> GetPointHashSet(this IEnumerable<PreflopRangeHoldemCell> cells)
	{
		return cells.Select(GetPoint).ToHashSet();
	}

	internal static PreflopRangeHoldemCell GetCell(this Point point)
	{
		return new PreflopRangeHoldemCell((CardRanks)(12 - point.X), (CardRanks)(12 - point.Y), (point.X > point.Y) ? Suitness.Suited : Suitness.Offsuited);
	}

	internal static Point GetPoint(this PreflopRangeHoldemCell cell)
	{
		return new Point(cell.Column, cell.Row);
	}

	public static string GetCellIndexAbbreviation(this int cellIndex)
	{
		return cellIndex.ToPreflopRangeHoldemCell().Abbreviation;
	}

	public static double GetCellIndexWeight(this int cellIndex)
	{
		return 1.0 / (double)cellIndex.ToPreflopRangeHoldemCell().Combos;
	}
}
