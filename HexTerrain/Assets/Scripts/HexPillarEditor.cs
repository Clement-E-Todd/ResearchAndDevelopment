using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(GameObject))]
[CanEditMultipleObjects]
public class HexPillarEditor : Editor
{
    bool toolsHiddenByMe = false;

    public void OnSceneGUI()
    {
        HexPillarEditable selectedPillar = Selection.activeGameObject ? Selection.activeGameObject.GetComponent<HexPillarEditable>() : null;

        if (!selectedPillar)
        {
            if (toolsHiddenByMe)
            {
                Tools.hidden = false;
                toolsHiddenByMe = false;
            }

            return;
        }

        Tools.hidden = true;
        toolsHiddenByMe = true;

        TopHandle(selectedPillar);
        MiddleHandle(selectedPillar);
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
        float delta = Handle(
            selectedPillar,
            new Vector3(0, selectedPillar.pillarInfo.topEnd.centerHeight, 0),
            Vector3.up);

        if (delta != 0f)
        {
            MoveTop(delta);
            RedrawSelections();
        }
    }

    void MiddleHandle(HexPillarEditable selectedPillar)
    {
        float height = (selectedPillar.pillarInfo.topEnd.centerHeight + selectedPillar.pillarInfo.lowEnd.centerHeight) / 2;

        float delta = Handle(
            selectedPillar,
            new Vector3(0, height, 0),
            Vector3.up,
            0.5f,
            Handles.SphereCap);

        if (delta != 0f)
        {
            MoveTop(delta);
            MoveBottom(-delta);
            RedrawSelections();
        }
    }

    void BottomHandle(HexPillarEditable selectedPillar)
    {
        float delta = Handle(
            selectedPillar,
            new Vector3(0, selectedPillar.pillarInfo.lowEnd.centerHeight, 0),
            Vector3.down);

        if (delta != 0f)
        {
            MoveBottom(delta);
            RedrawSelections();
        }
    }

    float Handle(
        HexPillarEditable pillar,
        Vector3 localPosition,
        Vector3 localDirection,
        float size = 1f,
        Handles.DrawCapFunction cap = null)
    {
        if (cap == null)
            cap = Handles.ArrowCap;

        Handles.color = new Color(1f, 0.5f, 0f);

        Vector3 positionBefore = pillar.transform.TransformPoint(localPosition);
        Vector3 direction = pillar.transform.TransformDirection(localDirection);

        Vector3 positionAfter = Handles.Slider(positionBefore, direction, size, cap, 0f);
        Vector3 positionDelta = positionAfter - positionBefore;

        return Vector3.Dot(positionDelta, direction);
    }

    void MoveTop(float amount)
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillarEditable selection = selectedObject.GetComponent<HexPillarEditable>();

            if (!selection)
                continue;

            selection.pillarInfo.topEnd.centerHeight = Mathf.Max(selection.pillarInfo.topEnd.centerHeight + amount, selection.pillarInfo.lowEnd.centerHeight);

            for (int i = 0; i < selection.pillarInfo.topEnd.cornerHeights.Length; ++i)
            {
                selection.pillarInfo.topEnd.cornerHeights[i] = Mathf.Max(selection.pillarInfo.topEnd.cornerHeights[i] + amount, selection.pillarInfo.lowEnd.cornerHeights[i]);
            }
        }
    }

    void MoveBottom(float amount)
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillarEditable selection = selectedObject.GetComponent<HexPillarEditable>();

            if (!selection)
                continue;

            selection.pillarInfo.lowEnd.centerHeight = Mathf.Min(selection.pillarInfo.lowEnd.centerHeight - amount, selection.pillarInfo.topEnd.centerHeight);

            for (int i = 0; i < selection.pillarInfo.lowEnd.cornerHeights.Length; ++i)
            {
                selection.pillarInfo.lowEnd.cornerHeights[i] = Mathf.Min(selection.pillarInfo.lowEnd.cornerHeights[i] - amount, selection.pillarInfo.topEnd.cornerHeights[i]);
            }
        }
    }

    void RedrawSelections()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillarEditable selection = selectedObject.GetComponent<HexPillarEditable>();

            if (!selection)
                continue;

            selection.GenerateMesh(selection.pillarInfo);
        }
    }
}