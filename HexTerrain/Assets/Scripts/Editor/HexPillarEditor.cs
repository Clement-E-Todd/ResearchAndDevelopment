using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace HexTerrain
{
    public static class HexPillarEditor
    {
        public static void UpdatePillarsMode()
        {
            if (HexTerrainEditor.selectedPillars == null)
                return;

            foreach (HexPillar selectedPillar in HexTerrainEditor.selectedPillars)
            {
                float delta = Handle(selectedPillar);

                if (delta != 0f)
                {
                    HexPillarEndEditor.MoveEndByAmount(selectedPillar.topEnd, delta, true);
                    HexPillarEndEditor.MoveEndByAmount(selectedPillar.bottomEnd, delta, true);
                    HexTerrainEditor.RedrawSelections();
                }
            }
        }

        public static void UpdateVerticesMode()
        {
            if (HexTerrainEditor.selectedPillars == null)
                return;

            foreach (HexPillar selectedPillar in HexTerrainEditor.selectedPillars)
            {
                for (HexCornerDirection direction = 0; direction < HexCornerDirection.MAX; ++direction)
                {
                    HexPillarCornerEditor.SelectionButton(selectedPillar, direction, true);
                    HexPillarCornerEditor.SelectionButton(selectedPillar, direction, false);
                }

                HexPillarEndEditor.SelectionButton(selectedPillar, true);
                HexPillarEndEditor.SelectionButton(selectedPillar, false);
            }
        }

        static float Handle(HexPillar pillar)
        {
            Vector3 positionBefore = pillar.transform.TransformPoint((pillar.topEnd.transform.localPosition + pillar.bottomEnd.transform.localPosition) / 2);
            Vector3 direction = pillar.transform.TransformDirection(pillar.transform.up);

            Handles.color = new Color(1f, 0.75f, 0f);

            EditorGUI.BeginChangeCheck();
            Vector3 positionAfter = Handles.Slider(positionBefore, direction, 0.25f, Handles.CylinderCap, 0f);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (HexPillarEnd selectedEnd in HexTerrainEditor.selectedEnds)
                {
                    Undo.RecordObject(selectedEnd, "Edit Pillar");
                }
            }

            Vector3 positionDelta = positionAfter - positionBefore;
            return Vector3.Dot(positionDelta, direction);
        }

        public static void HideSelectedEdges(bool hide)
        {
            List<HexPillar> pillarsToRedraw = new List<HexPillar>();

            foreach (HexPillarCorner corner in HexTerrainEditor.selectedCorners)
            {
                HexPillarCorner clockwiseCorner = corner.end.corners[(int)HexHelper.GetCornerDirectionNextToCorner(corner.direction, true)];

                if (HexTerrainEditor.selectedCorners.Contains(clockwiseCorner))
                {
                    HexEdgeDirection clockwiseEdge = HexHelper.GetEdgeDirectionNextToCorner(corner.direction, true);

                    corner.end.pillar.hideSides[(int)clockwiseEdge] = hide;

                    if (!pillarsToRedraw.Contains(corner.end.pillar))
                    {
                        pillarsToRedraw.Add(corner.end.pillar);
                    }
                }
            }

            foreach (HexPillar pillar in pillarsToRedraw)
            {
                bool allSidesHidden = true;

                for (HexEdgeDirection direction = 0; direction < HexEdgeDirection.MAX; ++direction)
                {
                    if (!pillar.hideSides[(int)direction])
                    {
                        allSidesHidden = false;
                        break;
                    }
                }

                if (!allSidesHidden)
                    pillar.GenerateMesh();
                else
                    Undo.DestroyObjectImmediate(pillar.gameObject);
            }
        }

        public static bool ShouldHideUnityTools()
        {
            return HexTerrainEditor.selectedPillars != null && (HexTerrainEditor.selectedPillars.Count > 0 || HexPillarCreationTool.creationInProgress);
        }
    }
}