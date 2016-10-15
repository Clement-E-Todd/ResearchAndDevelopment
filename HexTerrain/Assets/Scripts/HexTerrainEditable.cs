using UnityEngine;
using System.Collections.Generic;

public class HexTerrainEditable : HexTerrain
{
    public Dictionary<HexPillarInfo, HexPillarEditable> pillarObjects = new Dictionary<HexPillarInfo, HexPillarEditable>();

    const int sidesPerHex = 6;

    void Start()
    {
        AddPillar(new HexGrid.Coord(0, 0));
        //AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.SouthWest));
        //AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.South));
        //AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.SouthEast));
        //AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.NorthEast));
        //AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.North));
        //AddPillar(HexGrid.GetNeighbourCoordOffset(HexEdge.NorthWest));
    }

    void AddPillar(HexGrid.Coord coord)
    {
        HexPillarInfo pillarInfo = ScriptableObject.CreateInstance<HexPillarInfo>();
        
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
            editable.Init(this, pillarInfo);
            editable.GenerateMesh();
            pillarObjects.Add(pillarInfo, editable);
            pillarGrid[coord].Add(pillarInfo);

            for (int i = 0; i < pillarGrid[coord].Count; ++i)
            {
                pillarObjects[pillarGrid[coord][i]].name  = string.Format("Pillar [{0}, {1}] #{2}", coord.x, coord.y, i);
            }
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
