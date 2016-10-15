using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(HexTerrainEditable))]
public class HexTerrainEditor : Editor
{
    public void OnSceneGUI()
    {
        /*
        Handles.BeginGUI();

        if (GUILayout.Button("Press Me"))
            Debug.Log(terrain);

        Handles.EndGUI();
        */
    }
}
