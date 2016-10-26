namespace HexTerrain
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    public class HexTerrain : HexTerrainElement
    {
        public float hexRadius = 1f;
        public float heightSnap = 0.5f;
        public float minHeight = 0f;
        public float maxHeight = 20f;

        public HexPillarEndBrush[] pillarFloorBrushes;
        public HexPillarEndBrush[] pillarCeilingBrushes;

        public Material[] floorMaterials;
        public Material[] ceilingMaterials;
        public Material[] wallMaterials;

        public HexTerrainPillarGrid pillarGrid = new HexTerrainPillarGrid();

        const int sidesPerHex = 6;

        public HexPillar AddNewPillar(HexGrid.Coord coord, float topHeight = 1f, float bottomHeight = 0f)
        {
            if (floorMaterials == null || ceilingMaterials == null || wallMaterials == null)
                return null;

            GameObject pillarObject = new GameObject();
            pillarObject.name = string.Format("Pillar [{0}, {1}]", coord.x, coord.y);
            pillarObject.transform.SetParent(transform);
            pillarObject.transform.localPosition = GetLocalPositionForCoord(coord);

            pillarObject.AddComponent<MeshFilter>();
            pillarObject.AddComponent<MeshRenderer>();
            pillarObject.AddComponent<MeshCollider>();

            HexPillar pillar = pillarObject.AddComponent<HexPillar>();
            pillar.Init(this, coord);

            if (!pillarGrid.ContainsItemAtCoord(coord))
            {
                pillarGrid.Add(coord, new List<HexPillar>());
            }
            pillarGrid[coord].Add(pillar);

            if (pillarGrid[coord].Count > 1)
            {
                pillarGrid[coord] = pillarGrid[coord].OrderBy(x => (x.topEnd.centerHeight + x.bottomEnd.centerHeight) / 2).ToList();

                for (int i = 0; i < pillarGrid[coord].Count; ++i)
                {
                    pillarGrid[coord][i].name = string.Format("Pillar [{0}, {1}] #{2}", coord.x, coord.y, i);
                }
            }
            else
            {
                pillarGrid[coord][0].name = string.Format("Pillar [{0}, {1}]", coord.x, coord.y);
            }

            pillar.topEnd.SetFlatHeight(topHeight);
            pillar.bottomEnd.SetFlatHeight(bottomHeight);

            if (floorMaterials.Length > 0)
                pillar.topMaterial = floorMaterials[Random.Range(0, floorMaterials.Length)];

            if (ceilingMaterials.Length > 0)
                pillar.bottomMaterial = ceilingMaterials[Random.Range(0, ceilingMaterials.Length)];

            if (wallMaterials.Length > 0)
            {
                for (HexEdgeDirection edgeDirection = 0; edgeDirection < HexEdgeDirection.MAX; ++edgeDirection)
                {
                    pillar.wallMaterials[(int)edgeDirection] = wallMaterials[Random.Range(0, wallMaterials.Length)];
                }
            }

            pillar.GenerateMesh();

            return pillar;
        }

        public void AddExistingPillar(HexPillar pillar)
        {
            if (!pillarGrid.ContainsItemAtCoord(pillar.coord))
            {
                pillarGrid.Add(pillar.coord, new List<HexPillar>());
            }
            pillarGrid[pillar.coord].Add(pillar);

            if (pillarGrid[pillar.coord].Count > 1)
            {
                pillarGrid[pillar.coord] = pillarGrid[pillar.coord].OrderBy(x => (x.topEnd.centerHeight + x.bottomEnd.centerHeight) / 2).ToList();
            }
        }

        public Vector3 GetLocalPositionForCoord(HexGrid.Coord coord, float height = 0f)
        {
            Vector3 xDirection = HexHelper.GetEdgeCenterOffset(HexEdgeDirection.SouthEast, hexRadius) * 2;
            Vector3 yDirection = HexHelper.GetEdgeCenterOffset(HexEdgeDirection.NorthEast, hexRadius) * 2;
            return (xDirection * coord.x) + (yDirection * coord.y) + (transform.up * height);
        }

        public HexGrid.Coord GetCoordForLocalPosition(Vector3 localPosition)
        {
            Vector3 xDirection = HexHelper.GetEdgeCenterOffset(HexEdgeDirection.SouthEast, hexRadius) * 2;
            Vector3 yDirection = HexHelper.GetEdgeCenterOffset(HexEdgeDirection.NorthEast, hexRadius) * 2;

            Ray xRay = new Ray(transform.up * Vector3.Dot(transform.up, localPosition), xDirection.normalized);
            Plane yPlane = new Plane(Vector3.Cross(yDirection.normalized, transform.up), localPosition);

            float xDistance = 0f;
            yPlane.Raycast(xRay, out xDistance);
            Vector3 interestionPoint = xRay.GetPoint(xDistance);

            Vector2 xyDistances = new Vector2(
                Vector3.Distance(xRay.origin, interestionPoint) / xDirection.magnitude,
                Vector3.Distance(interestionPoint, localPosition) / yDirection.magnitude);

            if (Vector3.Dot(Vector3.Cross(transform.up, yDirection.normalized), localPosition) < 0f)
                xyDistances.x *= -1f;

            if (Vector3.Dot(Vector3.Cross(xDirection.normalized, transform.up), localPosition) < 0f)
                xyDistances.y *= -1f;

            return new HexGrid.Coord((int)Mathf.Round(xyDistances.x), (int)Mathf.Round(xyDistances.y));
        }

        public HexGrid.Coord GetCoordForWorldPosition(Vector3 worldPosition)
        {
            return GetCoordForLocalPosition(transform.InverseTransformPoint(worldPosition));
        }

        bool TryGetCoordForPillar(HexPillar pillar, out HexGrid.Coord coord)
        {
            foreach (List<HexPillar> pillarsInHex in pillarGrid)
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

        public override HexTerrain GetTerrain()
        {
            return this;
        }
    }
}