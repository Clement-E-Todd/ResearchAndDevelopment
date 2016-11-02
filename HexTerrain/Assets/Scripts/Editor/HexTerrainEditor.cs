namespace HexTerrain
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    [CustomEditor(typeof(GameObject))]
    [CanEditMultipleObjects]
    [ExecuteInEditMode]
    public class HexTerrainEditor : Editor
    {
        enum Mode
        {
            Pillars,
            Vertices,
            Materials
        }
        static Mode mode = Mode.Pillars;

        public static HexTerrain selectedTerrain { get; private set; }
        public static List<HexPillar> selectedPillars { get; private set; }
        public static List<HexPillarEnd> selectedEnds { get; private set; }
        public static List<HexPillarCorner> selectedCorners { get; private set; }

        public static bool xRayMode { get; private set; }

        bool terrainElementsSelected = false;
        public bool editorControlsVisible = true;

        public void OnSceneGUI()
        {
            HexPillarMaterialTool.DebugLog();

            if (terrainElementsSelected)
            {
                FindMissingPillars();

                HexPillarCreationTool.UpdatePillarCreationState();

                Tools.hidden = (
                    HexPillarEditor.ShouldHideUnityTools() ||
                    HexPillarEndEditor.ShouldHideUnityTools() ||
                    HexPillarCornerEditor.ShouldHideUnityTools());

                if (!HexPillarCreationTool.creationInProgress)
                {
                    ShowTerrainEditorControls();
                }

                if (selectedPillars != null && selectedPillars.Count > 0)
                {
                    switch (mode)
                    {
                        case Mode.Pillars:
                            HexPillarEditor.OnSelectionModePillars();
                            HexPillarEndEditor.OnSelectionModePillars();
                            break;

                        case Mode.Vertices:
                            HexPillarEditor.OnSelectionModeVertices();
                            break;
                    }
                }

                if (selectedEnds != null && selectedEnds.Count > 0 && mode == Mode.Vertices)
                    HexPillarEndEditor.OnSelectionModeVertices();

                if (selectedCorners != null && selectedCorners.Count > 0 && mode == Mode.Vertices)
                    HexPillarCornerEditor.OnSelectionModeVertices();

                HandleUtility.Repaint();
            }
            else if (Tools.hidden == true)
            {
                Tools.hidden = false;
            }

            UpdateSelections();
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

                selectedPillars = newSelectedPillars;
                selectedEnds = newSelectedEnds;
                selectedCorners = newSelectedCorners;

                HexPillarCornerEditor.UpdateOverlappingSelections();
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

            string buttonText = string.Format(mode == Mode.Pillars ? "> {0} <" : "{0}", "EDIT PILLARS");
            if (GUI.Button(GetEditorControlButtonRect(0, 3, 0, 2), buttonText))
                mode = Mode.Pillars;

            buttonText = string.Format(mode == Mode.Vertices ? "> {0} <" : "{0}", "EDIT VERTICES");
            if (GUI.Button(GetEditorControlButtonRect(1, 3, 0, 2), buttonText))
                mode = Mode.Vertices;

            buttonText = string.Format(mode == Mode.Materials ? "> {0} <" : "{0}", "EDIT MATERIALS");
            if (GUI.Button(GetEditorControlButtonRect(2, 3, 0, 2), buttonText))
                mode = Mode.Materials;

            switch (mode)
            {
                case Mode.Pillars:
                    {
                        buttonText = "Create New Pillars";
                        if (GUI.Button(GetEditorControlButtonRect(0, 2, 1, 2), buttonText))
                            HexPillarCreationTool.OnCreateNewPillarsPressed();

                        buttonText = "Duplicate Selected";
                        if (GUI.Button(GetEditorControlButtonRect(1, 2, 1, 2), buttonText))
                            Debug.LogError("\"Duplicate Selected\" not implemented yet.");

                        break;
                    }

                case Mode.Vertices:
                    {
                        buttonText = "X-Ray Mode: " + (xRayMode ? "ON" : "OFF");
                        if (GUI.Button(GetEditorControlButtonRect(0, 3, 1, 2), buttonText))
                            xRayMode = !xRayMode;

                        buttonText = "Hide Selected Edges";
                        if (GUI.Button(GetEditorControlButtonRect(1, 3, 1, 2), buttonText))
                            HexPillarEditor.HideSelectedEdges(true);

                        buttonText = "Show Selected Edges";
                        if (GUI.Button(GetEditorControlButtonRect(2, 3, 1, 2), buttonText))
                            HexPillarEditor.HideSelectedEdges(false);

                        break;
                    }

                case Mode.Materials:
                    {
                        buttonText = "PAINT FLOORS: " + (true ? "ON" : "OFF"); // TODO: Don't hard-code "ON"
                        if (GUI.Button(GetEditorControlButtonRect(0, 4, 1, 2), buttonText))
                            xRayMode = !xRayMode;

                        buttonText = "PAINT SIDES: " + (true ? "ON" : "OFF"); // TODO: Don't hard-code "ON"
                        if (GUI.Button(GetEditorControlButtonRect(1, 4, 1, 2), buttonText))
                            HexPillarEditor.HideSelectedEdges(true);

                        buttonText = "PAINT CEILINGS: " + (true ? "ON" : "OFF"); // TODO: Don't hard-code "ON"
                        if (GUI.Button(GetEditorControlButtonRect(2, 4, 1, 2), buttonText))
                            HexPillarEditor.HideSelectedEdges(false);

                        break;
                    }
            }

            Handles.EndGUI();
        }

        void OnTerrainElementsDeselected()
        {
            HexPillarCreationTool.CancelPillarCreation();
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

        static Rect GetEditorControlButtonRect(int buttonIndex, int maxButtonsInRow, int rowIndex, int maxRows)
        {
            if (buttonIndex < 0 || buttonIndex >= maxButtonsInRow)
            {
                Debug.LogWarning("Button index out of range for row.");
                return new Rect();
            }

            float padding = Screen.width * 0.025f;
            
            Vector2 buttonSize = new Vector2(
                (Screen.width - padding * (maxButtonsInRow + 1)) / maxButtonsInRow,
                Screen.height * 0.05f);

            float buttonDistanceY = buttonSize.y + padding;

            Vector2 buttonPosition = new Vector2(
                padding + (buttonSize.x + padding) * buttonIndex,
                Screen.height - (padding * 2) - (buttonDistanceY * maxRows) + (buttonDistanceY * rowIndex));

            return new Rect(buttonPosition, buttonSize);
        }

        public static bool IsPointObscured(Vector3 point)
        {
            Vector3 cameraPosition = SceneView.currentDrawingSceneView.camera.transform.position;
            Vector3 direction = (point - cameraPosition).normalized;
            return Physics.Raycast(cameraPosition, direction, Vector3.Distance(cameraPosition, point) - 0.01f);
        }

        void FindMissingPillars()
        {
            if (selectedTerrain && selectedTerrain.pillarGrid.Count != selectedTerrain.transform.childCount)
            {
                selectedTerrain.pillarGrid.Clear();

                for (int i = 0; i < selectedTerrain.transform.childCount; ++i)
                {
                    if (selectedTerrain.transform.GetChild(i).GetComponent<HexPillar>())
                    {
                        selectedTerrain.AddExistingPillar(selectedTerrain.transform.GetChild(i).GetComponent<HexPillar>());
                    }
                }
            }
        }
    }
}