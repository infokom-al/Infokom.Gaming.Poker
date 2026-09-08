using BinarySerializer;

namespace Poker.Calc;

[TypeTag(1, typeof(Board))]
[TypeTag(2, typeof(MultiBoard))]
public interface IBoard
{
	Streets Street { get; }
}
