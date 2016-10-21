using UnityEngine;

/*
 * A static class of handy functions meant to make working with hexigonal data easier.
 */
public static class HexHelper
{
    /*
     * Gets the hex corner direction pointing in the opposite direction of the one passed in.
     */
    public static HexCornerDirection GetOppositeCornerDirection(HexCornerDirection corner)
    {
        HexCornerDirection opposite = (HexCornerDirection)((int)corner + (int)HexCornerDirection.MAX/2);

        if (opposite >= HexCornerDirection.MAX)
        {
            opposite = (HexCornerDirection)((int)opposite - (int)HexCornerDirection.MAX);
        }

        return opposite;
    }

    /*
     * Gets a unit-length vector pointing in the direction specified.
     */
    public static Vector3 GetCornerDirectionVector(HexCornerDirection direction)
    {
        float rotation = ((float)direction / (float)HexCornerDirection.MAX) * Mathf.PI * 2;
        return new Vector3(Mathf.Cos(rotation), 0f, -Mathf.Sin(rotation));
    }

    /*
     * Gets a pair of hex corner direction enums representing either end of the specified edge.
     */
    public static HexCornerDirection[] GetCornerDirectionsNextToEdge(HexEdgeDirection edgeDirection)
    {
        switch (edgeDirection)
        {
            case HexEdgeDirection.SouthEast:
                return new HexCornerDirection[] { HexCornerDirection.East, HexCornerDirection.SouthEast };

            case HexEdgeDirection.South:
                return new HexCornerDirection[] { HexCornerDirection.SouthEast, HexCornerDirection.SouthWest };

            case HexEdgeDirection.SouthWest:
                return new HexCornerDirection[] { HexCornerDirection.SouthWest, HexCornerDirection.West };

            case HexEdgeDirection.NorthWest:
                return new HexCornerDirection[] { HexCornerDirection.West, HexCornerDirection.NorthWest };

            case HexEdgeDirection.North:
                return new HexCornerDirection[] { HexCornerDirection.NorthWest, HexCornerDirection.NorthEast };

            case HexEdgeDirection.NorthEast:
                return new HexCornerDirection[] { HexCornerDirection.NorthEast, HexCornerDirection.East };

            default:
                return new HexCornerDirection[] { };
        }
    }

    /*
     * Gets a vector representing the distance between the center of a hex and one of its edges. Note that
     * the returned vector is NOT necessarily unit length.
     */
    public static Vector3 GetEdgeCenterOffset(HexEdgeDirection edge, float hexRadius)
    {
        HexCornerDirection[] cornerDirections = GetCornerDirectionsNextToEdge(edge);

        if (cornerDirections.Length != 2)
        {
            Debug.LogErrorFormat("Invalid edge \'{0}\'.", edge);
            return Vector3.zero;
        }

        return ((GetCornerDirectionVector(cornerDirections[0]) + GetCornerDirectionVector(cornerDirections[1])) / 2) * hexRadius;
    }
}