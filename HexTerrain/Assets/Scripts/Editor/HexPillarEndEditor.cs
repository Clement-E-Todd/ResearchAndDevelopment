using UnityEngine;
using UnityEditor;

public static class HexPillarEndEditor
{
    public static void OnSceneGUI()
    {
        Handles.color = new Color(1f, 0.75f, 0f);
        foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
        {
            float delta = Handle(selectedEnd);

            if (delta != 0f)
            {
                if (selectedEnd.isTopEnd)
                    MoveSelectedTopsByAmount(delta);
                else
                    MoveSelectedBottomsByAmount(-delta);

                HexTerrainEditor.RedrawSelections();
            }
        }
    }

    static float Handle(HexPillarEnd end)
    {
        Vector3 positionBefore = end.transform.TransformPoint(end.transform.localPosition - new Vector3(0f, end.centerHeight, 0f));
        Vector3 direction = end.transform.TransformDirection(end.isTopEnd ? end.transform.up : -end.transform.up);

        EditorGUI.BeginChangeCheck();
        Vector3 positionAfter = Handles.Slider(positionBefore, direction, 1f, Handles.ArrowCap, 0f);
        if (EditorGUI.EndChangeCheck())
        {
            foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
            {
                Undo.RecordObject(selectedEnd, "Edit Pillar Corner");
            }
        }

        Vector3 positionDelta = positionAfter - positionBefore;
        return Vector3.Dot(positionDelta, direction);
    }

    public static void MoveEndByAmount(HexPillarEnd end, float amount)
    {
        float amountForThisEnd = end.isTopEnd ? amount : -amount;

        end.centerHeight += amount;

        if (end.isTopEnd)
            end.centerHeight = Mathf.Max(end.centerHeight, end.GetOtherEnd().centerHeight);
        else
            end.centerHeight = Mathf.Min(end.centerHeight, end.GetOtherEnd().centerHeight);

        for (int i = 0; i < end.corners.Length; ++i)
        {
            end.corners[i].height += amount;

            if (end.isTopEnd)
                end.corners[i].height = Mathf.Max(end.corners[i].height, end.GetOtherEnd().corners[i].height);
            else
                end.corners[i].height = Mathf.Min(end.corners[i].height, end.GetOtherEnd().corners[i].height);
        }

        end.SnapPointsToIncrement(end.GetTerrain().heightSnap);
    }

    public static void MoveSelectedTopsByAmount(float amount)
    {
        foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
        {
            if (selectedEnd.isTopEnd)
                MoveEndByAmount(selectedEnd, amount);
        }
    }

    public static void MoveSelectedBottomsByAmount(float amount)
    {
        foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
        {
            if (!selectedEnd.isTopEnd)
                MoveEndByAmount(selectedEnd, amount);
        }
    }

    public static bool HideUnityTools()
    {
        return HexTerrainEditor.selectedEnds.Length > 0;
    }

    public static bool HideTerrainEditorControls()
    {
        return false;
    }
}