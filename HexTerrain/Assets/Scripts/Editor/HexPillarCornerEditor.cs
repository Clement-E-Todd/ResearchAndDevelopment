using UnityEngine;
using UnityEditor;

public static class HexPillarCornerEditor
{
    public static void OnSelectionModeVertices()
    {
        foreach (HexPillarCorner selectedCorner in HexTerrainEditor.selectedCorners)
        {
            float delta = Handle(selectedCorner);

            if (delta != 0f)
            {
                MoveSelectedCorners(delta);
                HexTerrainEditor.RedrawSelections();
            }
        }
    }

    static float Handle(HexPillarCorner cornerObject)
    {
        Handles.color = new Color(1f, 0.25f, 0f);

        Vector3 direction = cornerObject.isOnTopEnd ? cornerObject.transform.up : -cornerObject.transform.up;

        EditorGUI.BeginChangeCheck();
        Vector3 positionAfter = Handles.Slider(
            cornerObject.transform.position,
            direction,
            0.5f,
            Handles.ArrowCap,
            0f);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (HexPillarCorner selectedCorner in HexTerrainEditor.selectedCorners)
            {
                Undo.RecordObject(selectedCorner, "Edit Pillar Corner");
            }
        }

        Vector3 positionDelta = positionAfter - cornerObject.transform.position;
        return Vector3.Dot(positionDelta, direction) * Vector3.Dot(direction, cornerObject.transform.up);
    }

    static void MoveSelectedCorners(float amount)
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillarCorner selectedCorner = selectedObject.GetComponent<HexPillarCorner>();

            if (!selectedCorner)
                continue;
            
            if (selectedCorner.isOnTopEnd)
                selectedCorner.height = Mathf.Max(selectedCorner.height + amount, selectedCorner.end.pillar.bottomEnd.corners[(int)selectedCorner.corner].height);
            else
                selectedCorner.height = Mathf.Min(selectedCorner.height + amount, selectedCorner.end.pillar.topEnd.corners[(int)selectedCorner.corner].height);

            selectedCorner.height = Mathf.Round(selectedCorner.height / selectedCorner.end.pillar.terrain.heightSnap) * selectedCorner.end.pillar.terrain.heightSnap;
        }
    }

    static void RedrawSelections()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillar selection = selectedObject.GetComponent<HexPillar>();

            if (!selection)
                continue;

            selection.GenerateMesh();
        }
    }

    public static bool HideUnityTools()
    {
        return HexTerrainEditor.selectedCorners.Length > 0;
    }

    public static bool HideTerrainEditorControls()
    {
        return false;
    }
}