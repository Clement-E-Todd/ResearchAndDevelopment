using UnityEngine;

public class HexPillarEnd : HexTerrainElement
{
    public HexPillar pillar { get; private set; }
    public bool isTopEnd { get; private set; }

    public float centerHeight;
    public HexPillarCorner[] corners = new HexPillarCorner[(int)HexCornerDirection.MAX];

    public void Init(HexPillar pillar, bool isTopEnd)
    {
        this.pillar = pillar;
        this.isTopEnd = isTopEnd;

        for (HexCornerDirection corner = 0; corner < HexCornerDirection.MAX; ++corner)
        {
            string cornerName = string.Format("Corner ({0}, {1})", corner.ToString(), isTopEnd ? "Top" : "Bottom");
            GameObject topCornerGameObject = new GameObject(cornerName, typeof(HexPillarCorner));
            topCornerGameObject.transform.SetParent(transform);
            corners[(int)corner] = topCornerGameObject.GetComponent<HexPillarCorner>();
            corners[(int)corner].Init(this, corner);
            corners[(int)corner].UpdatePosition();
        }
    }

    public void UpdatePosition()
    {
        transform.localPosition = new Vector3(0, centerHeight, 0);

        foreach (HexPillarCorner corner in corners)
        {
            corner.UpdatePosition();
        }
    }

    public void SetFlatHeight(float height)
    {
        centerHeight = height;

        for (int i = 0; i < corners.Length; ++i)
        {
            corners[i].height = height;
        }
    }

    public void SnapPointsToIncrement(float increment)
    {
        centerHeight = Mathf.Round(centerHeight / increment) * increment;

        for (int i = 0; i < corners.Length; ++i)
        {
            corners[i].height = Mathf.Round(corners[i].height / increment) * increment;
        }
    }

    public HexPillarEnd GetOtherEnd()
    {
        if (pillar.topEnd != this)
        {
            return pillar.topEnd;
        }
        else if (pillar.bottomEnd != this)
        {
            return pillar.bottomEnd;
        }
        else
        {
            return null;
        }
    }

    public override HexTerrain GetTerrain()
    {
        return pillar.terrain;
    }
}