using UnityEngine;

public abstract class HexTerrain : MonoBehaviour
{
    protected const float hexRadius = 1f;

    /*
        A HexTerrain.Tile represents a single tile within the terrain.
    */
    public class Tile
    {
        /*
            A layer represents a floor or ceiling within this Hex.
            Our array of layers alternates between floors and ceilings; if the first layer
            in the array represents a ceiling (ie topLayerIsCeiling == true), then the next
            one will represent the floor below it, then the next one will represent a
            ceiling below that etc.
        */
        class Layer
        {
            float centerHeight;
            float centerWallHeight = 0f;

            float[] cornerHeights = new float[(int)HexCorner.MAX];
            bool[] cornerWallHeights = new bool[(int)HexCorner.MAX];

            public Layer(float height)
            {
                centerHeight = height;

                for (int i = 0; i < cornerHeights.Length; ++i)
                {
                    cornerHeights[i] = height;
                }
            }
        }
        private Layer[] layers;
        private bool topLayerIsCeiling;

        public Tile() : this(0f) { }

        public Tile(float height)
        {
            layers = new Layer[] { new Layer(height) };
        }
    }

    protected HexTerrainTileGrid tileGrid = new HexTerrainTileGrid();

    public Vector3 GetLocalPositionForCoord(HexGrid.Coord coord)
    {
        Vector3 xDirection = HexHelper.GetEdgeCenterOffset(HexEdge.SouthEast) * 2;
        Vector3 yDirection = HexHelper.GetEdgeCenterOffset(HexEdge.NorthEast) * 2;
        return (xDirection * coord.x) + (yDirection * coord.y);
    }
}