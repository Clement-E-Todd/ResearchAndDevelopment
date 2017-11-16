namespace HexTerrain
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    /**
     * A static class of handy functions meant to make working with hexigonal data easier.
     */
    public static class HexHelper
    {
        /**
         * Gets the hex corner direction pointing in the opposite direction of the one passed in.
         */
        public static HexCornerDirection GetOppositeCornerDirection(HexCornerDirection corner)
        {
            HexCornerDirection opposite = (HexCornerDirection)((int)corner + (int)HexCornerDirection.MAX / 2);

            if (opposite >= HexCornerDirection.MAX)
            {
                opposite = (HexCornerDirection)((int)opposite - (int)HexCornerDirection.MAX);
            }

            return opposite;
        }

        /**
         * Gets a unit-length vector pointing in the direction specified.
         */
        public static Vector3 GetCornerDirectionVector(HexCornerDirection direction)
        {
            float rotation = ((float)direction / (float)HexCornerDirection.MAX) * Mathf.PI * 2;
            return new Vector3(Mathf.Cos(rotation), 0f, -Mathf.Sin(rotation));
        }

        /**
         * Gets a corner direction enum representing an end of the specified edge.
         */
        public static HexCornerDirection GetCornerDirectionNextToEdge(HexEdgeDirection edgeDirection, bool clockwise)
        {
            if (clockwise)
                return (edgeDirection + 1 < HexEdgeDirection.MAX) ? (HexCornerDirection)edgeDirection + 1 : 0;
            else
                return (HexCornerDirection)edgeDirection;
        }

        /**
         * Gets a corner direction enum representing a corner's neighbouring corner.
         */
        public static HexCornerDirection GetCornerDirectionNextToCorner(HexCornerDirection cornerDirection, bool clockwise)
        {
            if (clockwise)
                return (cornerDirection + 1 < HexCornerDirection.MAX) ? (HexCornerDirection)cornerDirection + 1 : 0;
            else
                return (cornerDirection - 1 > HexCornerDirection.INVALID) ? (HexCornerDirection)cornerDirection - 1 : HexCornerDirection.MAX - 1;
        }

        /**
         * Gets an edge direction enum representing a edge's neighbouring edge.
         */
        public static HexEdgeDirection GetEdgeDirectionNextToEdge(HexEdgeDirection edgeDirection, bool clockwise)
        {
            if (clockwise)
                return (edgeDirection + 1 < HexEdgeDirection.MAX) ? (HexEdgeDirection)edgeDirection + 1 : 0;
            else
                return (edgeDirection - 1 > HexEdgeDirection.INVALID) ? (HexEdgeDirection)edgeDirection - 1 : HexEdgeDirection.MAX - 1;
        }

        /**
         * Gets an edge direction enum representing an edge connected to the specified corner.
         */
        public static HexEdgeDirection GetEdgeDirectionNextToCorner(HexCornerDirection cornerDirection, bool clockwise)
        {
            if (clockwise)
                return (HexEdgeDirection)cornerDirection;
            else
                return (cornerDirection - 1 > HexCornerDirection.INVALID) ? (HexEdgeDirection)cornerDirection - 1 : 0;
        }

        /**
         * Gets a vector representing the distance between the center of a hex and one of its edges. Note that
         * the returned vector is NOT necessarily unit length.
         */
        public static Vector3 GetEdgeCenterOffset(HexEdgeDirection edge, float hexRadius)
        {
            if (edge == HexEdgeDirection.INVALID || edge == HexEdgeDirection.MAX)
            {
                Debug.LogErrorFormat("Invalid edge \'{0}\'.", edge);
                return Vector3.zero;
            }

            HexCornerDirection counterDirection = GetCornerDirectionNextToEdge(edge, false);
            HexCornerDirection clockwiseDirection = GetCornerDirectionNextToEdge(edge, true);

            return ((GetCornerDirectionVector(counterDirection) + GetCornerDirectionVector(clockwiseDirection)) / 2) * hexRadius;
        }

        /**
         * Gets all corners that occupy the same space as the specified corner from any pillar.
         */
        public static HexPillarCorner[] GetOverlappingCorners(HexPillarCorner corner)
        {
            List<HexPillarCorner> overlappingCorners = new List<HexPillarCorner>();

            // Get the coordinates at which we need to look for overlapping corners.
            HexGrid.Coord[] coords = new HexGrid.Coord[] {
            corner.end.pillar.coord,
            corner.end.pillar.coord + HexGrid.GetNeighbourCoordOffset(GetEdgeDirectionNextToCorner(corner.direction, false)),
            corner.end.pillar.coord + HexGrid.GetNeighbourCoordOffset(GetEdgeDirectionNextToCorner(corner.direction, true))
            };

            // Get the corner directions at which we need to look for overlaps at each coord.
            HexCornerDirection[] cornerDirections = new HexCornerDirection[] {
            corner.direction,
            (corner.direction + 2 < HexCornerDirection.MAX) ? (HexCornerDirection)(corner.direction + 2) : (HexCornerDirection)(corner.direction + 2 - (int)HexCornerDirection.MAX),
            (corner.direction - 2 > HexCornerDirection.INVALID) ? (HexCornerDirection)(corner.direction - 2) : (HexCornerDirection)(corner.direction - 2 + (int)HexCornerDirection.MAX)
        };

            // Iterate over each pillar that overlaps this corner to find corners with matching heights.
            for (int i = 0; i < 3; ++i)
            {
                if (!corner.GetTerrain().pillarGrid.ContainsItemAtCoord(coords[i]))
                    continue;

                foreach (HexPillar pillar in corner.GetTerrain().pillarGrid[coords[i]])
                {
                    if (pillar.topEnd.corners[(int)cornerDirections[i]].height == corner.height)
                        overlappingCorners.Add(pillar.topEnd.corners[(int)cornerDirections[i]]);

                    if (pillar.bottomEnd.corners[(int)cornerDirections[i]].height == corner.height)
                        overlappingCorners.Add(pillar.bottomEnd.corners[(int)cornerDirections[i]]);
                }
            }

            return overlappingCorners.ToArray();
        }
    }
}