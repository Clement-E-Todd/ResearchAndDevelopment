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
    }

    public void Constrain(float minHeight, float maxHeight, HexPillarInfo pillarAbove, HexPillarInfo pillarBelow)
    {
        topEnd.centerHeight = Mathf.Clamp(topEnd.centerHeight, bottomEnd.centerHeight, maxHeight);
        bottomEnd.centerHeight = Mathf.Clamp(bottomEnd.centerHeight, minHeight, topEnd.centerHeight);

        if (pillarAbove)
        {
            topEnd.centerHeight = Mathf.Min(topEnd.centerHeight, pillarAbove.bottomEnd.centerHeight);
        }

        if (pillarBelow)
        {
            bottomEnd.centerHeight = Mathf.Max(bottomEnd.centerHeight, pillarBelow.topEnd.centerHeight);
        }

        for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
        {
            topEnd.cornerHeights[(int)corner] = Mathf.Clamp(topEnd.cornerHeights[(int)corner], bottomEnd.cornerHeights[(int)corner], maxHeight);
            bottomEnd.cornerHeights[(int)corner] = Mathf.Clamp(bottomEnd.cornerHeights[(int)corner], minHeight, topEnd.cornerHeights[(int)corner]);

            if (pillarAbove)
            {
                topEnd.cornerHeights[(int)corner] = Mathf.Min(topEnd.cornerHeights[(int)corner], pillarAbove.bottomEnd.cornerHeights[(int)corner]);
            }

            if (pillarBelow)
            {
                bottomEnd.cornerHeights[(int)corner] = Mathf.Max(bottomEnd.cornerHeights[(int)corner], pillarBelow.topEnd.cornerHeights[(int)corner]);
            }
        }
    }
}