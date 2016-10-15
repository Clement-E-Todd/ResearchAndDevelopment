using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexPillarCornerEditable : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    float snapToNeighbour = 0f;

    HexPillarEditable pillar;
    HexCorner corner;
    bool isOnTopEnd;

    public void Init(HexPillarEditable pillar, HexCorner corner, bool isOnTopEnd)
    {
        this.pillar = pillar;
        this.corner = corner;
        this.isOnTopEnd = isOnTopEnd;
    }

    public void UpdatePosition()
    {
        transform.localPosition = HexHelper.GetCornerDirection(corner) * pillar.owner.hexRadius;
        float[] cornerHeights = isOnTopEnd ? pillar.pillarInfo.topEnd.cornerHeights : pillar.pillarInfo.bottomEnd.cornerHeights;
        transform.localPosition += new Vector3(0, cornerHeights[(int)corner], 0);
    }
}