using System;
using System.Text.Json.Serialization;
using BinarySerializer;
using MemoryPools;
using Poker.Calc;
using ProtoBuf;

namespace Hand2NoteCore.HandStrength;

[BinarySerializable]
[ProtoContract]
public sealed record HandStrengthValueExpression : IHandStrengthExpression, IEquatable<HandStrengthValueExpression>
{
	[Tag(1)]
	[ProtoMember(1)]
	public long FlopHandValueMask { get; set; }

	[Tag(2)]
	[ProtoMember(2)]
	public long TurnHandValueMask { get; set; }

	[Tag(3)]
	[ProtoMember(3)]
	public long RiverHandValueMask { get; set; }

	[Tag(4)]
	[ProtoMember(4)]
	public long FlopBoardMask { get; set; }

	[Tag(5)]
	[ProtoMember(5)]
	public long TurnBoardMask { get; set; }

	[Tag(6)]
	[ProtoMember(6)]
	public long RiverBoardMask { get; set; }

	[Tag(7)]
	[ProtoMember(7)]
	public PreflopRangeHoldemCompact PreflopRangeHoldem { get; set; }

	[Tag(8)]
	public OmahaPreflopRange OmahaPreflopRange { get; set; }

	[Tag(9)]
	public Poker.Calc.Range OmahaPreflopTopRange { get; set; }

	public RelevantHandValue FlopHandValue => (RelevantHandValue)FlopHandValueMask;

	public RelevantHandValue TurnHandValue => (RelevantHandValue)TurnHandValueMask;

	public RelevantHandValue RiverHandValue => (RelevantHandValue)RiverHandValueMask;

	public RelevantFlopType FlopBoard => (RelevantFlopType)FlopBoardMask;

	public RelevantTurnType TurnBoard => (RelevantTurnType)TurnBoardMask;

	public RelevantRiverType RiverBoard => (RelevantRiverType)RiverBoardMask;

	[JsonIgnore]
	public RelevantHandBoards Boards => new RelevantHandBoards(FlopBoard, TurnBoard, RiverBoard);

	[JsonIgnore]
	public bool HasHandValue
	{
		get
		{
			if (!HasPreflopRange && FlopHandValueMask == 0L && TurnHandValueMask == 0L)
			{
				return RiverHandValueMask != 0;
			}
			return true;
		}
	}

	[JsonIgnore]
	public bool HasBoard
	{
		get
		{
			if (FlopBoardMask == 0L && TurnBoardMask == 0L)
			{
				return RiverBoardMask != 0;
			}
			return true;
		}
	}

	[JsonIgnore]
	public bool HasFlopHandValue => FlopHandValueMask != 0;

	[JsonIgnore]
	public bool HasTurnHandValue => TurnHandValueMask != 0;

	[JsonIgnore]
	public bool HasRiverHandValue => RiverHandValueMask != 0;

	[JsonIgnore]
	public bool HasFlopBoard => FlopBoardMask != 0;

	[JsonIgnore]
	public bool HasTurnBoard => TurnBoardMask != 0;

	[JsonIgnore]
	public bool HasRiverBoard => RiverBoardMask != 0;

	[JsonIgnore]
	public bool HasPreflopRange
	{
		get
		{
			if (!PreflopRangeHoldem.IsNotEmpty && OmahaPreflopRange.IsEmpty)
			{
				return !OmahaPreflopTopRange.ContainsZeroToHundred;
			}
			return true;
		}
	}

	[JsonIgnore]
	public bool HasHoldemPreflopRange => PreflopRangeHoldem.IsNotEmpty;

	[JsonIgnore]
	public bool HasOmahaPreflopRange
	{
		get
		{
			if (!OmahaPreflopRange.IsNotEmpty)
			{
				return !OmahaPreflopTopRange.ContainsZeroToHundred;
			}
			return true;
		}
	}

	[JsonIgnore]
	public bool HasOmahaPreflopTopRange => !OmahaPreflopTopRange.ContainsZeroToHundred;

