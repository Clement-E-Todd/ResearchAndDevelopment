using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameObject))]
[CanEditMultipleObjects]
[ExecuteInEditMode]
public class HexTerrainEditor : Editor
{
    public static HexTerrain selectedTerrain { get; private set; }
    public static HexPillar[] selectedPillars { get; private set; }
    public static HexPillarEnd[] selectedEnds { get; private set; }
    public static HexPillarCorner[] selectedCorners { get; private set; }

    bool terrainElementsSelected = false;
    public bool editorControlsVisible = true;

    public void OnSceneGUI()
    {
        UpdateSelections();

        if (terrainElementsSelected)
        {
            HexPillarEditor.UpdatePillarCreationState();

            Tools.hidden = HexPillarEditor.HideUnityTools() || HexPillarEndEditor.HideUnityTools() || HexPillarCornerEditor.HideUnityTools();
            
            if (!HexPillarEditor.HideTerrainEditorControls() &&
                !HexPillarEndEditor.HideTerrainEditorControls() &&
                !HexPillarCornerEditor.HideTerrainEditorControls())
            {
                ShowTerrainEditorControls();
            }

            if (selectedPillars.Length > 0)
                HexPillarEditor.OnSceneGUI();

            if (selectedEnds.Length > 0)
                HexPillarEndEditor.OnSceneGUI();

            if (selectedCorners.Length > 0)
                HexPillarCornerEditor.OnSceneGUI();

            HandleUtility.Repaint();
        }
        else if (Tools.hidden)
        {
            Tools.hidden = false;
        }
    }

    void UpdateSelections()
    {
        selectedTerrain = null;
        List<HexPillar> newSelectedPillars = new List<HexPillar>();
        List<HexPillarEnd> newSelectedEnds = new List<HexPillarEnd>();
        List<HexPillarCorner> newSelectedCorners = new List<HexPillarCorner>();

        foreach (Transform transform in Selection.transforms)
        {
            if (transform.GetComponent<HexTerrain>() && (!selectedTerrain || Selection.activeTransform == transform))
            {
                selectedTerrain = transform.GetComponent<HexTerrain>();
            }

            if (transform.GetComponent<HexPillar>())
            {
                selectedTerrain = transform.GetComponent<HexPillar>().GetTerrain();
                newSelectedPillars.Add(transform.GetComponent<HexPillar>());
            }

            if (transform.GetComponent<HexPillarEnd>())
            {
                selectedTerrain = transform.GetComponent<HexPillarEnd>().GetTerrain();
                newSelectedEnds.Add(transform.GetComponent<HexPillarEnd>());
            }

            if (transform.GetComponent<HexPillarCorner>())
            {
                selectedTerrain = transform.GetComponent<HexPillarCorner>().GetTerrain();
                newSelectedCorners.Add(transform.GetComponent<HexPillarCorner>());
            }
        }

        if (selectedTerrain)
        {
            terrainElementsSelected = true;

            selectedPillars = newSelectedPillars.ToArray();
            selectedEnds = newSelectedEnds.ToArray();
            selectedCorners = newSelectedCorners.ToArray();
        }
        else if (terrainElementsSelected)
        {
            OnTerrainElementsDeselected();
            terrainElementsSelected = false;
        }
    }

    void ShowTerrainEditorControls()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(
            Screen.width * 0.05f, Screen.height * 0.875f,
            Screen.width * 0.9f, Screen.height * 0.125f));

        if (GUILayout.Button("Create New Hex Pillars"))
            HexPillarEditor.OnCreateNewPillarsPressed();

        GUILayout.EndArea();
        Handles.EndGUI();
    }

    void OnTerrainElementsDeselected()
    {
        HexPillarEditor.CancelPillarCreation();
        Tools.hidden = false;
    }

    public static void RedrawSelections()
    {
        List<HexPillar> pillarsToRedraw = new List<HexPillar>();

        foreach (Transform selectedTransform in Selection.transforms)
        {
            if (selectedTransform.GetComponent<HexPillar>() && !pillarsToRedraw.Contains(selectedTransform.GetComponent<HexPillar>()))
                pillarsToRedraw.Add(selectedTransform.GetComponent<HexPillar>());

            else if (selectedTransform.GetComponent<HexPillarEnd>() && !pillarsToRedraw.Contains(selectedTransform.GetComponent<HexPillarEnd>().pillar))
                pillarsToRedraw.Add(selectedTransform.GetComponent<HexPillarEnd>().pillar);

            else if (selectedTransform.GetComponent<HexPillarCorner>() && !pillarsToRedraw.Contains(selectedTransform.GetComponent<HexPillarCorner>().end.pillar))
                pillarsToRedraw.Add(selectedTransform.GetComponent<HexPillarCorner>().end.pillar);
        }

        foreach (HexPillar selectedPillar in pillarsToRedraw)
        {
            selectedPillar.GenerateMesh();
        }
    }
}
