using UnityEngine;
using System.Collections.Generic;

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
        public class Layer
        {
            public float centerHeight;
            public float centerWallHeight = 0f;

            public float[] cornerHeights = new float[(int)HexCorner.MAX];
            public bool[] cornerWallHeights = new bool[(int)HexCorner.MAX];

            public Layer(float height)
            {
                centerHeight = height;

                for (int i = 0; i < cornerHeights.Length; ++i)
                {
                    cornerHeights[i] = height;
                }
            }
        }
        public List<Layer> layers = new List<Layer>();
        public bool topLayerIsCeiling;

        public Tile() { }

        public Tile(params float[] layerHeights)
        {
            for (int i = 0; i < layerHeights.Length; ++i)
            {
                layers.Add(new Layer(layerHeights[i]));
            }
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