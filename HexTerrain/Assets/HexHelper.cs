public static class HexHelper
{
	public static HexCorner GetOppositeCorner(HexCorner corner)
	{
		HexCorner opposite = (HexCorner)((int)corner + (int)HexCorner.MAX/2);

		if (opposite >= HexCorner.MAX)
		{
			opposite = (HexCorner)((int)opposite - (int)HexCorner.MAX);
		}

		return opposite;
	}
}