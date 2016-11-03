namespace HexTerrain
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Linq;

    public static class HexPillarCornerEditor
    {
        public static void UpdateVerticesMode()
        {
            foreach (HexPillarCorner selectedCorner in HexTerrainEditor.selectedCorners)
            {
                float delta = Handle(selectedCorner);

                if (delta != 0f)
                {
                    MoveSelectedCorners(delta);
                    HexPillarEndEditor.MoveCenterOfSelectedEnds(delta, selectedCorner.end.isTopEnd);
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

        static void MoveCorner(HexPillarCorner corner, float amount)
        {
            if (corner.isOnTopEnd)
                corner.height = Mathf.Max(corner.height + amount, corner.end.pillar.bottomEnd.corners[(int)corner.direction].height);
            else
                corner.height = Mathf.Min(corner.height + amount, corner.end.pillar.topEnd.corners[(int)corner.direction].height);

            corner.height = Mathf.Round(corner.height / corner.end.pillar.terrain.heightSnap) * corner.end.pillar.terrain.heightSnap;
        }

        public static void MoveSelectedCorners(float amount)
        {
            foreach (HexPillarCorner selectedCorner in HexTerrainEditor.selectedCorners)
            {
                MoveCorner(selectedCorner, amount);
            }
        }

        public static void MoveCornersOnEnd(HexPillarEnd end, float amount)
        {
            foreach (HexPillarCorner corner in end.corners)
            {
                MoveCorner(corner, end.isTopEnd ? amount : -amount);
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
            return HexTerrainEditor.selectedCorners != null && HexTerrainEditor.selectedCorners.Count > 0;
        }
    }
}