using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexPillarCornerEditable : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    float snapToNeighbour = 0f;

    public HexPillarEditable owner { get; private set; }
    public HexCorner corner { get; private set; }
    public bool isOnTopEnd { get; private set; }

    public void Init(HexPillarEditable pillar, HexCorner corner, bool isOnTopEnd)
    {
        this.owner = pillar;
        this.corner = corner;
        this.isOnTopEnd = isOnTopEnd;
    }

    public void UpdatePosition()
    {
        transform.localPosition = HexHelper.GetCornerDirection(corner) * owner.owner.hexRadius;
        float[] cornerHeights = isOnTopEnd ? owner.pillarInfo.topEnd.cornerHeights : owner.pillarInfo.bottomEnd.cornerHeights;
        transform.localPosition += new Vector3(0, cornerHeights[(int)corner], 0);
    }
}