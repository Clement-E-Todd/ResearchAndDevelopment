using UnityEngine;
using UnityEditor;
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

    public HexGrid.Coord GetCoordForLocalPosition(Vector3 localPosition)
    {
        Vector3 xDirection = HexHelper.GetEdgeCenterOffset(HexEdge.SouthEast) * 2;
        Vector3 yDirection = HexHelper.GetEdgeCenterOffset(HexEdge.NorthEast) * 2;

        Ray xRay = new Ray(transform.up * Vector3.Dot(transform.up, localPosition), xDirection.normalized);
        Plane yPlane = new Plane(Vector3.Cross(yDirection.normalized, transform.up), localPosition);

        float xDistance = 0f;
        yPlane.Raycast(xRay, out xDistance);
        Vector3 interestionPoint = xRay.GetPoint(xDistance);

        Vector2 xyDistances = new Vector2(
            Vector3.Distance(xRay.origin, interestionPoint) / xDirection.magnitude,
            Vector3.Distance(interestionPoint, localPosition) / yDirection.magnitude);

        if (localPosition.x < 0)
            xyDistances.x *= -1f;

        if (localPosition.z < 0)
            xyDistances.y *= -1f;

        Debug.Log(xyDistances);

        return new HexGrid.Coord((int)Mathf.Round(xyDistances.x), (int)Mathf.Round(xyDistances.y));
    }

    public HexGrid.Coord GetCoordForWorldPosition(Vector3 worldPosition)
    {
        return GetCoordForLocalPosition(transform.InverseTransformPoint(worldPosition));
    }
}