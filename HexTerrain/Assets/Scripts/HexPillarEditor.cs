using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(GameObject))]
[CanEditMultipleObjects]
public class HexPillarEditor : Editor
{
    public void OnSceneGUI()
    {
        HexPillarEditable selectedPillar = Selection.activeGameObject ? Selection.activeGameObject.GetComponent<HexPillarEditable>() : null;

        if (!selectedPillar)
        {
            Tools.hidden = false;
            return;
        }

        Tools.hidden = true;

        TopHandle(selectedPillar);

        BottomHandle(selectedPillar);

        /*
        Handles.BeginGUI();

        if (GUILayout.Button("Press Me"))
            Debug.Log(terrain);

        Handles.EndGUI();
        */
    }

    void TopHandle(HexPillarEditable selectedPillar)
    {
        Handles.color = new Color(1f, 0.5f, 0f);

        Vector3 positionBefore = new Vector3(0, selectedPillar.pillarInfo.topEnd.centerHeight, 0);
        positionBefore = selectedPillar.transform.TransformPoint(positionBefore);
        Vector3 direction = selectedPillar.transform.TransformDirection(Vector3.up);

        Vector3 positionAfter = Handles.Slider(positionBefore, direction);
        Vector3 positionDelta = positionAfter - positionBefore;

        float delta = Vector3.Dot(positionDelta, direction);

        if (delta != 0f)
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                HexPillarEditable selection = selectedObject.GetComponent<HexPillarEditable>();

                if (!selection)
                    continue;

                selection.pillarInfo.topEnd.centerHeight = Mathf.Max(selection.pillarInfo.topEnd.centerHeight + delta, selection.pillarInfo.lowEnd.centerHeight);

                for (int i = 0; i < selection.pillarInfo.topEnd.cornerHeights.Length; ++i)
                {
                    selection.pillarInfo.topEnd.cornerHeights[i] = Mathf.Max(selection.pillarInfo.topEnd.cornerHeights[i] + delta, selection.pillarInfo.lowEnd.cornerHeights[i]);
                }

                selection.GenerateMesh(selection.pillarInfo);
            }
        }
    }

    void BottomHandle(HexPillarEditable selectedPillar)
    {
        Handles.color = new Color(1f, 0.5f, 0f);

        Vector3 positionBefore = new Vector3(0, selectedPillar.pillarInfo.lowEnd.centerHeight, 0);
        positionBefore = selectedPillar.transform.TransformPoint(positionBefore);
        Vector3 direction = selectedPillar.transform.TransformDirection(Vector3.down);

        Vector3 positionAfter = Handles.Slider(positionBefore, direction);
        Vector3 positionDelta = positionAfter - positionBefore;

        float delta = Vector3.Dot(positionDelta, direction);

        if (delta != 0f)
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                HexPillarEditable selection = selectedObject.GetComponent<HexPillarEditable>();

                if (!selection)
                    continue;

                selection.pillarInfo.lowEnd.centerHeight = Mathf.Min(selection.pillarInfo.lowEnd.centerHeight - delta, selection.pillarInfo.topEnd.centerHeight);

                for (int i = 0; i < selection.pillarInfo.lowEnd.cornerHeights.Length; ++i)
                {
                    selection.pillarInfo.lowEnd.cornerHeights[i] = Mathf.Min(selection.pillarInfo.lowEnd.cornerHeights[i] - delta, selection.pillarInfo.topEnd.cornerHeights[i]);
                }

                selection.GenerateMesh(selection.pillarInfo);
            }
        }
    }
}