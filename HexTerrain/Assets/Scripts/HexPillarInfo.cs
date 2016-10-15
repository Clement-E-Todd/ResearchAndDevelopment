using UnityEngine;

public class HexPillarInfo
{
    public class End
    {
        public float centerHeight;
        public float centerSplitHeight = 0f;

        public float[] cornerHeights = new float[(int)HexCorner.MAX];
        public bool[] cornerSplitHeights = new bool[(int)HexCorner.MAX];

        public End(float height)
        {
            centerHeight = height;

            for (int i = 0; i < cornerHeights.Length; ++i)
            {
                cornerHeights[i] = height;
            }
        }
    }

    public End topEnd = new End(1f);
    public End lowEnd = new End(0f);

    public bool drawTopEnd = true;
    public Material topMaterial;

    public bool drawLowEnd = true;
    public Material bottomMaterial;

    public Material[] wallMaterials = new Material[(int)HexEdge.MAX];
    public float sideTextureHeight = 1f;
}