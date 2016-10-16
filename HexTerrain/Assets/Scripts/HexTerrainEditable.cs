using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HexTerrainEditable : HexTerrain
{
    public Dictionary<HexPillarInfo, HexPillarEditable> pillarObjects = new Dictionary<HexPillarInfo, HexPillarEditable>();

    const int sidesPerHex = 6;

    void Start()
    {
        AddPillar(new HexGrid.Coord(0, 0));
        AddPillar(new HexGrid.Coord(0, 0), 3f, 2f);
        //AddPillar(new HexGrid.Coord(0, 0), 7f, 6f);
        //AddPillar(new HexGrid.Coord(0, 0), 5f, 4f);

        AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.SouthWest));
        AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.South));
        AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.SouthEast));
        AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.NorthEast));
        AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.North));
        AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.NorthWest));
    }

    void AddPillar(HexGrid.Coord coord, float topHeight = 1f, float bottomHeight = 0f)
    {
        HexPillarInfo pillarInfo = ScriptableObject.CreateInstance<HexPillarInfo>();

        pillarInfo.topEnd.SetFlatHeight(topHeight);
        pillarInfo.bottomEnd.SetFlatHeight(bottomHeight);

        if (floorMaterials.Length > 0)
            pillarInfo.topMaterial = floorMaterials[Random.Range(0, floorMaterials.Length)];

        if (ceilingMaterials.Length > 0)
            pillarInfo.bottomMaterial = ceilingMaterials[Random.Range(0, ceilingMaterials.Length)];

        if (wallMaterials.Length > 0)
        {
            for (HexEdge edge = 0; edge < HexEdge.MAX; ++edge)
            {
                pillarInfo.wallMaterials[(int)edge] = wallMaterials[Random.Range(0, wallMaterials.Length)];
            }
        }

        if (!pillarObjects.ContainsKey(pillarInfo))
        {
            if (!pillarGrid.ContainsItemAtCoord(coord))
            {
                pillarGrid.Add(coord, new List<HexPillarInfo>());
            }

            GameObject pillarObject = new GameObject();
            pillarObject.name = string.Format("Pillar [{0}, {1}]", coord.x, coord.y);
            pillarObject.transform.SetParent(transform);
            pillarObject.transform.localPosition = GetLocalPositionForCoord(coord);

            pillarObject.AddComponent<MeshFilter>();
            pillarObject.AddComponent<MeshRenderer>();

            HexPillarEditable editable = pillarObject.AddComponent<HexPillarEditable>();
            editable.Init(this, pillarInfo, coord);
            pillarObjects.Add(pillarInfo, editable);
            pillarGrid[coord].Add(pillarInfo);

            if (pillarGrid[coord].Count > 1)
            {
                pillarGrid[coord] = pillarGrid[coord].OrderBy(x => (x.topEnd.centerHeight + x.bottomEnd.centerHeight) / 2).ToList();

                for (int i = 0; i < pillarGrid[coord].Count; ++i)
                {
                    pillarObjects[pillarGrid[coord][i]].name = string.Format("Pillar [{0}, {1}] #{2}", coord.x, coord.y, i);
                }
            }
            else
            {
                pillarObjects[pillarGrid[coord][0]].name = string.Format("Pillar [{0}, {1}]", coord.x, coord.y);
            }

            editable.GenerateMesh();
        }
    }

    bool TryGetCoordForPillar(HexPillarInfo pillar, out HexGrid.Coord coord)
    {
        foreach (List<HexPillarInfo> pillarsInHex in pillarGrid)
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