	[JsonIgnore]
	public bool IsBoardOnlyExpression
	{
		get
		{
			if (HasBoard)
			{
				return !HasHandValue;
			}
			return false;
		}
	}

	[JsonIgnore]
	public bool IsDefault => Equals(Default);

	public Streets LastStreet => StreetsHelper.MaxStreet(HasHandValue ? GetLastHandValueStreet() : Streets.Preflop, HasBoard ? GetLastHandBoardStreet() : Streets.Preflop);

	public static HandStrengthValueExpression Default = new HandStrengthValueExpression();

	public HandStrengthValueExpression(long flopHandValueMask, long turnHandValueMask, long riverHandValueMask, long flopBoardMask, long turnBoardMask, long riverBoardMask, PreflopRangeHoldemCompact preflopRangeHoldem, OmahaPreflopRange omahaPreflopRange, Poker.Calc.Range omahaPreflopTopRange)
	{
		FlopHandValueMask = flopHandValueMask;
		TurnHandValueMask = turnHandValueMask;
		RiverHandValueMask = riverHandValueMask;
		FlopBoardMask = flopBoardMask;
		TurnBoardMask = turnBoardMask;
		RiverBoardMask = riverBoardMask;
		PreflopRangeHoldem = preflopRangeHoldem;
		OmahaPreflopRange = omahaPreflopRange;
		OmahaPreflopTopRange = omahaPreflopTopRange;
		if (OmahaPreflopTopRange.IsZeroToZero)
		{
			OmahaPreflopTopRange = Poker.Calc.Range.ZeroToHundred;
		}
	}

	public static HandStrengthValueExpression Create(RelevantHandValue flopHandValue, RelevantHandValue turnHandValue, RelevantHandValue riverHandValue, RelevantFlopType flopBoard, RelevantTurnType turnBoard, RelevantRiverType riverBoard, PreflopRangeHoldemCompact preflopRange, OmahaPreflopRange omahaPreflopRange, Poker.Calc.Range omahaPreflopTopRange)
	{
		HandStrengthValueExpression handStrengthValueExpression = ObjectPool<HandStrengthValueExpression>.ThreadShared.RentObject();
		handStrengthValueExpression.FlopHandValueMask = (long)flopHandValue;
		handStrengthValueExpression.TurnHandValueMask = (long)turnHandValue;
		handStrengthValueExpression.RiverHandValueMask = (long)riverHandValue;
		handStrengthValueExpression.FlopBoardMask = (long)flopBoard;
		handStrengthValueExpression.TurnBoardMask = (long)turnBoard;
		handStrengthValueExpression.RiverBoardMask = (long)riverBoard;
		handStrengthValueExpression.PreflopRangeHoldem = preflopRange;
		handStrengthValueExpression.OmahaPreflopRange = omahaPreflopRange;
		handStrengthValueExpression.OmahaPreflopTopRange = omahaPreflopTopRange;
		return handStrengthValueExpression;
	}

	public HandStrengthValueExpression()
	{
		PreflopRangeHoldem = PreflopRangeHoldemCompact.Empty;
		OmahaPreflopTopRange = Poker.Calc.Range.ZeroToHundred;
	}

	public static HandStrengthValueExpression Create(RelevantHandValues handValue, RelevantHandBoards handBoards, PreflopRangeHoldemCompact preflopRange, OmahaPreflopRange omahaPreflopRange, Poker.Calc.Range omahaPreflopTopRange)
	{
		return Create(handValue.Flop, handValue.Turn, handValue.River, handBoards.Flop, handBoards.Turn, handBoards.River, preflopRange, omahaPreflopRange, omahaPreflopTopRange);
	}

	public Streets GetLastHandValueStreet()
	{
		if (!HasRiverHandValue)
		{
			if (!HasTurnHandValue)
			{
				if (!HasFlopHandValue)
				{
					if (!HasPreflopRange)
					{
						throw new InvalidOperationException("Failed to get last hand value street because no hand value is filtered");
					}
					return Streets.Preflop;
				}
				return Streets.Flop;
			}
			return Streets.Turn;
		}
		return Streets.River;
	}

