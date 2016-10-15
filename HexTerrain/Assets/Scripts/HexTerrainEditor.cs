using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(HexTerrainEditable))]
public class HexTerrainEditor : Editor
{
    public void OnSceneGUI()
    {
        HexTerrainEditable terrain = Selection.activeGameObject ? Selection.activeGameObject.GetComponent<HexTerrainEditable>() : null;

		if (!terrain)
		{
			return;
		}

        foreach (List<HexTerrain.Pillar> pillarSet in terrain.pillarGrid)
        {
            HexGrid.Coord coord = new HexGrid.Coord();
            terrain.pillarGrid.TryGetCoordForItem(pillarSet, out coord);

            Vector3 positionBefore = terrain.GetLocalPositionForCoord(coord);
            positionBefore.y = pillarSet[0].topEnd.centerHeight;
            positionBefore = terrain.transform.TransformPoint(positionBefore);

            Vector3 direction = terrain.transform.TransformDirection(Vector3.up);

            Handles.color = new Color(1f, 0.5f, 0f);
            Vector3 positionAfter = Handles.Slider(positionBefore, direction);
            Vector3 positionDelta = positionAfter - positionBefore;

            float delta = Vector3.Dot(positionDelta, direction);

            pillarSet[0].topEnd.centerHeight = Mathf.Max(pillarSet[0].topEnd.centerHeight + delta, pillarSet[0].lowEnd.centerHeight);

            for (int i = 0; i < pillarSet[0].topEnd.cornerHeights.Length; ++i)
            {
                pillarSet[0].topEnd.cornerHeights[i] = Mathf.Max(pillarSet[0].topEnd.cornerHeights[i] + delta, pillarSet[0].lowEnd.cornerHeights[i]);
            }

            if (delta != 0f)
            {
                terrain.GenerateMeshForPillar(pillarSet[0]);
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
