namespace HexTerrain
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Linq;

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

        public static void SelectionButton(HexPillar pillar, HexCornerDirection cornerDirection, bool topCorner)
        {
            HexPillarCorner cornerObject = (topCorner ? pillar.topEnd.corners : pillar.bottomEnd.corners)[(int)cornerDirection];

            if (!HexTerrainEditor.xRayMode && HexTerrainEditor.IsPointObscured(cornerObject.transform.position))
            {
                return;
            }

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
                    bool addToSelection = !selectedObjects.Contains(cornerObject.gameObject);

                    foreach (HexPillarCorner overlappingCorner in HexHelper.GetOverlappingCorners(cornerObject))
                    {
                        if (addToSelection)
                        {
                            if (!selectedObjects.Contains(overlappingCorner.gameObject) &&
                                HexTerrainEditor.selectedPillars.ToList().Contains(overlappingCorner.end.pillar))
                                selectedObjects.Insert(0, overlappingCorner.gameObject);
                        }
                        else
                        {
                            selectedObjects.Remove(overlappingCorner.gameObject);
                        }
                    }
                }
                else
                {
                    foreach (GameObject selectedObject in Selection.objects)
                    {
                        if (selectedObject.GetComponent<HexPillarCorner>() ||
                            selectedObject.GetComponent<HexPillarEnd>())
                        {
                            selectedObjects.Remove(selectedObject);
                        }
                    }

                    foreach (HexPillarCorner overlappingCorner in HexHelper.GetOverlappingCorners(cornerObject))
                    {
                        if (HexTerrainEditor.selectedPillars.ToList().Contains(overlappingCorner.end.pillar))
                            selectedObjects.Insert(0, overlappingCorner.gameObject);
                    }
                }

                Selection.objects = selectedObjects.ToArray();
            }
        }

        static void MoveSelectedCorners(float amount)
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                HexPillarCorner selectedCorner = selectedObject.GetComponent<HexPillarCorner>();

                if (!selectedCorner)
                    continue;

                if (selectedCorner.isOnTopEnd)
                    selectedCorner.height = Mathf.Max(selectedCorner.height + amount, selectedCorner.end.pillar.bottomEnd.corners[(int)selectedCorner.direction].height);
                else
                    selectedCorner.height = Mathf.Min(selectedCorner.height + amount, selectedCorner.end.pillar.topEnd.corners[(int)selectedCorner.direction].height);

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

        public static void UpdateOverlappingSelections()
        {
            List<Object> selectedObjects = Selection.objects.ToList();
            List<HexPillarCorner> processedCorners = new List<HexPillarCorner>();

            foreach (HexPillarCorner selectedCorner in HexTerrainEditor.selectedCorners)
            {
                if (processedCorners.Contains(selectedCorner))
                    continue;

                foreach (HexPillarCorner overlappingCorner in HexHelper.GetOverlappingCorners(selectedCorner))
                {
                    if (selectedObjects.Contains(overlappingCorner.end.pillar.gameObject) &&
                        !selectedObjects.Contains(overlappingCorner.gameObject))
                    {
                        selectedObjects.Insert(0, overlappingCorner.gameObject);
                    }
                    else if (!selectedObjects.Contains(overlappingCorner.end.pillar.gameObject) &&
                        selectedObjects.Contains(overlappingCorner.gameObject))
                    {
                        selectedObjects.Remove(overlappingCorner.gameObject);
                    }

                    processedCorners.Add(overlappingCorner);
                }
            }

            Selection.objects = selectedObjects.ToArray();
        }

        public static bool ShouldHideUnityTools()
        {
            return HexTerrainEditor.selectedCorners != null && HexTerrainEditor.selectedCorners.Length > 0;
        }
    }
}