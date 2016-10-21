using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexPillarCorner : MonoBehaviour
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

    public void UpdatePosition()
    {
        transform.localPosition = HexHelper.GetCornerDirectionVector(corner) * end.pillar.terrain.hexRadius;
        transform.localPosition += new Vector3(0, end.corners[(int)corner].height - end.transform.localPosition.y, 0);
    }
}