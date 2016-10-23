namespace HexTerrain
{
    using UnityEngine;

    public class HexPillarCorner : HexTerrainElement
    {
        public HexPillarEnd end;
        public HexCornerDirection direction;
        public float height;

        public bool isOnTopEnd { get { return end.isTopEnd; } }

        public void Init(HexPillarEnd end, HexCornerDirection corner)
        {
            this.end = end;
            this.direction = corner;
        }

        void OnDestroy()
        {
            if (doNotRecreateOnDestroy || !end)
                return;

            string cornerName = string.Format("Corner ({0}, {1})", direction.ToString(), end.isTopEnd ? "Top" : "Bottom");
            GameObject cornerGmeObject = new GameObject(cornerName, typeof(HexPillarCorner));
            cornerGmeObject.transform.SetParent(end.transform);
            end.corners[(int)direction] = cornerGmeObject.GetComponent<HexPillarCorner>();
            end.corners[(int)direction].Init(end, direction);
            end.corners[(int)direction].height = end.centerHeight;
            end.corners[(int)direction].UpdatePosition();

            end.pillar.GenerateMesh();
        }

        public void UpdatePosition()
        {
            transform.localPosition = HexHelper.GetCornerDirectionVector(direction) * end.pillar.terrain.hexRadius;
            transform.localPosition += new Vector3(0, end.corners[(int)direction].height - end.transform.localPosition.y, 0);
        }

        public override HexTerrain GetTerrain()
        {
            return end.pillar.terrain;
        }
    }
}