﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameObject))]
[CanEditMultipleObjects]
public class HexTerrainEditor : Editor
{
    HexTerrain selectedTerrain = null;

    bool terrainElementsSelected = false;
    public bool editorControlsVisible = true;

    List<HexGrid.Coord> newPillarCoords = new List<HexGrid.Coord>();
    float newPillarTopHeight;
    float newPillarBottomHeight;

    Vector3 hexCreationHorizontalPlane;
    const float distanceToHorizonalPlane = 10f;
    
    enum CreationState
    {
        None,
        ChooseHorizontalPosition,
        ChooseTopHeight,
        ChooseBottomHeight
    }
    CreationState currentCreationState = CreationState.None;
    CreationState previousCreationState = CreationState.None;

    void Awake()
    {
        OnCreationStateBegin(currentCreationState);
    }

    public void OnSceneGUI()
    {
        if (UpdateTerrainElementsSelected())
        {
            UpdateCreationState();

            if (editorControlsVisible)
            {
                ShowTerrainEditorControls();
            }

            HandleUtility.Repaint();
        }
    }

    void ShowTerrainEditorControls()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(
            Screen.width * 0.05f, Screen.height * 0.875f,
            Screen.width * 0.9f, Screen.height * 0.125f));

        if (GUILayout.Button("Create New Hex Pillars"))
            OnCreateNewPillarsPressed();

        GUILayout.EndArea();
        Handles.EndGUI();
    }

    bool UpdateTerrainElementsSelected()
    {
        selectedTerrain = null;

        foreach (Transform transform in Selection.transforms)
        {
            if (transform.GetComponent<HexTerrain>() ||
                transform.GetComponent<HexPillarEditable>() ||
                transform.GetComponent<HexPillarCornerEditable>())
            {
                if (transform.GetComponent<HexTerrain>() && (!selectedTerrain || Selection.activeTransform == transform))
                {
                    selectedTerrain = transform.GetComponent<HexTerrain>();
                }

                if (!selectedTerrain && transform.GetComponent<HexPillarEditable>())
                {
                    selectedTerrain = transform.GetComponent<HexPillarEditable>().owner;
                }

                if (!selectedTerrain && transform.GetComponent<HexPillarCornerEditable>())
                {
                    selectedTerrain = transform.GetComponent<HexPillarCornerEditable>().owner.owner;
                }

                terrainElementsSelected = true;
                return true;
            }
        }

        if (terrainElementsSelected)
        {
            OnTerrainElementsDeselected();
            terrainElementsSelected = false;
        }

        return false;
    }

    void OnTerrainElementsDeselected()
    {
        currentCreationState = CreationState.None;
        Tools.hidden = false;
    }

    void OnCreationStateBegin(CreationState state)
    {
        editorControlsVisible = (state == CreationState.None);
        Tools.hidden = (state != CreationState.None);
    }

    void OnCreationStateEnd(CreationState state)
    {
    }

    void UpdateCreationState()
    {
        switch (currentCreationState)
        {
            case CreationState.ChooseHorizontalPosition:
                ChooseHorizontalPosition(selectedTerrain);
                break;

            case CreationState.ChooseTopHeight:
                ChooseTopHeight(selectedTerrain);
                break;

            case CreationState.ChooseBottomHeight:
                ChooseBottomHeight(selectedTerrain);
                break;
        }

        if (currentCreationState != previousCreationState)
        {
            OnCreationStateEnd(previousCreationState);
            OnCreationStateBegin(currentCreationState);
        }

        previousCreationState = currentCreationState;
    }

    void ChooseHorizontalPosition(HexTerrain terrain)
    {
        if (!Camera.current)
            return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        hexCreationHorizontalPlane = Camera.current.transform.position + Camera.current.transform.forward * distanceToHorizonalPlane;

        Plane plane = new Plane(terrain.transform.up, hexCreationHorizontalPlane);
        Ray ray = Camera.current.ScreenPointToRay(new Vector2(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y));

        float rayDistance;
        if (plane.Raycast(ray, out rayDistance))
        {
            Vector3 cursorWorldPosition = ray.GetPoint(rayDistance);

            Plane topPlane = new Plane(terrain.transform.up, terrain.transform.position + terrain.transform.up * terrain.maxHeight);
            Plane bottomPlane = new Plane(terrain.transform.up, terrain.transform.position + terrain.transform.up * terrain.minHeight);

            Handles.color = new Color(0f, 1f, 0f);
            Handles.DrawLine(
                cursorWorldPosition - topPlane.normal * topPlane.GetDistanceToPoint(cursorWorldPosition),
                cursorWorldPosition - bottomPlane.normal * bottomPlane.GetDistanceToPoint(cursorWorldPosition)
                );

            HexGrid.Coord coord = terrain.GetCoordForWorldPosition(cursorWorldPosition);

            if (Event.current.type ==  EventType.MouseDown || Event.current.type == EventType.MouseDrag)
            {
                if (Event.current.button == 0 && !newPillarCoords.Contains(coord))
                {
                    newPillarCoords.Add(coord);
                }
                else if (Event.current.button == 1)
                {
                    currentCreationState = CreationState.None;
                }
            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                newPillarTopHeight = Mathf.Clamp(Vector3.Dot(selectedTerrain.transform.up, hexCreationHorizontalPlane), selectedTerrain.minHeight, selectedTerrain.maxHeight);

                currentCreationState = CreationState.ChooseTopHeight;
            }

            foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
            {
                Handles.color = new Color(0f, 1f, 0.75f);
                DrawHorizontalPlacementWidget(newPillarCoord, false);
            }

            Handles.color = new Color(0f, 1f, 0.5f);
            DrawHorizontalPlacementWidget(coord, true);
        }
    }

    void ChooseTopHeight(HexTerrain terrain)
    {
        if (Event.current.isMouse)
        {
            newPillarTopHeight += Event.current.delta.y / Screen.height * (selectedTerrain.maxHeight - selectedTerrain.minHeight);
            newPillarTopHeight = Mathf.Clamp(newPillarTopHeight, selectedTerrain.maxHeight, selectedTerrain.minHeight);
        }

        foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
        {
            Handles.color = new Color(0f, 1f, 0.75f);
            DrawVerticalLinesAtCoord(newPillarCoord);
            DrawRingAtCoord(newPillarCoord, newPillarTopHeight);
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            newPillarBottomHeight = newPillarTopHeight;

            currentCreationState = CreationState.ChooseBottomHeight;
        }
    }

    void ChooseBottomHeight(HexTerrain terrain)
    {
        if (Event.current.isMouse)
        {
            newPillarBottomHeight += Event.current.delta.y / Screen.height * (selectedTerrain.maxHeight - selectedTerrain.minHeight);
            newPillarBottomHeight = Mathf.Clamp(newPillarBottomHeight, selectedTerrain.maxHeight, selectedTerrain.minHeight);
        }

        foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
        {
            Handles.color = new Color(0f, 1f, 0.75f);
            DrawVerticalLinesAtCoord(newPillarCoord);
            DrawRingAtCoord(newPillarCoord, newPillarBottomHeight);
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
            {
                selectedTerrain.AddPillar(newPillarCoord, newPillarTopHeight, newPillarBottomHeight);
            }

            newPillarCoords.Clear();
            currentCreationState = CreationState.None;
        }
    }


    void DrawVerticalLinesAtCoord(HexGrid.Coord coord)
    {
        Plane topPlane = new Plane(selectedTerrain.transform.up, selectedTerrain.transform.position + selectedTerrain.transform.up * selectedTerrain.maxHeight);
        Plane bottomPlane = new Plane(selectedTerrain.transform.up, selectedTerrain.transform.position + selectedTerrain.transform.up * selectedTerrain.minHeight);

        Vector3 hexLocalPosition = selectedTerrain.GetLocalPositionForCoord(coord);

        for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
        {
            Vector3 cornerLocalPosition = hexLocalPosition + HexHelper.GetCornerDirection(corner) * selectedTerrain.hexRadius;
            Vector3 cornerWorldPosition = selectedTerrain.transform.TransformPoint(cornerLocalPosition);

            Handles.DrawLine(
                    cornerWorldPosition - topPlane.normal * topPlane.GetDistanceToPoint(cornerWorldPosition),
                    cornerWorldPosition - bottomPlane.normal * bottomPlane.GetDistanceToPoint(cornerWorldPosition));
        }
    }

    void DrawRingAtCoord(HexGrid.Coord coord, float height)
    {
        Vector3 hexLocalPosition = selectedTerrain.GetLocalPositionForCoord(coord);
        height = Mathf.Clamp(height, selectedTerrain.minHeight, selectedTerrain.maxHeight);

        // Outter ring
        for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
        {
            Vector3 corner1LocalPosition = hexLocalPosition + HexHelper.GetCornerDirection(corner) * selectedTerrain.hexRadius;
            corner1LocalPosition.y = height;
            Vector3 corner1WorldPosition = selectedTerrain.transform.TransformPoint(corner1LocalPosition);

            HexCorner nextCorner = corner + 1;
            if (nextCorner == HexCorner.MAX)
                nextCorner = 0;

            Vector3 corner2LocalPosition = hexLocalPosition + HexHelper.GetCornerDirection(nextCorner) * selectedTerrain.hexRadius;
            corner2LocalPosition.y = height;
            Vector3 corner2WorldPosition = selectedTerrain.transform.TransformPoint(corner2LocalPosition);

            Handles.DrawLine(corner1WorldPosition, corner2WorldPosition);
        }

        // Inner ring
        for (HexCorner corner = 0; corner < HexCorner.MAX; ++corner)
        {
            Vector3 corner1LocalPosition = hexLocalPosition + HexHelper.GetCornerDirection(corner) * selectedTerrain.hexRadius / 2f;
            corner1LocalPosition.y = height;
            Vector3 corner1WorldPosition = selectedTerrain.transform.TransformPoint(corner1LocalPosition);

            HexCorner nextCorner = corner + 1;
            if (nextCorner == HexCorner.MAX)
                nextCorner = 0;

            Vector3 corner2LocalPosition = hexLocalPosition + HexHelper.GetCornerDirection(nextCorner) * selectedTerrain.hexRadius / 2f;
            corner2LocalPosition.y = height;
            Vector3 corner2WorldPosition = selectedTerrain.transform.TransformPoint(corner2LocalPosition);

            Handles.DrawLine(corner1WorldPosition, corner2WorldPosition);
        }
    }

    void DrawHorizontalPlacementWidget(HexGrid.Coord coord, bool drawVerticalLines)
    {
        if (drawVerticalLines)
            DrawVerticalLinesAtCoord(coord);

        DrawRingAtCoord(coord, Vector3.Dot(selectedTerrain.transform.up, hexCreationHorizontalPlane));

        /*
        for (float height = terrain.minHeight; height <= terrain.maxHeight; height += terrain.hexRadius)
        {
            DrawRingAtCoord(terrain, coord, height);
        }
        */
    }

    public void OnCreateNewPillarsPressed()
    {
        if (currentCreationState == CreationState.None)
        {
            currentCreationState = CreationState.ChooseHorizontalPosition;
        }
    }
}
