using System;
using BinarySerializer;

namespace Hand2NoteCore.HandStrength;

[BinarySerializable]
public readonly struct RelevantHandBoards : IEquatable<RelevantHandBoards>
{
	[Tag(1)]
	public long _flop { get; }

	[Tag(2)]
	public long _turn { get; } 

	[Tag(3)]
	public long _river { get; }

	public RelevantFlopType Flop => (RelevantFlopType)_flop;

	public RelevantTurnType Turn => (RelevantTurnType)_turn;

	public RelevantRiverType River => (RelevantRiverType)_river;

	public static RelevantHandBoards Default => new RelevantHandBoards((RelevantFlopType)0L, (RelevantTurnType)0uL, (RelevantRiverType)0uL);

	public bool HasFlop => _flop != 0;

	public bool HasTurn => _turn != 0;

	public bool HasRiver => _river != 0;

	public RelevantHandBoards(RelevantFlopType flop, RelevantTurnType turn, RelevantRiverType river)
	{
	}

	public bool Equals(RelevantHandBoards other)
	{
		if (_flop == other._flop && _turn == other._turn)
		{
			return _river == other._river;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is RelevantHandBoards other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_flop, _turn, _river);
	}
}
