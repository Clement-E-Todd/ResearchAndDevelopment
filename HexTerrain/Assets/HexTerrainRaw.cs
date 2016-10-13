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
        tileGrid.Add(new HexGrid.Coord(), new Tile(2.5f, 2.0f, 1.5f, 1.0f, 0.5f));

        /*
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.North),       new Tile(0.1f));
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.NorthEast),   new Tile(0.2f));
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.SouthEast),   new Tile(0.3f));
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.South),       new Tile(0.4f));
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.SouthWest),   new Tile(0.5f));
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.NorthWest),   new Tile(0.6f));
        */

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

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        /*
        * First, generate the surface of each floor and ceiling.
        */
        for (int iLayer = 0; iLayer < tile.layers.Count; ++iLayer)
        {
            // Find out whether this layer is a floor or a ceiling
            bool isCeiling = (iLayer % 2 != 0) != tile.topLayerIsCeiling;

            // Calculate vertices...
            vertices.Add(new Vector3(0, tile.layers[iLayer].centerHeight, 0));
            for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirection(corner);
                cornerPos += new Vector3(0, tile.layers[iLayer].cornerHeights[(int)corner], 0);
                vertices.Add(cornerPos);
            }

            // TODO: Calculate UVs...

            // Calculate triangles...
            int centerVertIndex = iLayer * 7;
            for (int iTri = 0; iTri < sidesPerHex; ++iTri)
            {
                triangles.Add(centerVertIndex);

                if (isCeiling)
                {
                    triangles.Add(centerVertIndex + (iTri + 2 <= sidesPerHex ? iTri + 2 : iTri - 4));
                    triangles.Add(centerVertIndex + (iTri + 1));
                }
                else
                {
                    triangles.Add(centerVertIndex + (iTri + 1));
                    triangles.Add(centerVertIndex + (iTri + 2 <= sidesPerHex ? iTri + 2 : iTri - 4));
                }
            }
        }

        /*
        * Next, generate the sides connecting each floor to the ceiling below it.
        */
        for (int iLayer = 0; iLayer < tile.layers.Count; ++iLayer)
        {
            bool isCeiling = (iLayer % 2 != 0) != tile.topLayerIsCeiling;

            if (!isCeiling && iLayer + 1 < tile.layers.Count)
            {
                for (HexEdge edge = 0; edge < HexEdge.MAX; ++edge)
                {
                    // Calculate vertices...
                    HexCorner[] corners = HexHelper.GetNeighbouringCorners(edge);

                    for (int iCorner = 0; iCorner < corners.Length; ++iCorner)
                    {
                        Vector3 cornerPos = HexHelper.GetCornerDirection(corners[iCorner]);

                        cornerPos.y = tile.layers[iLayer].cornerHeights[(int)corners[iCorner]];
                        vertices.Add(cornerPos);
                        
                        cornerPos.y = tile.layers[iLayer+1].cornerHeights[(int)corners[iCorner]];
                        vertices.Add(cornerPos);
                    }

                    // TODO: Calculate UVs...

                    // TODO: Calculate triangles...
                    triangles.Add(vertices.Count - 4);
                    triangles.Add(vertices.Count - 3);
                    triangles.Add(vertices.Count - 2);

                    triangles.Add(vertices.Count - 2);
                    triangles.Add(vertices.Count - 3);
                    triangles.Add(vertices.Count - 1);
                }
            }
        }

        // Finalize the mesh.
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

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
 