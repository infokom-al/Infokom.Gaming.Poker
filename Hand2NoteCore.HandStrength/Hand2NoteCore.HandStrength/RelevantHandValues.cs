using System;
using BinarySerializer;
using Poker.Calc;

namespace Hand2NoteCore.HandStrength;

[BinarySerializable]
public readonly struct RelevantHandValues : IEquatable<RelevantHandValues>
{
	public static RelevantHandValues None = new RelevantHandValues((RelevantHandValue)0L, (RelevantHandValue)0L, (RelevantHandValue)0L);

	[Tag(1)]
	public long _flop { get; }

	[Tag(2)]
	public long _turn { get; }

	[Tag(3)]
	public long _river { get; } 

	public RelevantHandValue Flop => (RelevantHandValue)_flop;

	public RelevantHandValue Turn => (RelevantHandValue)_turn;

	public RelevantHandValue River => (RelevantHandValue)_river;

	public bool HasFlopValue => _flop != 0;

	public bool HasTurnValue => _turn != 0;

	public bool HasRiverValue => _river != 0;

	public RelevantHandValues(RelevantHandValue flop, RelevantHandValue turn, RelevantHandValue river)
	{
	}

	public RelevantHandValue GetStreetValue(Streets street)
	{
		return street switch
		{
			Streets.Flop => Flop, 
			Streets.Turn => Turn, 
			Streets.River => River, 
			_ => throw new InvalidOperationException($"Invalid street {street}"), 
		};
	}

	public bool Equals(RelevantHandValues other)
	{
		if (_flop == other._flop && _turn == other._turn)
		{
			return _river == other._river;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is RelevantHandValues other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_flop, _turn, _river);
	}

	public override string ToString()
	{
		return $"Flop {Flop}\nTurn {Turn}\nRiver {River}";
	}
}
