using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(HexPillar))]
[CanEditMultipleObjects]
public class HexPillarEditor : Editor
{
    public void OnSceneGUI()
    {
        HexPillar selectedPillar = Selection.activeGameObject ? Selection.activeGameObject.GetComponent<HexPillar>() : null;

        if (!selectedPillar)
        {
            return;
        }

        Tools.hidden = true;

        TopHandle(selectedPillar);
        MiddleHandle(selectedPillar);
        BottomHandle(selectedPillar);

        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillar selection = selectedObject.GetComponent<HexPillar>();

            if (!selection)
                continue;

            for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
            {
                CornerButton(selection, direction, true);
                CornerButton(selection, direction, false);
            }
        }
    }

    void TopHandle(HexPillar selectedPillar)
    {
        float delta = Handle(
            selectedPillar,
            new Vector3(0, selectedPillar.topEnd.centerHeight, 0),
            Vector3.up);

        if (delta != 0f)
        {
            MoveTop(delta);
            RedrawSelections();
        }
    }

    void MiddleHandle(HexPillar selectedPillar)
    {
        float height = (selectedPillar.topEnd.centerHeight + selectedPillar.bottomEnd.centerHeight) / 2;

        float delta = Handle(
            selectedPillar,
            new Vector3(0, height, 0),
            Vector3.up,
            0.35f,
            Handles.SphereCap);

        if (delta != 0f)
        {
            MoveTop(delta);
            MoveBottom(-delta);
            RedrawSelections();
        }
    }

    void BottomHandle(HexPillar selectedPillar)
    {
        float delta = Handle(
            selectedPillar,
            new Vector3(0, selectedPillar.bottomEnd.centerHeight, 0),
            Vector3.down);

        if (delta != 0f)
        {
            MoveBottom(delta);
            RedrawSelections();
        }
    }

    float Handle(
        HexPillar pillar,
        Vector3 localPosition,
        Vector3 localDirection,
        float size = 1f,
        Handles.DrawCapFunction cap = null)
    {
        if (cap == null)
            cap = Handles.ArrowCap;

        Handles.color = new Color(1f, 0.75f, 0f);

        Vector3 positionBefore = pillar.transform.TransformPoint(localPosition);
        Vector3 direction = pillar.transform.TransformDirection(localDirection);

        EditorGUI.BeginChangeCheck();
        Vector3 positionAfter = Handles.Slider(positionBefore, direction, size, cap, 0f);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(pillar.topEnd, "Edit Pillar");
            Undo.RecordObject(pillar.bottomEnd, "Edit Pillar");
        }

        Vector3 positionDelta = positionAfter - positionBefore;
        return Vector3.Dot(positionDelta, direction);
    }

    void MoveTop(float amount)
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillar selectedPillar = selectedObject.GetComponent<HexPillar>();

            if (!selectedPillar)
                continue;

            selectedPillar.topEnd.centerHeight = Mathf.Max(selectedPillar.topEnd.centerHeight + amount, selectedPillar.bottomEnd.centerHeight);

            for (int i = 0; i < selectedPillar.topEnd.corners.Length; ++i)
            {
                selectedPillar.topEnd.corners[i].height = Mathf.Max(selectedPillar.topEnd.corners[i].height + amount, selectedPillar.bottomEnd.corners[i].height);
            }
            
            selectedPillar.topEnd.SnapPointsToIncrement(selectedPillar.terrain.heightSnap);
        }
    }

    void MoveBottom(float amount)
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            HexPillar selectedPillar = selectedObject.GetComponent<HexPillar>();

            if (!selectedPillar)
                continue;

            selectedPillar.bottomEnd.centerHeight = Mathf.Min(selectedPillar.bottomEnd.centerHeight - amount, selectedPillar.topEnd.centerHeight);

            for (int i = 0; i < selectedPillar.bottomEnd.corners.Length; ++i)
            {
                selectedPillar.bottomEnd.corners[i].height = Mathf.Min(selectedPillar.bottomEnd.corners[i].height - amount, selectedPillar.topEnd.corners[i].height);
            }

            selectedPillar.bottomEnd.SnapPointsToIncrement(selectedPillar.terrain.heightSnap);
        }
    }

    void CornerButton(HexPillar pillar, HexCornerDirection cornerDirection, bool topCorner)
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
                    if (selectedObject.GetComponent<HexPillarCorner>())
                    {
                        selectedObjects.Remove(selectedObject);
                    }
                }

                selectedObjects.Insert(0, cornerObject.gameObject);
            }

            Selection.objects = selectedObjects.ToArray();
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