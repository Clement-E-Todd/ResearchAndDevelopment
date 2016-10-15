using UnityEngine;
using System.Collections.Generic;

public abstract class HexTerrain : MonoBehaviour
{
    public float hexRadius = 1f;
	public float wallTextureHeight = 1f;

    public Material[] floorMaterials;
    public Material[] ceilingMaterials;
    public Material[] wallMaterials;

    public class Pillar
    {
        public class End
        {
            public float centerHeight;
            public float centerSplitHeight = 0f;

            public float[] cornerHeights = new float[(int)HexCorner.MAX];
            public bool[] cornerSplitHeights = new bool[(int)HexCorner.MAX];

            public End(float height)
            {
                centerHeight = height;

                for (int i = 0; i < cornerHeights.Length; ++i)
                {
                    cornerHeights[i] = height;
                }
            }
        }

        public End topEnd = new End(1f);
        public End lowEnd = new End(0f);

        public bool drawTopEnd = true;
        public bool drawLowEnd = true;
    }

    public HexTerrainPillarGrid pillarGrid = new HexTerrainPillarGrid();

    public Vector3 GetLocalPositionForCoord(HexGrid.Coord coord)
    {
        Vector3 xDirection = HexHelper.GetEdgeCenterOffset(HexEdge.SouthEast) * 2;
        Vector3 yDirection = HexHelper.GetEdgeCenterOffset(HexEdge.NorthEast) * 2;
        return (xDirection * coord.x) + (yDirection * coord.y);
    }
}