using UnityEngine;
using System.Collections.Generic;

public class HexTerrainEditable : HexTerrain
{
    Dictionary<HexGrid.Coord, GameObject> hexObjects = new Dictionary<HexGrid.Coord, GameObject>();

    const int sidesPerHex = 6;

    void Start()
    {
        pillarGrid.Add(new HexGrid.Coord(0, 0), new List<Pillar>());
        pillarGrid[0, 0].Add(new Pillar());

        pillarGrid.Add(new HexGrid.Coord(1, 0), new List<Pillar>());
        pillarGrid[1, 0].Add(new Pillar());

        pillarGrid.Add(new HexGrid.Coord(1, -1), new List<Pillar>());
        pillarGrid[1, -1].Add(new Pillar());

        pillarGrid.Add(new HexGrid.Coord(0, 1), new List<Pillar>());
        pillarGrid[0, 1].Add(new Pillar());

        pillarGrid.Add(new HexGrid.Coord(-1, 0), new List<Pillar>());
        pillarGrid[-1, 0].Add(new Pillar());

        pillarGrid.Add(new HexGrid.Coord(-1, 1), new List<Pillar>());
        pillarGrid[-1, 1].Add(new Pillar());

        pillarGrid.Add(new HexGrid.Coord(0, -1), new List<Pillar>());
        pillarGrid[0, -1].Add(new Pillar());

        GenerateAllPillarMeshes();
    }

    public void GenerateMeshForPillar(Pillar pillar)
    {
        // Find out at which coordinates the tile exists within our tile grid.
        HexGrid.Coord coord;
        if (!TryGetCoordForPillar(pillar, out coord))
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

            hexObjects.Add(coord, hexObject);
        }
        else
        {
            hexObject = hexObjects[coord];
        }

        // Collect the various materials into one array
        Material[] allMaterials = new Material[floorMaterials.Length + ceilingMaterials.Length + wallMaterials.Length];
        floorMaterials.CopyTo(allMaterials, 0);
        ceilingMaterials.CopyTo(allMaterials, floorMaterials.Length);
        wallMaterials.CopyTo(allMaterials, floorMaterials.Length + ceilingMaterials.Length);

        // Begin setting up the meshes.
        Mesh mesh = new Mesh();
        mesh.name = "Pillar Mesh";

        hexObject.GetComponent<MeshFilter>().mesh = mesh;
        hexObject.GetComponent<MeshRenderer>().materials = allMaterials;
        mesh.subMeshCount = allMaterials.Length;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        Dictionary<Material, List<int>> trianglesPerMat = new Dictionary<Material, List<int>>();

        for (int iMat = 0; iMat < allMaterials.Length; ++iMat)
        {
            if (!trianglesPerMat.ContainsKey(allMaterials[iMat]))
            {
                trianglesPerMat.Add(allMaterials[iMat], new List<int>());
            }
        }

        /*
        * Generate the surface of the top and bottom.
        */

        // Calculate vertices...
        if (pillar.drawTopEnd)
        {
            vertices.Add(new Vector3(0, pillar.topEnd.centerHeight, 0));
            for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirection(corner);
                cornerPos += new Vector3(0, pillar.topEnd.cornerHeights[(int)corner], 0);
                vertices.Add(cornerPos);
            }
        }


        if (pillar.drawLowEnd)
        {
            vertices.Add(new Vector3(0, pillar.lowEnd.centerHeight, 0));
            for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirection(corner);
                cornerPos += new Vector3(0, pillar.lowEnd.cornerHeights[(int)corner], 0);
                vertices.Add(cornerPos);
            }
        }

        // Calculate UVs...
        if (pillar.drawTopEnd)
        {
            uvs.Add(new Vector2(0.5f, 0.5f));
            for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
            {
                Vector3 direction = HexHelper.GetCornerDirection(corner);
                uvs.Add(new Vector2(0.5f + direction.x / 2, 0.5f + direction.z / 2));
            }
        }

        if (pillar.drawLowEnd)
        {
            uvs.Add(new Vector2(0.5f, 0.5f));
            for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
            {
                Vector3 direction = HexHelper.GetCornerDirection(corner);
                uvs.Add(new Vector2(0.5f - direction.x / 2, 0.5f + direction.z / 2));
            }
        }

        // Calculate triangles...
        if (pillar.drawTopEnd)
        {
            Material mat = floorMaterials[Random.Range(0, floorMaterials.Length)];

            List<int> triangles = trianglesPerMat[mat];
            const int centerVertIndex = 0;
            for (int iTri = 0; iTri < sidesPerHex; ++iTri)
            {
                triangles.Add(centerVertIndex);

                triangles.Add(centerVertIndex + (iTri + 1));
                triangles.Add(centerVertIndex + (iTri + 2 <= sidesPerHex ? iTri + 2 : iTri - 4));
            }
        }

        if (pillar.drawLowEnd)
        {
            Material mat = ceilingMaterials[Random.Range(0, ceilingMaterials.Length)];

            List<int> triangles = trianglesPerMat[mat];
            const int centerVertIndex = 7;
            for (int iTri = 0; iTri < sidesPerHex; ++iTri)
            {
                triangles.Add(centerVertIndex);

                triangles.Add(centerVertIndex + (iTri + 2 <= sidesPerHex ? iTri + 2 : iTri - 4));
                triangles.Add(centerVertIndex + (iTri + 1));
            }
        }

        /*
        * Generate the sides connecting the top to the bottom.
        */
        for (HexEdge edge = 0; edge < HexEdge.MAX; ++edge)
        {
            // Choose a material for this surface...
            Material layerMat = wallMaterials[Random.Range(0, wallMaterials.Length)];

            // Calculate vertices and UVs...
            HexCorner[] corners = HexHelper.GetNeighbouringCorners(edge);

            for (int iCorner = 0; iCorner < corners.Length; ++iCorner)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirection(corners[iCorner]);

                cornerPos.y = pillar.topEnd.cornerHeights[(int)corners[iCorner]];
                vertices.Add(cornerPos);
                uvs.Add(new Vector2(1 - iCorner, cornerPos.y / wallTextureHeight));

                cornerPos.y = pillar.lowEnd.cornerHeights[(int)corners[iCorner]];
                vertices.Add(cornerPos);
                uvs.Add(new Vector2(1 - iCorner, cornerPos.y / wallTextureHeight));
            }

            // Calculate triangles...
            List<int> triangles = trianglesPerMat[layerMat];
            triangles.Add(vertices.Count - 4); // Top-right of quad
            triangles.Add(vertices.Count - 3); // Low-right of quad
            triangles.Add(vertices.Count - 2); // Top-left of quad

            triangles.Add(vertices.Count - 2); // Top-left of quad
            triangles.Add(vertices.Count - 3); // Low-right of quad
            triangles.Add(vertices.Count - 1); // Low-left of quad
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

    void GenerateAllPillarMeshes()
    {
        foreach (List<Pillar> pillarSet in pillarGrid)
        {
            foreach (Pillar pillar in pillarSet)
            {
                GenerateMeshForPillar(pillar);
            }
        }
    }

    bool TryGetCoordForPillar(Pillar pillar, out HexGrid.Coord coord)
    {
        foreach (List<Pillar> pillarsInHex in pillarGrid)
        {
            if (pillarsInHex.Contains(pillar))
            {
                if (pillarGrid.TryGetCoordForItem(pillarsInHex, out coord))
                    return true;
                else
                    break;
            }
        }

        coord = new HexGrid.Coord();
        return false;
    }
}
