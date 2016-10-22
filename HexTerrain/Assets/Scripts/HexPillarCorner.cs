using UnityEngine;

public class HexPillarCorner : HexTerrainElement
{
    public HexPillarEnd end { get; private set; }
    public HexCornerDirection corner { get; private set; }
    public bool isOnTopEnd { get { return end.isTopEnd; } }
    
    public float height;

    public void Init(HexPillarEnd end, HexCornerDirection corner)
    {
        this.end = end;
        this.corner = corner;
    }

    void OnDestroy()
    {
        if (doNotRecreateOnDestroy || !end)
            return;

        string cornerName = string.Format("Corner ({0}, {1})", corner.ToString(), end.isTopEnd ? "Top" : "Bottom");
        GameObject cornerGmeObject = new GameObject(cornerName, typeof(HexPillarCorner));
        cornerGmeObject.transform.SetParent(end.transform);
        end.corners[(int)corner] = cornerGmeObject.GetComponent<HexPillarCorner>();
        end.corners[(int)corner].Init(end, corner);
        end.corners[(int)corner].height = end.centerHeight;
        end.corners[(int)corner].UpdatePosition();

        end.pillar.GenerateMesh();
    }

    public void UpdatePosition()
    {
        transform.localPosition = HexHelper.GetCornerDirectionVector(corner) * end.pillar.terrain.hexRadius;
        transform.localPosition += new Vector3(0, end.corners[(int)corner].height - end.transform.localPosition.y, 0);
    }

    public override HexTerrain GetTerrain()
    {
        return end.pillar.terrain;
    }
}