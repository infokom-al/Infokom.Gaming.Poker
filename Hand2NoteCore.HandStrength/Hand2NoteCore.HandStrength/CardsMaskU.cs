using System.Runtime.InteropServices;

namespace Hand2NoteCore.HandStrength;

[StructLayout(LayoutKind.Explicit)]
internal struct CardsMaskU
{
	[FieldOffset(0)]
	public long CardsMask;

	[FieldOffset(0)]
	public ushort Spades;

	[FieldOffset(2)]
	public ushort Clubs;

	[FieldOffset(4)]
	public ushort Diamonds;

	[FieldOffset(6)]
	public ushort Hearts;
}
