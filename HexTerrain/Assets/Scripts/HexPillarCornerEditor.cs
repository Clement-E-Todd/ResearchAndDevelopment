using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexPillarCornerEditable))]
[CanEditMultipleObjects]
public class HexPillarCornerEditor : Editor
{
    public void OnSceneGUI()
    {
        bool redraw = false;

        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillarCornerEditable selectedCorner = selectedObject.GetComponent<HexPillarCornerEditable>();

            if (!selectedCorner)
                continue;

            float delta = Handle(selectedCorner);

            if (delta != 0f)
            {
                MoveSelectedCorners(delta);
                redraw = true;
            }
        }

        if (redraw)
        {
            RedrawSelections();
        }
    }

    float Handle(HexPillarCornerEditable cornerObject)
    {
        Handles.color = new Color(1.0f, 0.25f, 0f);

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
            Undo.RecordObject(cornerObject.owner.pillarInfo.topEnd, "Edit Pillar Corner");
            Undo.RecordObject(cornerObject.owner.pillarInfo.bottomEnd, "Edit Pillar Corner");
        }

        Vector3 positionDelta = positionAfter - cornerObject.transform.position;
        return Vector3.Dot(positionDelta, direction) * Vector3.Dot(direction, cornerObject.transform.up);
    }

    void MoveSelectedCorners(float amount)
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillarCornerEditable selectedCorner = selectedObject.GetComponent<HexPillarCornerEditable>();

            if (!selectedCorner)
                continue;
            
            if (selectedCorner.isOnTopEnd)
                selectedCorner.owner.pillarInfo.topEnd.cornerHeights[(int)selectedCorner.corner] = Mathf.Max(
                    selectedCorner.owner.pillarInfo.topEnd.cornerHeights[(int)selectedCorner.corner] + amount,
                    selectedCorner.owner.pillarInfo.bottomEnd.cornerHeights[(int)selectedCorner.corner]);
            else
                selectedCorner.owner.pillarInfo.bottomEnd.cornerHeights[(int)selectedCorner.corner] = Mathf.Min(
                    selectedCorner.owner.pillarInfo.bottomEnd.cornerHeights[(int)selectedCorner.corner] + amount,
                    selectedCorner.owner.pillarInfo.topEnd.cornerHeights[(int)selectedCorner.corner]);
        }
    }

    void RedrawSelections()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillarEditable selection = selectedObject.GetComponent<HexPillarEditable>();

            if (!selection)
                continue;

            selection.GenerateMesh();
        }
    }
}