using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace HexTerrain
{
    public static class HexPillarEndEditor
    {
        public static void OnSelectionModePillars()
        {
            foreach (HexPillar selectedPillar in HexTerrainEditor.selectedPillars)
            {
                foreach (HexPillarEnd selectedEnd in new HexPillarEnd[] { selectedPillar.topEnd, selectedPillar.bottomEnd })
                {
                    if (!HexTerrainEditor.xRayMode && HexTerrainEditor.IsPointObscured(selectedEnd.transform.position))
                    {
                        continue;
                    }

                    float delta = Handle(selectedEnd, 1f, new Color(1f, 0.75f, 0f));

                    if (delta != 0f)
                    {
                        if (selectedEnd.isTopEnd)
                            MoveTopsOfSelectedPillars(delta);
                        else
                            MoveBottomsOfSelectedPillars(delta);

                        HexTerrainEditor.RedrawSelections();
                    }
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
                    MoveCenterOfSelectedEnds(delta, selectedEnd.isTopEnd);
                    HexPillarCornerEditor.MoveSelectedCorners(delta);
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

        public static void SelectionButton(HexPillar pillar, bool topEnd)
        {
            HexPillarEnd endObject = topEnd ? pillar.topEnd : pillar.bottomEnd;

            if (!HexTerrainEditor.xRayMode && HexTerrainEditor.IsPointObscured(endObject.transform.position))
            {
                return;
            }

            List<Object> selectedObjects = Selection.objects.ToList();

            if (selectedObjects.Contains(endObject.gameObject))
                Handles.color = new Color(0f, 1.0f, 0.25f);
            else
                Handles.color = new Color(0f, 0.5f, 0.125f);

            if (Handles.Button(
                endObject.transform.position,
                Quaternion.identity,
                0.05f, 0.05f,
                Handles.DotCap))
            {
                if (Event.current.shift)
                {
                    if (selectedObjects.Contains(endObject.gameObject))
                    {
                        selectedObjects.Remove(endObject.gameObject);
                    }
                    else
                    {
                        selectedObjects.Insert(0, endObject.gameObject);
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

                    selectedObjects.Insert(0, endObject.gameObject);
                }

                Selection.objects = selectedObjects.ToArray();
            }
        }

        public static void MoveEndByAmount(HexPillarEnd end, float amount, bool moveAllCorners)
        {
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

        public static void MoveTopsOfSelectedPillars(float amount)
        {
            foreach (HexPillar selectedPillar in HexTerrainEditor.selectedPillars)
            {
                foreach (HexPillarEnd selectedEnd in new HexPillarEnd[] { selectedPillar.topEnd, selectedPillar.bottomEnd })
                {
                    if (selectedEnd.isTopEnd)
                        MoveEndByAmount(selectedEnd, amount, true);
                }
            }
        }

        public static void MoveBottomsOfSelectedPillars(float amount)
        {
            foreach (HexPillar selectedPillar in HexTerrainEditor.selectedPillars)
            {
                foreach (HexPillarEnd selectedEnd in new HexPillarEnd[] { selectedPillar.topEnd, selectedPillar.bottomEnd })
                {
                    if (!selectedEnd.isTopEnd)
                        MoveEndByAmount(selectedEnd, -amount, true);
                }
            }
        }

        public static void MoveCenterOfSelectedEnds(float amount, bool handlePointsUp)
        {
            foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
            {
                float realAmount = (selectedEnd.isTopEnd == handlePointsUp) ? amount : -amount;

                MoveEndByAmount(selectedEnd, selectedEnd.isTopEnd ? realAmount : -realAmount, true);
                HexPillarCornerEditor.MoveCornersOnEnd(selectedEnd, -realAmount);
            }
        }

        public static bool ShouldHideUnityTools()
        {
            return HexTerrainEditor.selectedEnds != null && HexTerrainEditor.selectedEnds.Length > 0;
        }
    }
}