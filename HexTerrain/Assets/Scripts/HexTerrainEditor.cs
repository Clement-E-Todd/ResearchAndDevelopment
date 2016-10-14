using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexTerrainEditable))]
public class HexTerrainEditor : Editor
{
    public void OnSceneGUI()
    {
        HexTerrainEditable terrain = Selection.activeGameObject.GetComponent<HexTerrainEditable>();

        foreach (HexTerrain.Tile tile in terrain.tileGrid)
        {
            HexGrid.Coord coord = new HexGrid.Coord();
            terrain.tileGrid.TryGetCoordForItem(tile, out coord);

            Vector3 positionBefore = terrain.GetLocalPositionForCoord(coord);
            positionBefore.y = tile.layers[0].centerHeight;
            positionBefore = terrain.transform.TransformPoint(positionBefore);

            Vector3 direction = terrain.transform.TransformDirection(Vector3.up);

            Handles.color = new Color(1f, 0.5f, 0f);
            Vector3 positionAfter = Handles.Slider(positionBefore, direction);
            Vector3 positionDelta = positionAfter - positionBefore;

            float delta = Vector3.Dot(positionDelta, direction);

            tile.layers[0].centerHeight = Mathf.Max(tile.layers[0].centerHeight + delta, tile.layers[1].centerHeight);

            for (int i = 0; i < tile.layers[0].cornerHeights.Length; ++i)
            {
                tile.layers[0].cornerHeights[i] = Mathf.Max(tile.layers[0].cornerHeights[i] + delta, tile.layers[1].cornerHeights[i]);
            }

            if (delta != 0f)
            {
                terrain.GenerateMeshForTile(tile);
            }
        }

        /*
        Handles.BeginGUI();

        if (GUILayout.Button("Press Me"))
            Debug.Log(terrain);

        Handles.EndGUI();
        */
    }
}
