using UnityEngine;
using System.Collections.Generic;

public class HexTerrainRaw : HexTerrain
{
    Dictionary<HexGrid.Coord, GameObject> hexObjects = new Dictionary<HexGrid.Coord, GameObject>();

    const int sidesPerHex = 6;

	public Material[] floorMaterials;
	public Material[] ceilingMaterials;
	public Material[] wallMaterials;

	void Start()
    {
        tileGrid.Add(new HexGrid.Coord(), new Tile(true, 4.0f, 2.0f, 1.5f, 1.0f, 0.5f));
		
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.North),       new Tile(1.25f, 0));
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.NorthEast),   new Tile(1.5f, 0));
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.SouthEast),   new Tile(0.25f, 0));
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.South),       new Tile(0.5f, 0));
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.SouthWest),   new Tile(0.75f, 0));
        tileGrid.Add( HexGrid.GetNeighbourCoordOffset(HexEdge.NorthWest),   new Tile(1.0f, 0));

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

		// Collect the various materials into one array
		Material[] allMaterials = new Material[floorMaterials.Length + ceilingMaterials.Length + wallMaterials.Length];
		floorMaterials.CopyTo(allMaterials, 0);
		ceilingMaterials.CopyTo(allMaterials, floorMaterials.Length);
		wallMaterials.CopyTo(allMaterials, floorMaterials.Length + ceilingMaterials.Length);
		
		// Begin setting up the meshes.
		Mesh mesh = new Mesh();
		mesh.name = string.Format("Mesh for [{0}, {1}]", coord.x, coord.y);

		hexObject.GetComponent<MeshFilter>().mesh = mesh;
		hexObject.GetComponent<MeshRenderer>().materials = allMaterials;
		mesh.subMeshCount = allMaterials.Length;

		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		Dictionary<Material, List<int>> trianglesPerMat = new Dictionary<Material, List<int>>();

		for (int iMat = 0; iMat < allMaterials.Length; ++iMat)
		{
			trianglesPerMat.Add(allMaterials[iMat], new List<int>());
		}

        /*
        * First, generate the surface of each floor and ceiling.
        */
        for (int iLayer = 0; iLayer < tile.layers.Count; ++iLayer)
        {
            // Find out whether this layer is a floor or a ceiling
            bool isCeiling = (iLayer % 2 != 0) != tile.topLayerIsCeiling;

			// Choose a material for this surface...
			Material layerMat = null;
			if (isCeiling)
				layerMat = ceilingMaterials[Random.Range(0, ceilingMaterials.Length)];
			else
				layerMat = floorMaterials[Random.Range(0, floorMaterials.Length)];

			// Calculate vertices...
			vertices.Add(new Vector3(0, tile.layers[iLayer].centerHeight, 0));
            for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirection(corner);
                cornerPos += new Vector3(0, tile.layers[iLayer].cornerHeights[(int)corner], 0);
                vertices.Add(cornerPos);
            }

			// Calculate UVs...
			uvs.Add(new Vector2(0.5f, 0.5f));
			for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
			{
				Vector3 direction = HexHelper.GetCornerDirection(corner);
				uvs.Add(new Vector2(0.5f + direction.x / 2, 0.5f + direction.z / 2));
			}

			// Calculate triangles...
			List<int> triangles = trianglesPerMat[layerMat];
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
					// Choose a material for this surface...
					Material layerMat = wallMaterials[Random.Range(0, wallMaterials.Length)];

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
					uvs.Add(new Vector2(1, 1));
					uvs.Add(new Vector2(1, 0));
					uvs.Add(new Vector2(0, 1));
					uvs.Add(new Vector2(0, 0));

					// Calculate triangles...
					List<int> triangles = trianglesPerMat[layerMat];
					triangles.Add(vertices.Count - 4); // Top-right of quad
                    triangles.Add(vertices.Count - 3); // Low-right of quad
                    triangles.Add(vertices.Count - 2); // Top-left of quad

                    triangles.Add(vertices.Count - 2); // Top-left of quad
					triangles.Add(vertices.Count - 3); // Low-right of quad
					triangles.Add(vertices.Count - 1); // Low-left of quad
				}
            }
        }

		// Finalize the mesh.
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
		
		for (int iMat = 0; iMat < allMaterials.Length; ++iMat)
		{
			mesh.SetTriangles(trianglesPerMat[allMaterials[iMat]], iMat);
		}

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
 