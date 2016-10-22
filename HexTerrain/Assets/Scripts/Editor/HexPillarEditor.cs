using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public static class HexPillarEditor
{
    enum PillarCreationState
    {
        None,
        ChooseHorizontalPositions,
        ChooseTopHeight,
        ChooseBottomHeight
    }
    static PillarCreationState pillarCreationState = PillarCreationState.None;

    static Vector3 createPlanePoint;
    const float distanceToHorizonalPlane = 10f;

    static List<HexGrid.Coord> newPillarCoords = new List<HexGrid.Coord>();
    static float newPillarTopHeight;
    static float newPillarBottomHeight;

    static float totalMouseDelta;

    public static void OnSelectionModePillars()
    {
        foreach (HexPillar selectedPillar in HexTerrainEditor.selectedPillars)
        {
            float delta = Handle(selectedPillar);

            if (delta != 0f)
            {
                HexPillarEndEditor.MoveEndByAmount(selectedPillar.topEnd, delta, true);
                HexPillarEndEditor.MoveEndByAmount(selectedPillar.bottomEnd, delta, true);
                HexTerrainEditor.RedrawSelections();
            }
        }
    }

    public static void OnSelectionModeVertices()
    {
        foreach (HexPillar selectedPillar in HexTerrainEditor.selectedPillars)
        {
            for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
            {
                CornerButton(selectedPillar, direction, true);
                CornerButton(selectedPillar, direction, false);
            }

            EndButton(selectedPillar, true);
            EndButton(selectedPillar, false);
        }
    }

    public static void CancelPillarCreation()
    {
        pillarCreationState = PillarCreationState.None;
    }

    static float Handle(HexPillar pillar)
    {
        Vector3 positionBefore = pillar.transform.TransformPoint((pillar.topEnd.transform.localPosition + pillar.bottomEnd.transform.localPosition) / 2);
        Vector3 direction = pillar.transform.TransformDirection(pillar.transform.up);

        Handles.color = new Color(1f, 0.75f, 0f);

        EditorGUI.BeginChangeCheck();
        Vector3 positionAfter = Handles.Slider(positionBefore, direction, 0.35f, Handles.SphereCap, 0f);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
            {
                Undo.RecordObject(selectedEnd, "Edit Pillar");
            }
        }

        Vector3 positionDelta = positionAfter - positionBefore;
        return Vector3.Dot(positionDelta, direction);
    }

    static void EndButton(HexPillar pillar, bool topEnd)
    {
        HexPillarEnd endObject = topEnd ? pillar.topEnd : pillar.bottomEnd;
        List<Object> selectedObjects = Selection.objects.ToList();

        if (selectedObjects.Contains(endObject.gameObject))
            Handles.color = new Color(0f, 1.0f, 0.25f);
        else
            Handles.color = new Color(0f, 0.5f, 0.125f);

        if (Handles.Button(
            endObject.transform.position,
            Quaternion.identity,
            0.05f, 0.05f,
            Handles.DotCap))
        {
            if (Event.current.shift)
            {
                if (selectedObjects.Contains(endObject.gameObject))
                {
                    selectedObjects.Remove(endObject.gameObject);
                }
                else
                {
                    selectedObjects.Insert(0, endObject.gameObject);
                }
            }
            else
            {
                foreach (GameObject selectedObject in Selection.objects)
                {
                    if (selectedObject.GetComponent<HexPillarCorner>() ||
                        selectedObject.GetComponent<HexPillarEnd>())
                    {
                        selectedObjects.Remove(selectedObject);
                    }
                }

                selectedObjects.Insert(0, endObject.gameObject);
            }

            Selection.objects = selectedObjects.ToArray();
        }
    }

    static void CornerButton(HexPillar pillar, HexCornerDirection cornerDirection, bool topCorner)
    {
        HexPillarCorner cornerObject = (topCorner ? pillar.topEnd.corners : pillar.bottomEnd.corners)[(int)cornerDirection];
        List<Object> selectedObjects = Selection.objects.ToList();

        if (selectedObjects.Contains(cornerObject.gameObject))
            Handles.color = new Color(1.0f, 0.25f, 0f);
        else
            Handles.color = new Color(0.5f, 0.125f, 0f);

        if (Handles.Button(
            cornerObject.transform.position,
            Quaternion.identity,
            0.05f, 0.05f,
            Handles.DotCap))
        {
            if (Event.current.shift)
            {
                if (selectedObjects.Contains(cornerObject.gameObject))
                {
                    selectedObjects.Remove(cornerObject.gameObject);
                }
                else
                {
                    selectedObjects.Insert(0, cornerObject.gameObject);
                }
            }
            else
            {
                foreach (GameObject selectedObject in Selection.objects)
                {
                    if (selectedObject.GetComponent<HexPillarCorner>() ||
                        selectedObject.GetComponent<HexPillarEnd>())
                    {
                        selectedObjects.Remove(selectedObject);
                    }
                }

                selectedObjects.Insert(0, cornerObject.gameObject);
            }

            Selection.objects = selectedObjects.ToArray();
        }
    }

    public static void UpdatePillarCreationState()
    {
        if (!HexTerrainEditor.selectedTerrain)
        {
            return;
        }

        switch (pillarCreationState)
        {
            case PillarCreationState.ChooseHorizontalPositions:
                ChooseHorizontalPositionsForNewPillars(HexTerrainEditor.selectedTerrain);
                break;

            case PillarCreationState.ChooseTopHeight:
                ChooseTopHeightForNewPillars(HexTerrainEditor.selectedTerrain);
                break;

            case PillarCreationState.ChooseBottomHeight:
                ChooseBottomHeightForNewPillars(HexTerrainEditor.selectedTerrain);
                break;
        }
    }

    static void ChooseHorizontalPositionsForNewPillars(HexTerrain terrain)
    {
        if (!Camera.current)
        {
            pillarCreationState = PillarCreationState.None;
            return;
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        createPlanePoint = Camera.current.transform.position + Camera.current.transform.forward * distanceToHorizonalPlane;

        Plane plane = new Plane(terrain.transform.up, createPlanePoint);
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

            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
            {
                if (Event.current.button == 0 && !newPillarCoords.Contains(coord))
                {
                    newPillarCoords.Add(coord);
                }
                else if (Event.current.button == 1)
                {
                    pillarCreationState = PillarCreationState.None;
                }
            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                pillarCreationState = PillarCreationState.ChooseTopHeight;
                totalMouseDelta = 0f;
            }

            Handles.color = new Color(0f, 1f, 0.75f);
            foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
            {
                DrawRingAtCoord(terrain, coord, Vector3.Dot(terrain.transform.up, createPlanePoint), true);
            }

            DrawVerticalLinesAtCoord(terrain, coord);
            DrawRingAtCoord(terrain, coord, Vector3.Dot(terrain.transform.up, createPlanePoint), false);
        }
    }

    static void ChooseTopHeightForNewPillars(HexTerrain terrain)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (Event.current.isMouse)
        {
            totalMouseDelta -= Event.current.delta.y;

            newPillarTopHeight = Vector3.Dot(terrain.transform.up, createPlanePoint);
            newPillarTopHeight += (totalMouseDelta / Screen.height) * (terrain.maxHeight - terrain.minHeight);
            newPillarTopHeight = Mathf.Round(newPillarTopHeight / terrain.heightSnap) * terrain.heightSnap;
            newPillarTopHeight = Mathf.Clamp(newPillarTopHeight, terrain.minHeight, terrain.maxHeight);
        }

        foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
        {
            Handles.color = new Color(0f, 1f, 0.75f);
            DrawVerticalLinesAtCoord(terrain, newPillarCoord);
            DrawRingAtCoord(terrain, newPillarCoord, newPillarTopHeight, true);
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            newPillarBottomHeight = newPillarTopHeight;

            pillarCreationState = PillarCreationState.ChooseBottomHeight;
            totalMouseDelta = 0f;
        }
    }

    static void ChooseBottomHeightForNewPillars(HexTerrain terrain)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (Event.current.isMouse)
        {
            totalMouseDelta -= Event.current.delta.y;

            newPillarBottomHeight = newPillarTopHeight;
            newPillarBottomHeight += (totalMouseDelta / Screen.height) * (terrain.maxHeight - terrain.minHeight);
            newPillarBottomHeight = Mathf.Round(newPillarBottomHeight / terrain.heightSnap) * terrain.heightSnap;
            newPillarBottomHeight = Mathf.Clamp(newPillarBottomHeight, terrain.minHeight, newPillarTopHeight);
        }

        foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
        {
            Handles.color = new Color(0f, 1f, 0.75f);
            DrawVerticalLinesAtCoord(terrain, newPillarCoord);
            DrawRingAtCoord(terrain, newPillarCoord, newPillarTopHeight, false);
            DrawRingAtCoord(terrain, newPillarCoord, newPillarBottomHeight, true);
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            foreach (HexGrid.Coord newPillarCoord in newPillarCoords)
            {
                HexPillar newPillar = terrain.AddPillar(newPillarCoord, newPillarTopHeight, newPillarBottomHeight);
                Undo.RegisterCreatedObjectUndo(newPillar.gameObject, "Create Pillar");
            }

            newPillarCoords.Clear();
            pillarCreationState = PillarCreationState.None;
        }
    }


    static void DrawVerticalLinesAtCoord(HexTerrain terrain, HexGrid.Coord coord)
    {
        Plane topPlane = new Plane(terrain.transform.up, terrain.transform.position + terrain.transform.up * terrain.maxHeight);
        Plane bottomPlane = new Plane(terrain.transform.up, terrain.transform.position + terrain.transform.up * terrain.minHeight);

        Vector3 hexLocalPosition = terrain.GetLocalPositionForCoord(coord);

        for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
        {
            Vector3 cornerLocalPosition = hexLocalPosition + HexHelper.GetCornerDirectionVector(direction) * terrain.hexRadius;
            Vector3 cornerWorldPosition = terrain.transform.TransformPoint(cornerLocalPosition);

            Handles.DrawLine(
                    cornerWorldPosition - topPlane.normal * topPlane.GetDistanceToPoint(cornerWorldPosition),
                    cornerWorldPosition - bottomPlane.normal * bottomPlane.GetDistanceToPoint(cornerWorldPosition));
        }
    }

    static void DrawRingAtCoord(HexTerrain terrain, HexGrid.Coord coord, float height, bool innerRing)
    {
        Vector3 hexLocalPosition = terrain.GetLocalPositionForCoord(coord);
        height = Mathf.Clamp(height, terrain.minHeight, terrain.maxHeight);

        // Outter ring
        for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
        {
            Vector3 corner1LocalPosition = hexLocalPosition + HexHelper.GetCornerDirectionVector(direction) * terrain.hexRadius;
            corner1LocalPosition.y = height;
            Vector3 corner1WorldPosition = terrain.transform.TransformPoint(corner1LocalPosition);

            HexCornerDirection nextDirection = direction + 1;
            if (nextDirection == HexCornerDirection.MAX)
                nextDirection = 0;

            Vector3 corner2LocalPosition = hexLocalPosition + HexHelper.GetCornerDirectionVector(nextDirection) * terrain.hexRadius;
            corner2LocalPosition.y = height;
            Vector3 corner2WorldPosition = terrain.transform.TransformPoint(corner2LocalPosition);

            Handles.DrawLine(corner1WorldPosition, corner2WorldPosition);
        }

        // Inner ring
        if (innerRing)
        {
            for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
            {
                Vector3 corner1LocalPosition = hexLocalPosition + HexHelper.GetCornerDirectionVector(direction) * terrain.hexRadius / 2f;
                corner1LocalPosition.y = height;
                Vector3 corner1WorldPosition = terrain.transform.TransformPoint(corner1LocalPosition);

                HexCornerDirection nextDirection = direction + 1;
                if (nextDirection == HexCornerDirection.MAX)
                    nextDirection = 0;

                Vector3 corner2LocalPosition = hexLocalPosition + HexHelper.GetCornerDirectionVector(nextDirection) * terrain.hexRadius / 2f;
                corner2LocalPosition.y = height;
                Vector3 corner2WorldPosition = terrain.transform.TransformPoint(corner2LocalPosition);

                Handles.DrawLine(corner1WorldPosition, corner2WorldPosition);
            }
        }
    }

    public static void OnCreateNewPillarsPressed()
    {
        if (pillarCreationState == PillarCreationState.None)
        {
            pillarCreationState = PillarCreationState.ChooseHorizontalPositions;
        }
    }

    public static bool HideUnityTools()
    {
        return HexTerrainEditor.selectedPillars.Length > 0 || pillarCreationState != PillarCreationState.None;
    }

    public static bool HideTerrainEditorControls()
    {
        return pillarCreationState != PillarCreationState.None;
    }
}