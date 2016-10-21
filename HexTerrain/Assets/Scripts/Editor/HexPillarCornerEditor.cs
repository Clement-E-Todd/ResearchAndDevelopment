using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexPillarCorner))]
[CanEditMultipleObjects]
public class HexPillarCornerEditor : Editor
{
    public void OnSceneGUI()
    {
        bool redraw = false;

        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillarCorner selectedCorner = selectedObject.GetComponent<HexPillarCorner>();

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

    float Handle(HexPillarCorner cornerObject)
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
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                HexPillarCorner selectedCorner = selectedObject.GetComponent<HexPillarCorner>();

                if (!selectedCorner)
                    continue;

                Undo.RecordObject(selectedCorner, "Edit Pillar Corner");
            }
        }

        Vector3 positionDelta = positionAfter - cornerObject.transform.position;
        return Vector3.Dot(positionDelta, direction) * Vector3.Dot(direction, cornerObject.transform.up);
    }

    void MoveSelectedCorners(float amount)
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

    void RedrawSelections()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillar selection = selectedObject.GetComponent<HexPillar>();

            if (!selection)
                continue;

            selection.GenerateMesh();
        }
    }
}