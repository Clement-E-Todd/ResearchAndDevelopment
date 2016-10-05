using UnityEngine;

public abstract class HexTerrain : MonoBehaviour
{
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
		}
		private Layer[] layers;
		private bool topLayerIsCeiling;
	}
}