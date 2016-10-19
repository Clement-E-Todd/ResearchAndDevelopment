using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexPillarEditable : MonoBehaviour
{
    public HexTerrain owner { get; private set; }
    public HexPillarInfo pillarInfo { get; private set; }
    public HexGrid.Coord coord { get; private set; }

    public HexPillarCornerEditable[] topCornerObjects = new HexPillarCornerEditable[(int)HexCorner.MAX];
    public HexPillarCornerEditable[] bottomCornerObjects = new HexPillarCornerEditable[(int)HexCorner.MAX];

    public void Awake()
    {
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    public void Init(HexTerrain owner, HexPillarInfo pillarInfo, HexGrid.Coord coord)
    {
        this.owner = owner;
        this.pillarInfo = pillarInfo;
        this.coord = coord;

        for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
        {
            string cornerName = string.Format("Corner ({0}, {1})", corner.ToString(), "Top");
            GameObject topCornerGameObject = new GameObject(cornerName, typeof(HexPillarCornerEditable));
            topCornerGameObject.transform.SetParent(transform);
            topCornerObjects[(int)corner] = topCornerGameObject.GetComponent<HexPillarCornerEditable>();
            topCornerObjects[(int)corner].Init(this, corner, true);
            topCornerObjects[(int)corner].UpdatePosition();
        }

        for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
        {
            string cornerName = string.Format("Corner ({0}, {1})", corner.ToString(), "Bottom");
            GameObject bottomCornerGameObject = new GameObject(cornerName, typeof(HexPillarCornerEditable));
            bottomCornerGameObject.transform.SetParent(transform);
            bottomCornerObjects[(int)corner] = bottomCornerGameObject.GetComponent<HexPillarCornerEditable>();
            bottomCornerObjects[(int)corner].Init(this, corner, false);
            bottomCornerObjects[(int)corner].UpdatePosition();
        }
    }

    public void GenerateMesh()
    {
        // Place logical constraints on the geometry before creating a mesh based on it
        int pillarIndex = owner.pillarGrid[coord].IndexOf(pillarInfo);
        HexPillarInfo pillarAbove = pillarIndex + 1 < owner.pillarGrid[coord].Count ? owner.pillarGrid[coord][pillarIndex + 1] : null;
        HexPillarInfo pillarBelow = pillarIndex - 1 >= 0 ? owner.pillarGrid[coord][pillarIndex - 1] : null;
        pillarInfo.Constrain(owner.minHeight, owner.maxHeight, pillarAbove, pillarBelow);

        // Collect the various materials into one list
        List<Material> allMaterials = new List<Material>();

        if (pillarInfo.topMaterial)
            allMaterials.Add(pillarInfo.topMaterial);

        if (pillarInfo.bottomMaterial)
            allMaterials.Add(pillarInfo.bottomMaterial);

        foreach (Material mat in pillarInfo.wallMaterials)
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
        if (pillarInfo.drawTopEnd && pillarInfo.topMaterial)
        {
            vertices.Add(new Vector3(0, pillarInfo.topEnd.centerHeight, 0));
            for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirection(corner);
                cornerPos += new Vector3(0, pillarInfo.topEnd.cornerHeights[(int)corner], 0);
                vertices.Add(cornerPos);
            }
        }
        
        if (pillarInfo.drawBottomEnd && pillarInfo.bottomMaterial)
        {
            vertices.Add(new Vector3(0, pillarInfo.bottomEnd.centerHeight, 0));
            for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirection(corner);
                cornerPos += new Vector3(0, pillarInfo.bottomEnd.cornerHeights[(int)corner], 0);
                vertices.Add(cornerPos);
            }
        }

        // Calculate UVs...
        if (pillarInfo.drawTopEnd && pillarInfo.topMaterial)
        {
            uvs.Add(new Vector2(0.5f, 0.5f));
            for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
            {
                Vector3 direction = HexHelper.GetCornerDirection(corner);
                uvs.Add(new Vector2(0.5f + direction.x / 2, 0.5f + direction.z / 2));
            }
        }

        if (pillarInfo.drawBottomEnd && pillarInfo.bottomMaterial)
        {
            uvs.Add(new Vector2(0.5f, 0.5f));
            for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
            {
                Vector3 direction = HexHelper.GetCornerDirection(corner);
                uvs.Add(new Vector2(0.5f - direction.x / 2, 0.5f + direction.z / 2));
            }
        }

        // Calculate triangles...
        if (pillarInfo.drawTopEnd && pillarInfo.topMaterial)
        {
            List<int> triangles = trianglesPerMat[pillarInfo.topMaterial];
            const int centerVertIndex = 0;
            for (int iTri = 0; iTri < (int)HexEdge.MAX; ++iTri)
            {
                triangles.Add(centerVertIndex);

                triangles.Add(centerVertIndex + (iTri + 1));
                triangles.Add(centerVertIndex + (iTri + 2 <= (int)HexEdge.MAX ? iTri + 2 : iTri - 4));
            }
        }

        if (pillarInfo.drawBottomEnd && pillarInfo.bottomMaterial)
        {
            List<int> triangles = trianglesPerMat[pillarInfo.bottomMaterial];
            const int centerVertIndex = 7;
            for (int iTri = 0; iTri < (int)HexEdge.MAX; ++iTri)
            {
                triangles.Add(centerVertIndex);

                triangles.Add(centerVertIndex + (iTri + 2 <= (int)HexEdge.MAX ? iTri + 2 : iTri - 4));
                triangles.Add(centerVertIndex + (iTri + 1));
            }
        }

        /*
        * Generate the sides connecting the top to the bottom.
        */
        for (HexEdge edge = 0; edge < HexEdge.MAX; ++edge)
        {
            // Choose a material for this surface...
            Material mat = pillarInfo.wallMaterials[Random.Range(0, pillarInfo.wallMaterials.Length)];

            if (!mat)
                continue;

            // Calculate vertices and UVs...
            HexCorner[] corners = HexHelper.GetNeighbouringCorners(edge);

            for (int iCorner = 0; iCorner < corners.Length; ++iCorner)
            {
                Vector3 cornerPos = HexHelper.GetCornerDirection(corners[iCorner]);

                cornerPos.y = pillarInfo.topEnd.cornerHeights[(int)corners[iCorner]];
                vertices.Add(cornerPos);
                uvs.Add(new Vector2(1 - iCorner, cornerPos.y / pillarInfo.sideTextureHeight));

                cornerPos.y = pillarInfo.bottomEnd.cornerHeights[(int)corners[iCorner]];
                vertices.Add(cornerPos);
                uvs.Add(new Vector2(1 - iCorner, cornerPos.y / pillarInfo.sideTextureHeight));
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

        // Update corner handles to match new model
        for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
        {
            topCornerObjects[(int)corner].UpdatePosition();
            bottomCornerObjects[(int)corner].UpdatePosition();
        }
    }

    void OnUndoRedo()
    {
        GenerateMesh();
    }
}