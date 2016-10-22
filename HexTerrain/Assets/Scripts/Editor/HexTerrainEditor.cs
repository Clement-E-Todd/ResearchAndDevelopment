using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameObject))]
[CanEditMultipleObjects]
[ExecuteInEditMode]
public class HexTerrainEditor : Editor
{
    enum SelectionMode
    {
        Pillars,
        Vertices,
        MAX
    }
    static SelectionMode selectionMode = SelectionMode.Pillars;

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
            {
                switch (selectionMode)
                {
                    case SelectionMode.Pillars:
                        HexPillarEditor.OnSelectionModePillars();
                        break;
                        
                    case SelectionMode.Vertices:
                        HexPillarEditor.OnSelectionModeVertices();
                        break;
                }
            }

            if (selectedEnds.Length > 0)
            {
                switch (selectionMode)
                {
                    case SelectionMode.Pillars:
                        HexPillarEndEditor.OnSelectionModePillars();
                        break;

                    case SelectionMode.Vertices:
                        HexPillarEndEditor.OnSelectionModeVertices();
                        break;
                }
            }

            if (selectedCorners.Length > 0 && selectionMode == SelectionMode.Vertices)
                HexPillarCornerEditor.OnSelectionModeVertices();

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

        foreach (GameObject selection in Selection.gameObjects)
        {
            if (selection.GetComponent<HexTerrain>() && (!selectedTerrain || Selection.activeTransform == selection))
            {
                selectedTerrain = selection.GetComponent<HexTerrain>();
            }

            if (selection.GetComponent<HexPillar>())
            {
                selectedTerrain = selection.GetComponent<HexPillar>().GetTerrain();
                newSelectedPillars.Add(selection.GetComponent<HexPillar>());

                if (selectionMode == SelectionMode.Pillars)
                {
                    if (!newSelectedEnds.Contains(selection.GetComponent<HexPillar>().topEnd))
                        newSelectedEnds.Add(selection.GetComponent<HexPillar>().topEnd);

                    if (!newSelectedEnds.Contains(selection.GetComponent<HexPillar>().bottomEnd))
                        newSelectedEnds.Add(selection.GetComponent<HexPillar>().bottomEnd);
                }
            }

            if (selection.GetComponent<HexPillarEnd>())
            {
                selectedTerrain = selection.GetComponent<HexPillarEnd>().GetTerrain();
                newSelectedEnds.Add(selection.GetComponent<HexPillarEnd>());
            }

            if (selection.GetComponent<HexPillarCorner>())
            {
                selectedTerrain = selection.GetComponent<HexPillarCorner>().GetTerrain();
                newSelectedCorners.Add(selection.GetComponent<HexPillarCorner>());
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
            Screen.width * 0.05f, Screen.height * 0.825f,
            Screen.width * 0.9f, Screen.height * 0.175f));

        if (GUILayout.Button("Create New Hex Pillars"))
            HexPillarEditor.OnCreateNewPillarsPressed();

        if (GUILayout.Button("Selection Mode: " + (selectionMode == SelectionMode.Pillars ? "Pillars" : "Vertices")))
            selectionMode = (selectionMode == SelectionMode.Pillars ? SelectionMode.Vertices : SelectionMode.Pillars);

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
