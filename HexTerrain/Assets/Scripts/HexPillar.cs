using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexPillar : MonoBehaviour
{
    public HexTerrain terrain { get; private set; }
    public HexGrid.Coord coord { get; private set; }

    public HexPillarEnd topEnd;
    public HexPillarEnd bottomEnd;

    public bool drawTopEnd = true;
    public Material topMaterial;

    public bool drawBottomEnd = true;
    public Material bottomMaterial;

    public Material[] wallMaterials = new Material[(int)HexEdgeDirection.MAX];
    public float sideTextureHeight = 1f;

    public void Init(HexTerrain terrain, HexGrid.Coord coord)
    {
        this.terrain = terrain;
        this.coord = coord;

        Undo.undoRedoPerformed += OnUndoRedo;
        
        GameObject topEndGameObject = new GameObject("Top End", typeof(HexPillarEnd));
        topEndGameObject.transform.SetParent(transform);
        topEnd = topEndGameObject.GetComponent<HexPillarEnd>();
        topEnd.Init(this, true);
        topEnd.UpdatePosition();

        GameObject bottomEndGameObject = new GameObject("Bottom End", typeof(HexPillarEnd));
        bottomEndGameObject.transform.SetParent(transform);
        bottomEnd = bottomEndGameObject.GetComponent<HexPillarEnd>();
        bottomEnd.Init(this, false);
        bottomEnd.UpdatePosition();
    }

    public void Constrain(float minHeight, float maxHeight, HexPillar pillarAbove, HexPillar pillarBelow)
    {
        topEnd.centerHeight = Mathf.Clamp(topEnd.centerHeight, bottomEnd.centerHeight, maxHeight);
        bottomEnd.centerHeight = Mathf.Clamp(bottomEnd.centerHeight, minHeight, topEnd.centerHeight);

        if (pillarAbove)
        {
            topEnd.centerHeight = Mathf.Min(topEnd.centerHeight, pillarAbove.bottomEnd.centerHeight);
        }

        if (pillarBelow)
        {
            bottomEnd.centerHeight = Mathf.Max(bottomEnd.centerHeight, pillarBelow.topEnd.centerHeight);
        }

        for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
        {
            topEnd.corners[(int)direction].height = Mathf.Clamp(topEnd.corners[(int)direction].height, bottomEnd.corners[(int)direction].height, maxHeight);
            bottomEnd.corners[(int)direction].height = Mathf.Clamp(bottomEnd.corners[(int)direction].height, minHeight, topEnd.corners[(int)direction].height);

            if (pillarAbove)
            {
                topEnd.corners[(int)direction].height = Mathf.Min(topEnd.corners[(int)direction].height, pillarAbove.bottomEnd.corners[(int)direction].height);
            }

            if (pillarBelow)
            {
                bottomEnd.corners[(int)direction].height = Mathf.Max(bottomEnd.corners[(int)direction].height, pillarBelow.topEnd.corners[(int)direction].height);
            }
        }
    }

    public void GenerateMesh()
    {
        // Place logical constraints on the geometry before creating a mesh based on it
        int pillarIndex = terrain.pillarGrid[coord].IndexOf(this);
        HexPillar pillarAbove = pillarIndex + 1 < terrain.pillarGrid[coord].Count ? terrain.pillarGrid[coord][pillarIndex + 1] : null;
        HexPillar pillarBelow = pillarIndex - 1 >= 0 ? terrain.pillarGrid[coord][pillarIndex - 1] : null;
        Constrain(terrain.minHeight, terrain.maxHeight, pillarAbove, pillarBelow);

        // Collect the various materials into one list
        List<Material> allMaterials = new List<Material>();

        if (topMaterial)
            allMaterials.Add(topMaterial);

        if (bottomMaterial)
            allMaterials.Add(bottomMaterial);

        foreach (Material mat in wallMaterials)
        {
            if (mat)
                allMaterials.Add(mat);
        }

        // Begin setting up the meshes.
        Mesh mesh = new Mesh();
        mesh.name = "Pillar Mesh";

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().materials = allMaterials.ToArray();
        mesh.subMeshCount = allMaterials.Count;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        Dictionary<Material, List<int>> trianglesPerMat = new Dictionary<Material, List<int>>();

        for (int iMat = 0; iMat < allMaterials.Count; ++iMat)
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
        if (drawTopEnd && topMaterial)
        {
            vertices.Add(new Vector3(0, topEnd.centerHeight, 0));
            for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirectionVector(direction);
                cornerPos += new Vector3(0, topEnd.corners[(int)direction].height, 0);
                vertices.Add(cornerPos);
            }
        }
        
        if (drawBottomEnd && bottomMaterial)
        {
            vertices.Add(new Vector3(0, bottomEnd.centerHeight, 0));
            for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirectionVector(direction);
                cornerPos += new Vector3(0, bottomEnd.corners[(int)direction].height, 0);
                vertices.Add(cornerPos);
            }
        }

        // Calculate UVs...
        if (drawTopEnd && topMaterial)
        {
            uvs.Add(new Vector2(0.5f, 0.5f));
            for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
            {
                Vector3 directionVector = HexHelper.GetCornerDirectionVector(direction);
                uvs.Add(new Vector2(0.5f + directionVector.x / 2, 0.5f + directionVector.z / 2));
            }
        }

        if (drawBottomEnd && bottomMaterial)
        {
            uvs.Add(new Vector2(0.5f, 0.5f));
            for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
            {
                Vector3 directionVector = HexHelper.GetCornerDirectionVector(direction);
                uvs.Add(new Vector2(0.5f - directionVector.x / 2, 0.5f + directionVector.z / 2));
            }
        }

        // Calculate triangles...
        if (drawTopEnd && topMaterial)
        {
            List<int> triangles = trianglesPerMat[topMaterial];
            const int centerVertIndex = 0;
            for (int iTri = 0; iTri < (int)HexEdgeDirection.MAX; ++iTri)
            {
                triangles.Add(centerVertIndex);

                triangles.Add(centerVertIndex + (iTri + 1));
                triangles.Add(centerVertIndex + (iTri + 2 <= (int)HexEdgeDirection.MAX ? iTri + 2 : iTri - 4));
            }
        }

        if (drawBottomEnd && bottomMaterial)
        {
            List<int> triangles = trianglesPerMat[bottomMaterial];
            const int centerVertIndex = 7;
            for (int iTri = 0; iTri < (int)HexEdgeDirection.MAX; ++iTri)
            {
                triangles.Add(centerVertIndex);

                triangles.Add(centerVertIndex + (iTri + 2 <= (int)HexEdgeDirection.MAX ? iTri + 2 : iTri - 4));
                triangles.Add(centerVertIndex + (iTri + 1));
            }
        }

        /*
        * Generate the sides connecting the top to the bottom.
        */
        for (HexEdgeDirection edge = 0; edge < HexEdgeDirection.MAX; ++edge)
        {
            // Choose a material for this surface...
            Material mat = wallMaterials[Random.Range(0, wallMaterials.Length)];

            if (!mat)
                continue;

            // Calculate vertices and UVs...
            HexCornerDirection[] corners = HexHelper.GetCornerDirectionsNextToEdge(edge);

            for (int iCorner = 0; iCorner < corners.Length; ++iCorner)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirectionVector(corners[iCorner]);

                cornerPos.y = topEnd.corners[(int)corners[iCorner]].height;
                vertices.Add(cornerPos);
                uvs.Add(new Vector2(1 - iCorner, cornerPos.y / sideTextureHeight));

                cornerPos.y = bottomEnd.corners[(int)corners[iCorner]].height;
                vertices.Add(cornerPos);
                uvs.Add(new Vector2(1 - iCorner, cornerPos.y / sideTextureHeight));
            }

            // Calculate triangles...
            List<int> triangles = trianglesPerMat[mat];
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

        for (int iMat = 0; iMat < allMaterials.Count; ++iMat)
        {
            mesh.SetTriangles(trianglesPerMat[allMaterials[iMat]], iMat);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        // Update end and corner handles to match new model
        topEnd.UpdatePosition();
        bottomEnd.UpdatePosition();
    }

    void OnUndoRedo()
    {
        GenerateMesh();
    }
}