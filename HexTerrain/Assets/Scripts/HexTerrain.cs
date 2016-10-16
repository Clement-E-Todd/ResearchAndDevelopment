using UnityEngine;
using System.Collections.Generic;

public abstract class HexTerrain : MonoBehaviour
{
    public float hexRadius = 1f;
    public float minHeight = 0f;
    public float maxHeight = 20f;

    public Material[] floorMaterials;
    public Material[] ceilingMaterials;
    public Material[] wallMaterials;

    public HexTerrainPillarGrid pillarGrid = new HexTerrainPillarGrid();

    public Vector3 GetLocalPositionForCoord(HexGrid.Coord coord)
    {
        Vector3 xDirection = HexHelper.GetEdgeCenterOffset(HexEdge.SouthEast) * 2;
        Vector3 yDirection = HexHelper.GetEdgeCenterOffset(HexEdge.NorthEast) * 2;
        return (xDirection * coord.x) + (yDirection * coord.y);
    }
}