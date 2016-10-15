using UnityEngine;

public class HexPillarInfo : ScriptableObject
{
    public class End : ScriptableObject
    {
        public float centerHeight;
        public float centerSplitHeight = 0f;

        public float[] cornerHeights = new float[(int)HexCorner.MAX];
        public bool[] cornerSplitHeights = new bool[(int)HexCorner.MAX];

        public void SetFlatHeight(float height)
        {
            centerHeight = height;

            for (int i = 0; i < cornerHeights.Length; ++i)
            {
                cornerHeights[i] = height;
            }
        }
    }

    public End topEnd;
    public End bottomEnd;

    public bool drawTopEnd = true;
    public Material topMaterial;

    public bool drawBottomEnd = true;
    public Material bottomMaterial;

    public Material[] wallMaterials = new Material[(int)HexEdge.MAX];
    public float sideTextureHeight = 1f;

    void Awake()
    {
        topEnd = ScriptableObject.CreateInstance<HexPillarInfo.End>();
        bottomEnd = ScriptableObject.CreateInstance<HexPillarInfo.End>();

        topEnd.SetFlatHeight(1f);
        bottomEnd.SetFlatHeight(0f);
    }
}