	public Streets GetLastHandBoardStreet()
	{
		if (!HasRiverBoard)
		{
			if (!HasTurnBoard)
			{
				if (!HasFlopBoard)
				{
					throw new InvalidOperationException("Failed to get last hand value street because no hand value is filtered");
				}
				return Streets.Flop;
			}
			return Streets.Turn;
		}
		return Streets.River;
	}

	public bool Evaluate(RelevantHandBoards boards)
	{
		if ((FlopBoardMask == 0L || (long)((ulong)FlopBoardMask & (ulong)boards.Flop) > 0L) && (TurnBoardMask == 0L || (long)((ulong)TurnBoardMask & (ulong)boards.Turn) > 0L))
		{
			if (RiverBoardMask != 0L)
			{
				return (long)((ulong)RiverBoardMask & (ulong)boards.River) > 0L;
			}
			return true;
		}
		return false;
	}

	public bool Evaluate(HandStrengthValueExpression hand)
	{
		if ((FlopHandValueMask == 0L || (FlopHandValueMask & hand.FlopHandValueMask) > 0) && (TurnHandValueMask == 0L || (TurnHandValueMask & hand.TurnHandValueMask) > 0) && (RiverHandValueMask == 0L || (RiverHandValueMask & hand.RiverHandValueMask) > 0) && (FlopBoardMask == 0L || (FlopBoardMask & hand.FlopBoardMask) > 0) && (TurnBoardMask == 0L || (TurnBoardMask & hand.TurnBoardMask) > 0) && (RiverBoardMask == 0L || (RiverBoardMask & hand.RiverBoardMask) > 0) && (PreflopRangeHoldem.IsEmpty || PreflopRangeHoldem.Intersects(hand.PreflopRangeHoldem)) && (OmahaPreflopRange.IsEmpty || OmahaPreflopRange.Intersects(hand.OmahaPreflopRange)))
		{
			if (!OmahaPreflopTopRange.ContainsZeroToHundred)
			{
				return OmahaPreflopTopRange.Contains(hand.OmahaPreflopTopRange);
			}
			return true;
		}
		return false;
	}

	public bool Equals(HandStrengthValueExpression other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		if (FlopHandValueMask == other.FlopHandValueMask && TurnHandValueMask == other.TurnHandValueMask && RiverHandValueMask == other.RiverHandValueMask && FlopBoardMask == other.FlopBoardMask && TurnBoardMask == other.TurnBoardMask && RiverBoardMask == other.RiverBoardMask && PreflopRangeHoldem.Equals(other.PreflopRangeHoldem) && OmahaPreflopRange.Equals(other.OmahaPreflopRange))
		{
			return OmahaPreflopTopRange.Equals(other.OmahaPreflopTopRange);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(FlopHandValueMask, TurnHandValueMask, RiverHandValueMask, FlopBoardMask, TurnBoardMask, RiverBoardMask, HashCode.Combine(PreflopRangeHoldem, OmahaPreflopTopRange, OmahaPreflopRange));
	}

	public override string ToString()
	{
		string text = string.Empty;
		if (HasFlopHandValue)
		{
			text = text + "Flop: " + FlopHandValue;
		}
		if (HasFlopBoard)
		{
			text = text + " Flop Board: " + FlopBoard;
		}
		if (HasTurnHandValue)
		{
			text = text + " Turn: " + TurnHandValue;
		}
		if (HasTurnBoard)
		{
			text = text + " Turn Board: " + TurnBoard;
		}
		if (HasRiverHandValue)
		{
			text = text + " River: " + RiverHandValue;
		}
		if (HasRiverBoard)
		{
			text = text + " River Board: " + RiverBoard;
		}
		if (HasHoldemPreflopRange)
		{
			text = text.AddWord($"Preflop Range: {PreflopRangeHoldem}");
		}
		if (HasOmahaPreflopRange)
		{
			if (!OmahaPreflopTopRange.ContainsZeroToHundred)
			{
				text = text.AddWord($"Top: {OmahaPreflopTopRange}");
			}
			text = text.AddWord($"Preflop Range: {OmahaPreflopRange}");
		}
		return text;
	}
}
