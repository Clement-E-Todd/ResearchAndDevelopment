namespace HexTerrain
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class HexPillar : HexTerrainElement
    {
        public HexTerrain terrain;
        public HexGrid.Coord coord;
        public HexPillarEnd topEnd;
        public HexPillarEnd bottomEnd;
        
        public HexPillarMaterialBrush topMaterialBrush;
        Material topMaterial;
        short topMaterialRotation;
        public bool drawTopEnd = true;

        public HexPillarMaterialBrush sideMaterialBrush;
        Material[] sideMaterials;
        HexCornerDirection sideMaterialWrapCorner;
        public bool[] hideSides = new bool[(int)HexEdgeDirection.MAX];
        public float sideTextureHeight = 1f;

        public HexPillarMaterialBrush bottomMaterialBrush;
        Material bottomMaterial;
        short bottomMaterialRotation;
        public bool drawBottomEnd = true;

        List<int> topTriangles;
        List<int> sideTriangles;
        List<int> bottomTriangles;

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

        void OnDestroy()
        {
            topEnd.DestoryWithoutRecreating();
            bottomEnd.DestoryWithoutRecreating();

            terrain.pillarGrid[coord].Remove(this);
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
            if (!topMaterialBrush && drawTopEnd)
            {
                if (terrain.defaultFloorBrush)
                    topMaterialBrush = terrain.defaultFloorBrush;
                else
                    return;

                SelectTopMaterialFromBrush();
            }

            if (!sideMaterialBrush)
            {
                if (terrain.defaultSideBrush)
                    sideMaterialBrush = terrain.defaultSideBrush;
                else
                    return;

                SelectSideMaterialsFromBrush();
            }

            if (!bottomMaterialBrush && drawBottomEnd)
            {
                if (terrain.defaultCeilingBrush)
                    bottomMaterialBrush = terrain.defaultCeilingBrush;
                else
                    return;

                SelectBottomMaterialFromBrush();
            }

            // Place logical constraints on the geometry before creating a mesh based on it
            int pillarIndex = terrain.pillarGrid[coord].IndexOf(this);
            HexPillar pillarAbove = pillarIndex + 1 < terrain.pillarGrid[coord].Count ? terrain.pillarGrid[coord][pillarIndex + 1] : null;
            HexPillar pillarBelow = pillarIndex - 1 >= 0 ? terrain.pillarGrid[coord][pillarIndex - 1] : null;
            Constrain(terrain.minHeight, terrain.maxHeight, pillarAbove, pillarBelow);

            // Collect the various materials into one list
            List<Material> allMaterials = new List<Material>();

            allMaterials.Add(topMaterial);
            allMaterials.Add(bottomMaterial);
            foreach (Material material in sideMaterials)
            {
                if (material && !allMaterials.Contains(material))
                    allMaterials.Add(material);
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
            if (drawTopEnd && topMaterialBrush)
            {
                vertices.Add(new Vector3(0, topEnd.centerHeight, 0));
                for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
                {
                    Vector3 cornerPos = HexHelper.GetCornerDirectionVector(direction);
                    cornerPos += new Vector3(0, topEnd.corners[(int)direction].height, 0);
                    vertices.Add(cornerPos);
                }
            }

            if (drawBottomEnd && bottomMaterialBrush)
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
            if (drawTopEnd && topMaterialBrush)
            {
                uvs.Add(new Vector2(0.5f, 0.5f));
                for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
                {
                    HexCornerDirection realDirection = direction + topMaterialRotation;
                    if (realDirection >= HexCornerDirection.MAX)
                        realDirection -= HexCornerDirection.MAX;

                    Vector3 directionVector = HexHelper.GetCornerDirectionVector(realDirection);
                    uvs.Add(new Vector2(0.5f + directionVector.x / 2, 0.5f + directionVector.z / 2));
                }
            }

            if (drawBottomEnd && bottomMaterialBrush)
            {
                uvs.Add(new Vector2(0.5f, 0.5f));
                for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
                {
                    HexCornerDirection realDirection = direction + bottomMaterialRotation;
                    if (realDirection >= HexCornerDirection.MAX)
                        realDirection -= HexCornerDirection.MAX;

                    Vector3 directionVector = HexHelper.GetCornerDirectionVector(realDirection);
                    uvs.Add(new Vector2(0.5f - directionVector.x / 2, 0.5f + directionVector.z / 2));
                }
            }

            // Calculate triangles...
            int totalTriangles = 0;
            topTriangles = new List<int>();
            sideTriangles = new List<int>();
            bottomTriangles = new List<int>();

            if (drawTopEnd && topMaterial)
            {
                List<int> triangles = trianglesPerMat[topMaterial];
                const int centerVertIndex = 0;
                for (int iTri = 0; iTri < (int)HexEdgeDirection.MAX; ++iTri)
                {
                    if (hideSides[iTri])
                    {
                        continue;
                    }

                    triangles.Add(centerVertIndex);

                    triangles.Add(centerVertIndex + (iTri + 1));
                    triangles.Add(centerVertIndex + (iTri + 2 <= (int)HexEdgeDirection.MAX ? iTri + 2 : iTri - 4));

                    topTriangles.Add(totalTriangles++);
                }
            }

            if (drawBottomEnd && bottomMaterial)
            {
                List<int> triangles = trianglesPerMat[bottomMaterial];
                const int centerVertIndex = 7;
                for (int iTri = 0; iTri < (int)HexEdgeDirection.MAX; ++iTri)
                {
                    if (hideSides[iTri])
                    {
                        continue;
                    }

                    triangles.Add(centerVertIndex);

                    triangles.Add(centerVertIndex + (iTri + 2 <= (int)HexEdgeDirection.MAX ? iTri + 2 : iTri - 4));
                    triangles.Add(centerVertIndex + (iTri + 1));

                    bottomTriangles.Add(totalTriangles++);
                }
            }

            /*
            * Generate the sides connecting the top to the bottom.
            */
            int matIndex = 0;
            for (HexEdgeDirection edge = 0; edge < HexEdgeDirection.MAX; ++edge)
            {
                // Get information on the corners and hidden status of this edge...
                HexCornerDirection counterCorner = HexHelper.GetCornerDirectionNextToEdge(edge, false);
                HexCornerDirection clockwiseCorner = HexHelper.GetCornerDirectionNextToEdge(edge, true);

                bool edgeHidden = hideSides[(int)edge];
                bool counterEdgeHidden = hideSides[(int)HexHelper.GetEdgeDirectionNextToEdge(edge, false)];
                bool clockwiseEdgeHidden = hideSides[(int)HexHelper.GetEdgeDirectionNextToEdge(edge, true)];

                // Calculate vertices and UVs...
                int counterOuterTopIndex = -1;
                int counterOuterBottomIndex = -1;
                int clockwiseOuterTopIndex = -1;
                int clockwiseOuterBottomIndex = -1;

                int counterInnerTopIndex = -1;
                int counterInnerBottomIndex = -1;
                int clockwiseInnerTopIndex = -1;
                int clockwiseInnerBottomIndex = -1;

                {
                    Vector3 vertPos;

                    if (!(edgeHidden && counterEdgeHidden))
                    {
                        vertPos = HexHelper.GetCornerDirectionVector(counterCorner);

                        // Counter-clockwise corner, Outer, Top
                        vertPos.y = topEnd.corners[(int)counterCorner].height;
                        vertices.Add(vertPos);
                        uvs.Add(new Vector2(1, vertPos.y / sideTextureHeight));
                        counterOuterTopIndex = vertices.Count - 1;

                        // Counter-clockwise corner, Outer, Bottom
                        vertPos.y = bottomEnd.corners[(int)counterCorner].height;
                        vertices.Add(vertPos);
                        uvs.Add(new Vector2(1, vertPos.y / sideTextureHeight));
                        counterOuterBottomIndex = vertices.Count - 1;

                        if (edgeHidden)
                        {
                            vertPos = Vector3.zero;

                            // Counter-clockwise corner, Inner, Top
                            vertPos.y = topEnd.centerHeight;
                            vertices.Add(vertPos);
                            uvs.Add(new Vector2(0, vertPos.y / sideTextureHeight));
                            counterInnerTopIndex = vertices.Count - 1;

                            // Counter-clockwise corner, Inner, Bottom
                            vertPos.y = bottomEnd.centerHeight;
                            vertices.Add(vertPos);
                            uvs.Add(new Vector2(0, vertPos.y / sideTextureHeight));
                            counterInnerBottomIndex = vertices.Count - 1;
                        }
                    }

                    if (!(edgeHidden && clockwiseEdgeHidden))
                    {
                        vertPos = HexHelper.GetCornerDirectionVector(clockwiseCorner);

                        // Clockwise corner, Outer, Top
                        vertPos.y = topEnd.corners[(int)clockwiseCorner].height;
                        vertices.Add(vertPos);
                        uvs.Add(new Vector2(0, vertPos.y / sideTextureHeight));
                        clockwiseOuterTopIndex = vertices.Count - 1;

                        // Clockwise corner, Outer, Bottom
                        vertPos.y = bottomEnd.corners[(int)clockwiseCorner].height;
                        vertices.Add(vertPos);
                        uvs.Add(new Vector2(0, vertPos.y / sideTextureHeight));
                        clockwiseOuterBottomIndex = vertices.Count - 1;

                        if (edgeHidden)
                        {
                            vertPos = Vector3.zero;

                            // Clockwise corner, Inner, Top
                            vertPos.y = topEnd.centerHeight;
                            vertices.Add(vertPos);
                            uvs.Add(new Vector2(1, vertPos.y / sideTextureHeight));
                            clockwiseInnerTopIndex = vertices.Count - 1;

                            // Clockwise corner, Inner, Bottom
                            vertPos.y = bottomEnd.centerHeight;
                            vertices.Add(vertPos);
                            uvs.Add(new Vector2(1, vertPos.y / sideTextureHeight));
                            clockwiseInnerBottomIndex = vertices.Count - 1;
                        }
                    }
                }

                // Calculate triangles...
                if (!edgeHidden)
                {
                    Material mat = sideMaterials[matIndex];
                    List<int> triangles = trianglesPerMat[mat];
                    matIndex++;

                    triangles.Add(counterOuterTopIndex);      // Top-right of outer quad
                    triangles.Add(counterOuterBottomIndex);   // Low-right of outer quad
                    triangles.Add(clockwiseOuterTopIndex);    // Top-left of outer quad
                    sideTriangles.Add(totalTriangles++);

                    triangles.Add(clockwiseOuterTopIndex);    // Top-left of outer quad
                    triangles.Add(counterOuterBottomIndex);   // Low-right of outer quad
                    triangles.Add(clockwiseOuterBottomIndex); // Low-left of outer quad
                    sideTriangles.Add(totalTriangles++);
                }
                else
                {
                    if (!counterEdgeHidden)
                    {
                        Material mat = sideMaterials[matIndex];
                        List<int> triangles = trianglesPerMat[mat];
                        matIndex++;

                        triangles.Add(counterOuterTopIndex);    // Top-right of inner counter-clockwise quad
                        triangles.Add(counterOuterBottomIndex); // Low-right of inner counter-clockwise quad
                        triangles.Add(counterInnerTopIndex);    // Top-left of inner counter-clockwise quad
                        sideTriangles.Add(totalTriangles++);

                        triangles.Add(counterInnerTopIndex);    // Top-left of inner counter-clockwise quad
                        triangles.Add(counterOuterBottomIndex); // Low-right of inner counter-clockwise quad
                        triangles.Add(counterInnerBottomIndex); // Low-left of inner counter-clockwise quad
                        sideTriangles.Add(totalTriangles++);
                    }

                    if (!clockwiseEdgeHidden)
                    {
                        Material mat = sideMaterials[matIndex];
                        List<int> triangles = trianglesPerMat[mat];
                        matIndex++;

                        triangles.Add(clockwiseInnerTopIndex);    // Top-right of inner clockwise quad
                        triangles.Add(clockwiseInnerBottomIndex); // Low-right of inner clockwise quad
                        triangles.Add(clockwiseOuterTopIndex);    // Top-left of inner clockwise quad
                        sideTriangles.Add(totalTriangles++);

                        triangles.Add(clockwiseOuterTopIndex);    // Top-left of inner clockwise quad
                        triangles.Add(clockwiseInnerBottomIndex); // Low-right of inner clockwise quad
                        triangles.Add(clockwiseOuterBottomIndex); // Low-left of inner clockwise quad
                        sideTriangles.Add(totalTriangles++);
                    }
                }
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

            // Have the physics use this meh as a collider
            MeshCollider collider = GetComponent<MeshCollider>();
            if (collider)
                collider.sharedMesh = mesh;
        }

        void SelectTopMaterialFromBrush()
        {
            topMaterial = topMaterialBrush.materials[Random.Range(0, topMaterialBrush.materials.Length)];

            if (topMaterialBrush.endPaintStyle == HexPillarMaterialBrush.EndPaintStyle.PerHexWithRandomRotation)
            {
                topMaterialRotation = (short)Random.Range(0, 6);
            }
            else
            {
                topMaterialRotation = 0;
            }
        }

        void SelectSideMaterialsFromBrush()
        {
            sideMaterials = new Material[GetNeededSideMaterialCount()];

            if (sideMaterialBrush.sidePaintStyle == HexPillarMaterialBrush.SidePaintStyle.RandomMaterialOnEveryFace)
            {
                for (int i = 0; i < sideMaterials.Length; ++i)
                {
                    sideMaterials[i] = sideMaterialBrush.materials[Random.Range(0, sideMaterialBrush.materials.Length)];
                }
            }
            else
            {
                if (sideMaterialBrush.sidePaintStyle == HexPillarMaterialBrush.SidePaintStyle.WrapFromRandomCorner)
                {
                    sideMaterialWrapCorner = (HexCornerDirection)Random.Range(0, (int)HexCornerDirection.MAX);
                }
                else
                {
                    sideMaterialWrapCorner = (HexCornerDirection)(sideMaterialBrush.sidePaintStyle - 2);
                }

                int matIndex = sideMaterialBrush.materials.Length - 1;
                for (int i = 0; i < sideMaterials.Length; ++i)
                {
                    sideMaterials[i] = sideMaterialBrush.materials[matIndex];

                    matIndex--;

                    if (matIndex < 0)
                        matIndex = sideMaterialBrush.materials.Length - 1;
                }
            }
        }

        void SelectBottomMaterialFromBrush()
        {
            bottomMaterial = bottomMaterialBrush.materials[Random.Range(0, bottomMaterialBrush.materials.Length)];

            if (bottomMaterialBrush.endPaintStyle == HexPillarMaterialBrush.EndPaintStyle.PerHexWithRandomRotation)
            {
                bottomMaterialRotation = (short)Random.Range(0, 6);
            }
            else
            {
                bottomMaterialRotation = 0;
            }
        }

        int GetNeededSideMaterialCount()
        {
            int count = 0;

            for (HexEdgeDirection direction = 0; direction < HexEdgeDirection.MAX; ++direction)
            {
                if (hideSides[(int)direction])
                {
                    // Add one material for the counter-clockwise edge of the cut-out side.
                    if (!hideSides[(int)HexHelper.GetEdgeDirectionNextToEdge(direction, false)])
                        count++;

                    // Add one material for the clockwise edge of the cut-out side.
                    if (!hideSides[(int)HexHelper.GetEdgeDirectionNextToEdge(direction, true)])
                        count++;
                }
                else
                {
                    // Add one material for the unhidden side.
                    count++;
                }
            }

            return count;
        }

        void OnUndoRedo()
        {
            if (this)
            {
                GenerateMesh();
            }
        }

        public override HexTerrain GetTerrain()
        {
            return terrain;
        }

        public bool IsFloorTriangle(int triangleIndex)
        {
            return topTriangles.Contains(triangleIndex);
        }

        public bool IsSideTriangle(int sideIndex)
        {
            return sideTriangles.Contains(sideIndex);
        }

        public bool IsCeilingTriangle(int ceilingIndex)
        {
            return bottomTriangles.Contains(ceilingIndex);
        }
    }
}