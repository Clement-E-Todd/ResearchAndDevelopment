using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(HexTerrainEditable))]
public class HexTerrainEditor : Editor
{
    const float distanceToHorizonalPlane = 10f;

    enum State
    {
        None,
        ChooseHorizontalPosition,
        ChooseTopHeight,
        ChooseBottomHeight
    }
    State currentState = State.None;

    public void OnSceneGUI()
    {
        HexTerrainEditable selectedTerrain = Selection.activeGameObject ? Selection.activeGameObject.GetComponent<HexTerrainEditable>() : null;

        if (!selectedTerrain)
        {
            return;
        }

        switch (currentState)
        {
            case State.None:
                None(selectedTerrain);
                break;

            case State.ChooseHorizontalPosition:
                ChooseHorizontalPosition(selectedTerrain);
                break;

            case State.ChooseTopHeight:
                ChooseTopHeight(selectedTerrain);
                break;

            case State.ChooseBottomHeight:
                ChooseBottomHeight(selectedTerrain);
                break;
        }

        HandleUtility.Repaint();
    }

    void None(HexTerrainEditable terrain)
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(
            Screen.width * 0.05f, Screen.height * 0.875f,
            Screen.width * 0.9f, Screen.height * 0.125f));

        if (GUILayout.Button("Create New Hex Pillars"))
            currentState = State.ChooseHorizontalPosition;

        GUILayout.EndArea();
        Handles.EndGUI();
    }

    void ChooseHorizontalPosition(HexTerrainEditable terrain)
    {
        if (!Camera.current)
            return;

        Vector3 planeTargetPosition = Camera.current.transform.position + Camera.current.transform.forward * distanceToHorizonalPlane;

        Plane plane = new Plane(terrain.transform.up, planeTargetPosition);
        Ray ray = Camera.current.ScreenPointToRay(new Vector2(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y));

        float rayDistance;
        if (plane.Raycast(ray, out rayDistance))
        {
            Vector3 cursorWorldPosition = ray.GetPoint(rayDistance);

            Plane topPlane = new Plane(terrain.transform.up, terrain.transform.position + terrain.transform.up * terrain.maxHeight);
            Plane bottomPlane = new Plane(terrain.transform.up, terrain.transform.position + terrain.transform.up * terrain.minHeight);

            Handles.DrawLine(
                cursorWorldPosition - topPlane.normal * topPlane.GetDistanceToPoint(cursorWorldPosition),
                cursorWorldPosition - bottomPlane.normal * bottomPlane.GetDistanceToPoint(cursorWorldPosition)
                );

            HexGrid.Coord coord = terrain.GetCoordForWorldPosition(cursorWorldPosition);
        }
    }

    void ChooseTopHeight(HexTerrainEditable terrain)
    {
        currentState = State.ChooseBottomHeight;
    }

    void ChooseBottomHeight(HexTerrainEditable terrain)
    {
        currentState = State.None;
    }
}
