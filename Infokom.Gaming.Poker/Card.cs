namespace Infokom.Gaming.Poker
{
	// $C = R \times S$
	public enum Card : sbyte
	{
		_2 = -14,
		S2 = _2 + 16, S3, S4, S5, S6, S7, S8, S9, ST, SJ, SQ, SK, SA,
		D2 = S2 + 16, D3, D4, D5, D6, D7, D8, D9, DT, DJ, DQ, DK, DA,
		C2 = D2 + 16, C3, C4, C5, C6, C7, C8, C9, CT, CJ, CQ, CK, CA,
		H2 = C2 + 16, H3, H4, H5, H6, H7, H8, H9, HT, HJ, HQ, HK, HA,
	}
}
