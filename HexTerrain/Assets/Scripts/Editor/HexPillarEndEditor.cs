using UnityEngine;
using UnityEditor;

public static class HexPillarEndEditor
{
    public static void OnSelectionModePillars()
    {
        foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
        {
            if (!HexTerrainEditor.xRayMode)
            {
                // With X-Ray Mode off, we must ensure that we're allowed to see this point before continuing.
                Vector3 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
                Vector3 direction = (selectedEnd.transform.position - cameraPosition).normalized;

                RaycastHit hit;
                if (Physics.Raycast(cameraPosition, direction, out hit, Vector3.Distance(cameraPosition, selectedEnd.transform.position) - 0.01f))
                {
                    continue;
                }
            }

            float delta = Handle(selectedEnd, 1f, new Color(1f, 0.75f, 0f));

            if (delta != 0f)
            {
                if (selectedEnd.isTopEnd)
                    MoveSelectedTopsByAmount(delta, true);
                else
                    MoveSelectedBottomsByAmount(-delta, true);

                HexTerrainEditor.RedrawSelections();
            }
        }
    }

    public static void OnSelectionModeVertices()
    {
        foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
        {
            float delta = Handle(selectedEnd, 0.5f, new Color(0f, 1f, 0.25f));

            if (delta != 0f)
            {
                if (selectedEnd.isTopEnd)
                    MoveSelectedTopsByAmount(delta, false);
                else
                    MoveSelectedBottomsByAmount(-delta, false);

                HexTerrainEditor.RedrawSelections();
            }
        }
    }

    static float Handle(HexPillarEnd end, float size, Color color)
    {
        Handles.color = color;

        Vector3 positionBefore = end.transform.TransformPoint(end.transform.localPosition - new Vector3(0f, end.centerHeight, 0f));
        Vector3 direction = end.transform.TransformDirection(end.isTopEnd ? end.transform.up : -end.transform.up);

        EditorGUI.BeginChangeCheck();
        Vector3 positionAfter = Handles.Slider(positionBefore, direction, size, Handles.ArrowCap, 0f);
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

    public static void MoveEndByAmount(HexPillarEnd end, float amount, bool moveAllCorners)
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

    public static void MoveSelectedTopsByAmount(float amount, bool moveAllCorners)
    {
        foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
        {
            if (selectedEnd.isTopEnd)
                MoveEndByAmount(selectedEnd, amount, moveAllCorners);
        }
    }

    public static void MoveSelectedBottomsByAmount(float amount, bool moveAllCorners)
    {
        foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
        {
            if (!selectedEnd.isTopEnd)
                MoveEndByAmount(selectedEnd, amount, moveAllCorners);
        }
    }

    public static bool HideUnityTools()
    {
        return HexTerrainEditor.selectedEnds != null && HexTerrainEditor.selectedEnds.Length > 0;
    }

    public static bool HideTerrainEditorControls()
    {
        return false;
    }
}