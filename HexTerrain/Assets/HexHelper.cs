using UnityEngine;

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

    public static Vector3 GetCornerDirection(HexCorner corner)
    {
        float rotation = ((float)corner / (float)HexCorner.MAX) * Mathf.PI * 2;
        return new Vector3(Mathf.Cos(rotation), 0f, -Mathf.Sin(rotation));
    }

    public static HexCorner[] GetNeighbouringCorners(HexEdge edge)
    {
        switch (edge)
        {
            case HexEdge.SouthEast:
                return new HexCorner[] { HexCorner.East, HexCorner.SouthEast };

            case HexEdge.South:
                return new HexCorner[] { HexCorner.SouthEast, HexCorner.SouthWest };

            case HexEdge.SouthWest:
                return new HexCorner[] { HexCorner.SouthWest, HexCorner.West };

            case HexEdge.NorthWest:
                return new HexCorner[] { HexCorner.West, HexCorner.NorthWest };

            case HexEdge.North:
                return new HexCorner[] { HexCorner.NorthWest, HexCorner.NorthEast };

            case HexEdge.NorthEast:
                return new HexCorner[] { HexCorner.NorthEast, HexCorner.East };

            default:
                return new HexCorner[] { };
        }
    }

    public static Vector3 GetEdgeCenterOffset(HexEdge edge)
    {
        HexCorner[] corners = GetNeighbouringCorners(edge);

        if (corners.Length != 2)
        {
            Debug.LogErrorFormat("Invalid edge \'{0}\'.", edge);
            return Vector3.zero;
        }

        return (GetCornerDirection(corners[0]) + GetCornerDirection(corners[1])) / 2;
    }
}