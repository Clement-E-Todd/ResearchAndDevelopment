using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent (typeof(MeshFilter))]
public class HexTerrainRaw : HexTerrain
{
    Dictionary<HexGrid.Coord, GameObject> hexObjects = new Dictionary<HexGrid.Coord, GameObject>();

	const int sidesPerHex = 6;
	const int sidesPerTri = 3;

    void Start()
    {
        tileGrid.Add( new HexGrid.Coord(),                                  new Tile());
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.North),       new Tile());
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.NorthEast),   new Tile());
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.SouthEast),   new Tile());
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.South),       new Tile());
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.SouthWest),   new Tile());
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.NorthWest),   new Tile());

        GenerateAllHexMeshes();
	}
    
	void GenerateMeshForTile(HexTerrain.Tile tile)
	{
        // Find out at which coordinates the tile exists within our tile grid.
        HexGrid.Coord coord;
        if (!tileGrid.TryGetCoordForItem(tile, out coord))
        {
            return;
        }

        // If we don't already have a GameObject in the scene for this hex, create one now.
        GameObject hexObject = null;
        if (!hexObjects.ContainsKey(coord))
        {
            hexObject = new GameObject();
            hexObject.name = string.Format("Hex [{0}, {1}]", coord.x, coord.y);
            hexObject.transform.SetParent(transform);
            hexObject.transform.localPosition = GetLocalPositionForCoord(coord);

            hexObject.AddComponent<MeshFilter>();
            hexObject.AddComponent<MeshRenderer>();
        }

        // Begin setting up the mesh.
		Mesh mesh = new Mesh();
		mesh.name = string.Format("Hex Mesh [{0}, {1}]", coord.x, coord.y);
        hexObject.GetComponent<MeshFilter>().mesh = mesh;

        // Calculate where each vertex in the mesh belongs.
		List<Vector3> vertices = new List<Vector3>() { Vector3.zero };
		for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
		{
			vertices.Add(HexHelper.GetCornerDirection(corner));
		}
		mesh.vertices = vertices.ToArray();

        // TODO: Calculate UVs
		mesh.uv = new Vector2[mesh.vertices.Length];

        // Find all of the triangles in the mesh.
		List<int> triangles = new List<int>();
		for (int i = 0; i < sidesPerHex; ++i)
		{
			triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i + 2 <= sidesPerHex ? i + 2 : i - 4);
		}
		mesh.triangles = triangles.ToArray();

        // Finalize the mesh.
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

    void GenerateAllHexMeshes()
    {
        foreach (HexTerrain.Tile tile in tileGrid)
        {
            GenerateMeshForTile(tile);
        }
    }
}